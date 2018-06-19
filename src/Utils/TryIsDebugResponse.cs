using DotNetAssemblyInformer.Core;

namespace DotNetAssemblyInformer.Utils
{
    internal class TryIsDebugResponse
    {
        public IsDebugResult Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}