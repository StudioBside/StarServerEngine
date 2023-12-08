namespace Cs.Legacy
{
    using System;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Cs.Logging;

    [SupportedOSPlatform("windows")]
    public static class ServiceOperator
    {
        public static bool StartService(string serviceName, TimeSpan waitSpan)
        {
            try
            {
                using (var service = new System.ServiceProcess.ServiceController(serviceName))
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, waitSpan);
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public static bool StopService(string serviceName, TimeSpan waitSpan)
        {
            try
            {
                using (var service = new System.ServiceProcess.ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, waitSpan);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public static bool IsServiceAlive(string serviceName)
        {
            try
            {
                using (var service = new System.ServiceProcess.ServiceController(serviceName))
                {
                    return service.Status == ServiceControllerStatus.Running;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public static bool RestartService(string serviceName, TimeSpan waitSpan)
        {
            try
            {
                using (var service = new System.ServiceProcess.ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, waitSpan);
                    }

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, waitSpan);
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public static Task<bool> RestartServiceAsync(string serviceName, TimeSpan waitSpan)
        {
            return Task.Run(() =>
            {
                return RestartService(serviceName, waitSpan);
            });
        }

        public static bool IsServiceInstalled(string serviceName)
        {
            var result = System.ServiceProcess.ServiceController.GetServices()
                .Where(e => e.ServiceName == serviceName)
                .FirstOrDefault();

            return result != null;
        }
    }
}
