using System;
using System.Reflection;
using DotNetAssemblyInformer.Utils;

namespace DotNetAssemblyInformer.Core
{
    public class Runner
    {
        private readonly RunnerSettings settings;

        public Runner(RunnerSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            this.settings = settings;
        }

        public RunnerResult Run()
        {
            var results = new RunnerResult();

            foreach (var startPath in settings.StartPaths)
            {
                Assembly assembly;
                string errorMessage;
                if ((errorMessage = AssemblyUtils.Load(startPath, out assembly)) == string.Empty)
                {
                    IsDebugResult isdebugResult;
                    string debugerror = string.Empty;
                    if ((debugerror = AssemblyUtils.TryIsDebug(assembly, out isdebugResult)) == string.Empty)
                        results.Ok(startPath, isdebugResult);
                    else
                        results.Fail(errorMessage);
                }
                else
                    results.Fail(errorMessage);
            }

            return results;
        }
    }
}
