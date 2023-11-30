namespace UnitTest
{
    using System;
    using System.IO;
    using Cs.Backend.Config;
    using Cs.Messaging;
    using Cs.Templets.Base;

    [TestClass]
    public static class GlobalSetup
    {
        private static string solutionPath = string.Empty;

        [AssemblyInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            FindSolutionPath();
            var serverScriptPath = Path.Combine(solutionPath, "TempletBin");
            var clientScriptPath = Path.Combine(solutionPath, "../StarClient/Assets/ASSET_BUNDLE/AB_BUILTIN/ABB_SCRIPT_TXT");
            TempletPathResolver.Initialize(serverScriptPath, clientScriptPath);
            ConfigPathResolver.Initialize(solutionPath);

            GlobalTimer.Start(includeCaller: false);
        }

        private static void FindSolutionPath()
        {
            var currentPath = Environment.CurrentDirectory;
            int index = currentPath.IndexOf(@"Test\UnitTest");
            if (index < 0)
            {
                throw new Exception($"cannot find solution path. currentPath:{currentPath}");
            }

            solutionPath = currentPath[..index];
        }
    }
}
