using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetAssemblyInformer.Core;

namespace DotNetAssemblyInformer.Utils
{
    internal class AssemblyUtils
    {
        public static LoadAssemblyResponse Load(string assemblyPath)
        {
            var response = new LoadAssemblyResponse();

            if (string.IsNullOrEmpty(assemblyPath))
            {
                response.ErrorMessage = "Empty path";
                return response;
            }

            if (!File.Exists(assemblyPath))
            {
                response.ErrorMessage = $"Could not load {assemblyPath}. Reason: not found.";
                return response;
            }

            try
            {
                response.Assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
            catch (Exception exception)
            {
                response.ErrorMessage = $"Could not load {assemblyPath}. Reason: {exception.Message}";
            }

            return response;
        }

        public static TryIsDebugResponse TryIsDebug(Assembly assembly)
        {
            var response = new TryIsDebugResponse
                           {
                               Result = new IsDebugResult
                                        {
                                            IsJITOptimized = true,
                                            HasDebuggableAttribute = false,
                                            DebugOutput = DebugOutputType.Undefined
                                        }
                           };

            CustomAttributeData reflectedAttribute;

            try
            {
                reflectedAttribute = CustomAttributeData.GetCustomAttributes(assembly)
                    .FirstOrDefault(data => data.AttributeType == typeof(DebuggableAttribute));
            }
            catch (Exception exception)
            {
                response.ErrorMessage = $"Could not load {assembly.FullName}. Reason: {exception.Message}";
                return response;
            }

            if (reflectedAttribute == null)
            {
                return response;
            }

            var debuggingModesArgument = reflectedAttribute.ConstructorArguments
                .FirstOrDefault(argument => argument.ArgumentType == typeof(DebuggableAttribute.DebuggingModes));

            var argumentIntValue = debuggingModesArgument.Value as int?;
            if (argumentIntValue != null)
            {
                var debuggableAttribute = new DebuggableAttribute((DebuggableAttribute.DebuggingModes)argumentIntValue.Value);

                response.Result.HasDebuggableAttribute = true;
                response.Result.IsJITOptimized = !debuggableAttribute.IsJITOptimizerDisabled;

                // check for Debug Output "full" or "pdb-only"
                var output = GetDebugOutputType(debuggableAttribute);
                response.Result.DebugOutput = output;
            }

            return response;
        }

        private static DebugOutputType GetDebugOutputType(DebuggableAttribute debuggableAttribute)
        {
            return (debuggableAttribute.DebuggingFlags & DebuggableAttribute.DebuggingModes.Default) !=
                   DebuggableAttribute.DebuggingModes.None
                ? DebugOutputType.Full
                : DebugOutputType.PdbOnly;
        }
    }
}
