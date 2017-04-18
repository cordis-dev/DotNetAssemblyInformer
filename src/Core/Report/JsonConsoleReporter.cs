using System;

namespace DotNetAssemblyInformer.Core.Report
{
    internal class JsonConsoleReporter : BasicConsoleReporter
    {
        public override void Generate(RunnerResult runnerResult)
        {
            var serializer = new NewtonsoftJsonSerializer();
            var serialize = serializer.Serialize(runnerResult);
            Console.WriteLine(serialize);
        }
    }
}