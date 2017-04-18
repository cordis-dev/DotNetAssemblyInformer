using System;

namespace DotNetAssemblyInformer.Core
{
    public class RunnerSettings
    {
        public bool Recursive { get; set; }
        public string[] StartPaths { get; set; }

        public RunnerSettings()
        {
            Recursive = false;
            StartPaths = new[] { Environment.CurrentDirectory };
        }
    }
}
