namespace Cs.Logging
{
    using System;

    public readonly struct LogProviderSwitcher : IDisposable
    {
        private readonly ILogProvider prevProvider;

        public LogProviderSwitcher(ILogProvider newProvider)
        {
            this.prevProvider = Log.Provider;
            Log.Provider = newProvider;
        }

        public void Dispose()
        {
            Log.Provider = this.prevProvider;
        }
    }
}
