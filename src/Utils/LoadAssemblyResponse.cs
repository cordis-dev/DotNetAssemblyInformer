using System.Reflection;

namespace DotNetAssemblyInformer.Utils
{
    internal class LoadAssemblyResponse
    {
        public Assembly Assembly { get; set; }
        public string ErrorMessage { get; set; }
    }
}