using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolkit.Core.Mathematics;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Core.Textual;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Processors
{
    public static class SerializationProcessorFactory
    {
        private static Type[] s_processorTypes;

        private static Type[] ProcessorTypes
        {
            get
            {
                if (s_processorTypes == null)
                {
                    s_processorTypes = AssemblyUtility.GetTypes(AssemblyCategory.Custom)
                        .Where(type => type.IsClass && !type.IsInterface && !type.IsAbstract &&
                                       type.IsDerivedFrom<ISerializationProcessor>())
                        .ToArray();
                }

                return s_processorTypes;
            }
        }

        private static readonly ITypeMatcher TypeMatcher;

        static SerializationProcessorFactory()
        {
            TypeMatcher = TypeMatcherFactory.CreateDefault();
            InitializeTypeMatcher();
        }

        private static void InitializeTypeMatcher()
        {
            TypeMatcher.SetTypeMatchCandidates(ProcessorTypes
                .OrderByDescending(GetProcessorPriority)
                .Select((type, i) =>
                {
                    var argType = type.GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>));
                    return new TypeMatchCandidate(type, ProcessorTypes.Length - i, argType);
                }));
        }

        /// <summary>
        /// Creates the processor for the specified value type
        /// </summary>
        /// <param name="valueType">The type to get a processor for.</param>
        /// <returns>The created processor.</returns>
        public static ISerializationProcessor CreateProcessor(Type valueType, SerializationContext context)
        {
            return CreateProcessor(valueType, context, null);
        }

        private static void InjectDependencyToProcessor([NotNull] ISerializationProcessor processor, SerializationContext context)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            foreach (var memberInfo in processor.GetType().GetMembers(MemberAccessFlags.All))
            {
                var attribute = memberInfo.GetCustomAttribute<DependencyProcessorAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var memberType = memberInfo.GetMemberType();
                if (!memberType.IsImplementsGenericDefinition(typeof(ISerializationProcessor<>)))
                {
                    throw new InvalidOperationException(
                        $"Member '{memberInfo.Name}' of type '{memberType.FullName}' is not a ISerializationProcessor<T>.");
                }

                var valueType = memberType.GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>))[0];

                // Get candidate and excluded types from attribute
                var candidateTypes = GetTypesFromExpression(processor, attribute.CandidateTypesGetter, memberInfo.Name);
                var excludedTypes = GetTypesFromExpression(processor, attribute.ExcludedTypesGetter, memberInfo.Name);

                ISerializationProcessor dependency;

                // If candidate types are specified, use filtered processor creation
                if (candidateTypes != null || excludedTypes != null)
                {
                    var filteredTypes = FilterProcessorTypes(candidateTypes, excludedTypes);
                    dependency = CreateProcessor(valueType, context, filteredTypes);

                    if (dependency == null)
                    {
                        throw new InvalidOperationException(
                            $"No suitable processor found for type '{valueType.FullName}' " +
                            $"from candidates [{string.Join(", ", filteredTypes.Select(t => t.Name))}] " +
                            $"for member '{memberInfo.Name}'.");
                    }
                }
                else
                {
                    dependency = CreateProcessor(valueType, context);

                    if (dependency == null)
                    {
                        throw new InvalidOperationException(
                            $"No suitable processor found for type '{valueType.FullName}' " +
                            $"for member '{memberInfo.Name}'.");
                    }
                }

                // Set the dependency value
                if (memberInfo is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(processor, dependency);
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    propertyInfo.GetSetMethod(true).Invoke(processor, new object[] { dependency });
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Member '{memberInfo.Name}' of type '{memberType.FullName}' is not a field or property.");
                }
            }
        }

        private static ISerializationProcessor CreateProcessor(Type valueType, SerializationContext context, [CanBeNull] Type[] candidateTypes)
        {
            var processor = PureCreateProcessor(valueType, candidateTypes);
            if (processor == null)
                return null;

            processor.Context = context;
            InjectDependencyToProcessor(processor, context);
            return processor;
        }

        private static OrderPriority GetProcessorPriority(Type processorType)
        {
            OrderPriority priority;

            if (processorType.IsDefined<ProcessorConfigurationAttribute>())
            {
                priority = processorType.GetCustomAttribute<ProcessorConfigurationAttribute>().Priority;
            }
            else
            {
                priority = new OrderPriority(ProcessorPriorityLevel.Custom);
            }

            return priority;
        }

        private static bool CanProcessType(Type serializerType, Type valueType)
        {
            var serializer = (ISerializationProcessor)FormatterServices.GetUninitializedObject(serializerType);
            return serializer.CanProcess(valueType);
        }

        /// <summary>
        /// Gets the array of types from an expression path evaluated against the processor instance.
        /// </summary>
        /// <param name="processor">The processor instance to evaluate against.</param>
        /// <param name="expressionPath">The expression path to evaluate.</param>
        /// <param name="memberName">The member name for error messages.</param>
        /// <returns>The array of types, or null if the expression path is null or empty.</returns>
        private static Type[] GetTypesFromExpression(ISerializationProcessor processor, string expressionPath, string memberName)
        {
            if (expressionPath.IsNullOrWhiteSpace())
            {
                return null;
            }

            var evaluator = ExpressionEvaluatorFactory.CreateEvaluator(expressionPath, processor.GetType());
            var result = evaluator.Evaluate(processor);

            if (result == null)
            {
                return null;
            }

            if (result is IEnumerable<Type> types)
            {
                return types.ToArray();
            }

            throw new InvalidOperationException(
                $"Expression '{expressionPath}' for member '{memberName}' must return IEnumerable<Type>, " +
                $"but returned '{result.GetType().FullName}'.");
        }

        /// <summary>
        /// Filters processor types based on candidate and excluded types.
        /// </summary>
        /// <param name="candidateTypes">The candidate types to include, or null for all.</param>
        /// <param name="excludedTypes">The types to exclude, or null for none.</param>
        /// <returns>The filtered list of processor types.</returns>
        private static Type[] FilterProcessorTypes(Type[] candidateTypes, Type[] excludedTypes)
        {
            var candidates = candidateTypes ?? ProcessorTypes;
            var excluded = excludedTypes?.ToHashSet() ?? new HashSet<Type>();

            return candidates
                .Where(type => excluded.All(excludedType => type != excludedType))
                .ToArray();
        }

        /// <summary>
        /// Creates a processor from the specified candidate types for the given value type.
        /// </summary>
        /// <param name="valueType">The type to get a processor for.</param>
        /// <param name="candidateTypes">The candidate processor types.</param>
        /// <returns>The created processor, or null if no suitable processor was found.</returns>
        private static ISerializationProcessor PureCreateProcessor(Type valueType, Type[] candidateTypes = null)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                TypeMatcher.GetMatches(Type.EmptyTypes),
                TypeMatcher.GetMatches(valueType)
            };

            var results = TypeMatcher.GetMergedResults(resultsList);

            // Build a HashSet of candidate types for quick lookup
            var candidateSet = candidateTypes?.ToHashSet();

            foreach (var result in results)
            {
                if (result.Constraints[0] != valueType)
                {
                    continue;
                }

                // Check if the matched type is in the candidate set
                if (candidateSet != null && !candidateSet.Contains(result.MatchedType))
                {
                    continue;
                }

                if (CanProcessType(result.MatchedType, valueType))
                {
                    return result.MatchedType.CreateInstance<ISerializationProcessor>();
                }
            }

            return null;
        }
    }
}
