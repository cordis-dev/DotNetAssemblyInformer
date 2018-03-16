using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DotNetAssemblyInformer.Core;

namespace DotNetAssemblyInformer.Utils
{
    internal class AssemblyUtils
    {
        /// <summary>
        /// Load an assembly for a given path.
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly to load</param>
        /// <param name="assembly">Loaded Assembly, null if failed</param>
        /// <returns>True is the assembly was successfully loaded, false otherwise.</returns>
        public static string Load(string assemblyPath, out Assembly assembly)
        {
            assembly = null;

            if (string.IsNullOrEmpty(assemblyPath))
            {
                return string.Empty;
            }

            if (!File.Exists(assemblyPath))
            {
                return $"Could not load {assemblyPath}. Reason: not found.";
            }

            try
            {
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            catch (FileLoadException fex)
            {
                return $"Could not load {assemblyPath}. Reason: {fex.Message}";
            }
            catch (BadImageFormatException biex)
            {
                return $"Could not load {assemblyPath}. Reason: {biex.Message}";
            }

            return string.Empty;
        }

        /// <summary>
        /// see http://dave-black.blogspot.fr/2011/12/how-to-tell-if-assembly-is-debug-or.html
        /// </summary>
        /// <param name="assembly">Assembly to test</param>
        /// <returns>IsDebugResult instance.</returns>
        public static string TryIsDebug(Assembly assembly, out IsDebugResult result)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            result = new IsDebugResult();
            object[] attribs;
            try
            {
                attribs = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);
            }
            catch (FileNotFoundException fne)
            {
                return $"Could not load {fne.FileName}. Reason : {assembly.FullName}";
            }
            catch (TypeLoadException typeLoadException)
            {
                return $"Could not load {assembly.FullName}. Reason : {typeLoadException.Message}";
            }

            // If the 'DebuggableAttribute' is not found then it is definitely an OPTIMIZED build
            if (attribs.Length > 0)
            {
                // Just because the 'DebuggableAttribute' is found doesn't necessarily mean
                // it's a DEBUG build; we have to check the JIT Optimization flag
                // i.e. it could have the "generate PDB" checked but have JIT Optimization enabled
                var debuggableAttribute = attribs[0] as DebuggableAttribute;
                if (debuggableAttribute != null)
                {
                    result.HasDebuggableAttribute = true;
                    result.IsJITOptimized = !debuggableAttribute.IsJITOptimizerDisabled;
                    // check for Debug Output "full" or "pdb-only"
                    result.DebugOutput = (debuggableAttribute.DebuggingFlags &
                                    DebuggableAttribute.DebuggingModes.Default) !=
                                    DebuggableAttribute.DebuggingModes.None
                                    ? DebugOutputType.Full : DebugOutputType.PdbOnly;
                }
            }
            else
            {
                result.IsJITOptimized = true;
            }

            return string.Empty;
        }
    }
}
