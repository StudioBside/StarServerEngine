namespace Cs.Core.Interfaces;

public interface INetworkResolver
{
    string LocalIp { get; }
    string WslHostIp { get; }
}
