namespace UnitTest
{
    using System;
    #if !OPEN_TO_GITHUB
    using System.IO;
    using Cs.Backend.Config;
    using Cs.Messaging;
    using Cs.Templets.Base;
    #endif

    [TestClass]
    public static class GlobalSetup
    {
        private static string solutionPath = string.Empty;

        [AssemblyInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            #if !OPEN_TO_GITHUB
            FindSolutionPath();
            var serverTempletPath = Path.Combine(solutionPath, "../StarServerAsset/ServerTemplet/Bytes");
            var clientScriptPath = Path.Combine(solutionPath, "../StarClient/Assets/ASSET_BUNDLE/AB_BUILTIN/ABB_SCRIPT_TXT");
            TempletPathResolver.Initialize(serverTempletPath, clientScriptPath, commonBasePath: string.Empty);
            ConfigPathResolver.Initialize(solutionPath);

            GlobalTimer.Start(includeCaller: false);
            #endif
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
