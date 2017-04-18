namespace DotNetAssemblyInformer.Core
{
    public class IsDebugResult
    {
        public bool? HasDebuggableAttribute { get; set; }
        public bool? IsJITOptimized { get; set; }

        public BuildType Build => IsJITOptimized ?? false ? BuildType.Release : BuildType.Debug;

        public DebugOutputType DebugOutput { get; set; }
    }
}
