using System;
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
                var loadAssemblyResult = AssemblyUtils.Load(startPath);
                if (loadAssemblyResult.ErrorMessage != null)
                {
                    results.Fail(loadAssemblyResult.ErrorMessage);
                    continue;
                }

                if (loadAssemblyResult.Assembly == null)
                {
                    continue;
                }

                var tryIsDebugResponse = AssemblyUtils.TryIsDebug(loadAssemblyResult.Assembly);

                if (tryIsDebugResponse.ErrorMessage != null)
                {
                    results.Fail(tryIsDebugResponse.ErrorMessage);
                    continue;
                }

                if (tryIsDebugResponse.Result != null)
                {
                    results.Ok(startPath, tryIsDebugResponse.Result);
                }
            }

            return results;
        }
    }
}
