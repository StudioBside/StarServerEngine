namespace Cs.HttpClient.Detail
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    // https://github.com/MihaZupan/DnsRoundRobin/blob/main/DnsRoundRobin/DnsRoundRobinConnector.cs
    public sealed class DnsRoundRobinConnector : IDisposable
    {
        private const int DefaultDnsRefreshIntervalSeconds = 2 * 60;
        private const int MaxCleanupIntervalSeconds = 60;

#pragma warning disable CA2213 // 삭제 가능한 필드는 삭제해야 합니다.
        private readonly Timer cleanupTimer;
#pragma warning restore CA2213 // 삭제 가능한 필드는 삭제해야 합니다.
        private readonly ConcurrentDictionary<string, HostRoundRobinState> states = new(StringComparer.Ordinal);
        private readonly TimeSpan cleanupInterval;
        private readonly long cleanupIntervalTicks;
        private readonly long dnsRefreshTimeoutTicks;
        private readonly TimeSpan endpointConnectTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsRoundRobinConnector"/> class.
        /// </summary>
        /// <param name="dnsRefreshInterval">Maximum amount of time a Dns resolution is cached for. Default to 2 minutes.</param>
        /// <param name="endpointConnectTimeout">Maximum amount of time allowed for a connection attempt to any individual endpoint. Defaults to infinite.</param>
        public DnsRoundRobinConnector(TimeSpan? dnsRefreshInterval = null, TimeSpan? endpointConnectTimeout = null)
        {
            dnsRefreshInterval = TimeSpan.FromSeconds(Math.Max(1, dnsRefreshInterval?.TotalSeconds ?? DefaultDnsRefreshIntervalSeconds));
            this.cleanupInterval = TimeSpan.FromSeconds(Math.Clamp(dnsRefreshInterval.Value.TotalSeconds / 2, 1, MaxCleanupIntervalSeconds));
            this.cleanupIntervalTicks = (long)(this.cleanupInterval.TotalSeconds * Stopwatch.Frequency);
            this.dnsRefreshTimeoutTicks = (long)(dnsRefreshInterval.Value.TotalSeconds * Stopwatch.Frequency);
            this.endpointConnectTimeout = endpointConnectTimeout is null || endpointConnectTimeout.Value.Ticks < 1 ? Timeout.InfiniteTimeSpan : endpointConnectTimeout.Value;

            bool restoreFlow = false;
            try
            {
                // Don't capture the current ExecutionContext and its AsyncLocals onto the timer causing them to live forever
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                // Ensure the Timer has a weak reference to the connector; otherwise, it
                // can introduce a cycle that keeps the connector rooted by the Timer
                this.cleanupTimer = new Timer(
                    static state =>
                    {
                        var thisWeakRef = (WeakReference<DnsRoundRobinConnector>)state!;
                        if (thisWeakRef.TryGetTarget(out DnsRoundRobinConnector? thisRef))
                        {
                            thisRef.Cleanup();
                            thisRef.cleanupTimer.Change(thisRef.cleanupInterval, Timeout.InfiniteTimeSpan);
                        }
                    },
                    state: new WeakReference<DnsRoundRobinConnector>(this),
                    Timeout.Infinite,
                    Timeout.Infinite);

                this.cleanupTimer.Change(this.cleanupInterval, Timeout.InfiniteTimeSpan);
            }
            finally
            {
                if (restoreFlow)
                {
                    ExecutionContext.RestoreFlow();
                }
            }
        }

        public static DnsRoundRobinConnector Shared { get; } = new();

        public void Dispose()
        {
            this.states.Clear();
        }

        public Task<Socket> ConnectAsync(DnsEndPoint endPoint, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<Socket>(cancellationToken);
            }

            if (IPAddress.TryParse(endPoint.Host, out IPAddress? address))
            {
                // Avoid the overhead of HostRoundRobinState if we're dealing with a single endpoint
                return ConnectToIPAddressAsync(address, endPoint.Port, cancellationToken);
            }

            HostRoundRobinState state = this.states.GetOrAdd(
                endPoint.Host,
                static (_, thisRef) => new HostRoundRobinState(thisRef.dnsRefreshTimeoutTicks, thisRef.endpointConnectTimeout),
                this);

            return state.ConnectAsync(endPoint, cancellationToken);
        }

        private static async Task<Socket> ConnectToIPAddressAsync(IPAddress address, int port, CancellationToken cancellationToken)
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
            };

            // -------------------------------------------------------------------------------------------------------------------------
            //ServicePointManager.ReusePort = true;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, true);
            // -------------------------------------------------------------------------------------------------------------------------
            try
            {
                await socket.ConnectAsync(address, port, cancellationToken);
                return socket;
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        private void Cleanup()
        {
            long minTimestamp = Stopwatch.GetTimestamp() - this.cleanupIntervalTicks;

            foreach (KeyValuePair<string, HostRoundRobinState> state in this.states)
            {
                if (state.Value.LastAccessTimestamp < minTimestamp)
                {
                    this.states.TryRemove(state);
                }
            }
        }

        private sealed class HostRoundRobinState
        {
            private readonly long dnsRefreshTimeoutTicks;
            private readonly TimeSpan endpointConnectTimeout;
            private long lastAccessTimestamp;
            private long lastDnsTimestamp;
            private IPAddress[]? addresses;
            private uint roundRobinIndex;
            public HostRoundRobinState(long dnsRefreshTimeoutTicks, TimeSpan endpointConnectTimeout)
            {
                this.dnsRefreshTimeoutTicks = dnsRefreshTimeoutTicks;
                this.endpointConnectTimeout = endpointConnectTimeout;

                this.roundRobinIndex--; // Offset the first Increment to ensure we start with the first address in the list

                this.RefreshLastAccessTimestamp();
            }

            public long LastAccessTimestamp => Volatile.Read(ref this.lastAccessTimestamp);

            private bool AddressesAreStale => Stopwatch.GetTimestamp() - Volatile.Read(ref this.lastDnsTimestamp) > this.dnsRefreshTimeoutTicks;

            public async Task<Socket> ConnectAsync(DnsEndPoint endPoint, CancellationToken cancellationToken)
            {
                this.RefreshLastAccessTimestamp();

                uint sharedIndex = Interlocked.Increment(ref this.roundRobinIndex);
                IPAddress[]? attemptedAddresses = null;
                IPAddress[]? addresses = null;
                Exception? lastException = null;

                while (attemptedAddresses is null)
                {
                    if (addresses is null)
                    {
                        addresses = this.addresses;
                    }
                    else
                    {
                        attemptedAddresses = addresses;

                        // Give each connection attempt a chance to do its own Dns call.
                        addresses = null;
                    }

                    if (addresses is null || this.AddressesAreStale)
                    {
                        // It's possible that multiple connection attempts are resolving the same host concurrently - that's okay.
                        this.addresses = addresses = await Dns.GetHostAddressesAsync(endPoint.Host, cancellationToken);
                        Volatile.Write(ref this.lastDnsTimestamp, Stopwatch.GetTimestamp());

                        if (attemptedAddresses is not null && AddressListsAreEquivalent(attemptedAddresses, addresses))
                        {
                            // We've already tried to connect to every address in the list, and a new Dns resolution returned the same list.
                            // Instead of attempting every address again, give up early.
                            break;
                        }
                    }

                    for (int i = 0; i < addresses.Length; i++)
                    {
                        Socket? attemptSocket = null;
                        CancellationTokenSource? endpointConnectTimeoutCts = null;
                        try
                        {
                            IPAddress address = addresses[(int)((sharedIndex + i) % addresses.Length)];

                            if (Socket.OSSupportsIPv6 && address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                attemptSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                                if (address.IsIPv4MappedToIPv6)
                                {
                                    attemptSocket.DualMode = true;
                                }
                            }
                            else if (Socket.OSSupportsIPv4 && address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                attemptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            }

                            if (attemptSocket is not null)
                            {
                                attemptSocket.NoDelay = true;

                                if (this.endpointConnectTimeout != Timeout.InfiniteTimeSpan)
                                {
                                    endpointConnectTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                                    endpointConnectTimeoutCts.CancelAfter(this.endpointConnectTimeout);
                                }

                                await attemptSocket.ConnectAsync(address, endPoint.Port, endpointConnectTimeoutCts?.Token ?? cancellationToken);

                                this.RefreshLastAccessTimestamp();
                                return attemptSocket;
                            }
                        }
                        catch (Exception ex)
                        {
                            attemptSocket?.Dispose();

                            if (cancellationToken.IsCancellationRequested)
                            {
                                throw;
                            }

                            if (endpointConnectTimeoutCts?.IsCancellationRequested == true)
                            {
                                ex = new TimeoutException($"Failed to connect to any endpoint within the specified endpoint connect timeout of {this.endpointConnectTimeout.TotalSeconds:N2} seconds.", ex);
                            }

                            lastException = ex;
                        }
                        finally
                        {
                            endpointConnectTimeoutCts?.Dispose();
                        }
                    }
                }

                throw lastException ?? new SocketException((int)SocketError.NoData);
            }

            private static bool AddressListsAreEquivalent(IPAddress[] left, IPAddress[] right)
            {
                if (ReferenceEquals(left, right))
                {
                    return true;
                }

                if (left.Length != right.Length)
                {
                    return false;
                }

                for (int i = 0; i < left.Length; i++)
                {
                    if (!left[i].Equals(right[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            private void RefreshLastAccessTimestamp() => Volatile.Write(ref this.lastAccessTimestamp, Stopwatch.GetTimestamp());
        }
    }
}