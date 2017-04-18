using DotNetAssemblyInformer.Core;
using Xunit;

namespace DotNetAssemblyInformer.Tests
{
    public class RunnerTests
    {
        [Fact]
        public void IsDebug()
        {
            var runner = new Runner(new RunnerSettings { Recursive = true, StartPaths = new[] { @"..\..\..\dot-net-4.6-optimized\" } });
            var result =runner.Run();

            Assert.False(result.OverralSuccess);
        }

        [Fact]
        public void IsRelease()
        {
            var runner = new Runner(new RunnerSettings { Recursive = true, StartPaths = new[] { @"..\..\..\dot-net-4.6-unoptimized\" } });
            var result = runner.Run();

            Assert.True(result.OverralSuccess);
        }
    }
}
