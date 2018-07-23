using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetAssemblyInformer.Core;

namespace DotNetAssemblyInformer.Utils
{
    internal class AssemblyUtils
    {
        private static readonly Dictionary<string, Assembly> locationsWithAssemblies = new Dictionary<string, Assembly>();

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
                response.Assembly = GetAssembly(assemblyPath);
                locationsWithAssemblies.Add(response.Assembly.Location, response.Assembly);

                // to ensure that TryIsDebug will work whether or not assembly references have been previously loaded into AppDomain
                var referencedAssemblies = response.Assembly.GetReferencedAssemblies();
                foreach (var referencedAssembly in referencedAssemblies)
                {
                    try
                    {
                        var assembly = Assembly.ReflectionOnlyLoad(referencedAssembly.FullName);
                        locationsWithAssemblies.Add(assembly.Location, assembly);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
            }
            catch (Exception exception)
            {
                if (!exception.Message.Contains("has already loaded from a different location"))
                {
                    response.ErrorMessage = $"Could not load {assemblyPath}. Reason: {exception.Message}";
                }
            }

            return response;
        }

        private static Assembly GetAssembly(string assemblyPath)
        {
            var locationToCheck = Path.Combine(Environment.CurrentDirectory, assemblyPath);

            if (locationsWithAssemblies.ContainsKey(locationToCheck))
            {
                return locationsWithAssemblies[locationToCheck];
            }

            return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
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

            CustomAttributeTypedArgument debuggingModesArgument;

            try
            {
                var reflectedAttribute = CustomAttributeData.GetCustomAttributes(assembly)
                    .FirstOrDefault(data => data.AttributeType == typeof(DebuggableAttribute));

                if (reflectedAttribute == null)
                {
                    return response;
                }

                debuggingModesArgument = reflectedAttribute.ConstructorArguments
                    .FirstOrDefault(argument => argument.ArgumentType == typeof(DebuggableAttribute.DebuggingModes));
            }
            catch (Exception exception)
            {
                response.ErrorMessage = $"Could not load {assembly.FullName}. Reason: {exception.Message}";
                return response;
            }

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
