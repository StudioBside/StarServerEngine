namespace Cs.Core.Util
{
    using System.Net.NetworkInformation;

    public static class NicUtil
    {
        static NicUtil()
        {
            NicName = string.Empty;
            MacAddress = string.Empty;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in interfaces)
            {
                if (adapter.Supports(NetworkInterfaceComponent.IPv4) &&
                    adapter.OperationalStatus == OperationalStatus.Up &&
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    adapter.Description.StartsWith("Hyper-V") == false)
                {
                    NicName = adapter.Description.Replace('(', '[')
                        .Replace(')', ']')
                        .Replace('#', '_');

                    MacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
        }

        public static string NicName { get; }
        public static string MacAddress { get; }
    }
}
