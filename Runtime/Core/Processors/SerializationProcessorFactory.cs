using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using EasyToolkit.Core.Collections;
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
        /// <param name="context">The serialization context.</param>
        /// <param name="parent">The parent processor in the serialization hierarchy.</param>
        /// <returns>The created processor.</returns>
        public static ISerializationProcessor CreateProcessor(Type valueType, SerializationContext context, [CanBeNull] ISerializationProcessor parent)
        {
            return CreateProcessor(valueType, context, null, null, parent);
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
                        $"Member '{memberInfo.Name}' of type '{memberType}' is not a ISerializationProcessor<T>.");
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
                    dependency = CreateProcessor(valueType, context, filteredTypes, processor.GetType(), processor);

                    if (dependency == null)
                    {
                        var stringBuilder = new StringBuilder();
                        stringBuilder.Append($"No suitable processor found for type '{valueType}' ");
                        if (candidateTypes.IsNotNullOrEmpty() || excludedTypes.IsNotNullOrEmpty())
                        {
                            stringBuilder.Append("from ");

                            if (candidateTypes.IsNotNullOrEmpty())
                            {
                                stringBuilder.Append($"candidateTypes [{string.Join(", ", candidateTypes!.Select(t => t.ToString()))}]");
                            }
                            else
                            {
                                stringBuilder.Append($"excludedTypes [{string.Join(", ", excludedTypes!.Select(t => t.ToString()))}]");
                            }
                        }
                        stringBuilder.Append($"for member '{memberInfo.Name}'.");

                        throw new InvalidOperationException(stringBuilder.ToString());
                    }
                }
                else
                {
                    dependency = CreateProcessor(valueType, context, null, processor.GetType(), processor);

                    if (dependency == null)
                    {
                        throw new InvalidOperationException(
                            $"No suitable processor found for type '{valueType}' " +
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
                        $"Member '{memberInfo.Name}' of type '{memberType}' is not a field or property.");
                }
            }
        }

        private static ISerializationProcessor CreateProcessor(
            Type valueType,
            SerializationContext context,
            [CanBeNull] Type[] candidateTypes,
            [CanBeNull] Type owningProcessorType,
            [CanBeNull] ISerializationProcessor parent)
        {
            var processor = PureCreateProcessor(valueType, context, candidateTypes);
            if (processor == null)
                return null;

            if (processor.GetType() == owningProcessorType)
            {
                throw new InvalidOperationException(
                    $"Circular dependency detected: Processor '{owningProcessorType}' " +
                    $"cannot be injected into itself. To fix this, add ExcludedTypes to the " +
                    $"[DependencyProcessor] attribute to exclude the owning processor type. " +
                    $"Example: [DependencyProcessor(ExcludedTypesGetter = nameof(ExcludedTypes))] " +
                    $"with private static readonly Type[] ExcludedTypes = {{ typeof({owningProcessorType}) }};");
            }

            processor.Context = context;
            processor.Parent = parent;
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

        private static bool CanProcessType(Type serializerType, Type valueType, SerializationContext context)
        {
            var serializer = (ISerializationProcessor)FormatterServices.GetUninitializedObject(serializerType);
            return serializer.CanProcess(valueType, context);
        }

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
                $"but returned '{result.GetType()}'.");
        }

        private static Type[] FilterProcessorTypes(Type[] candidateTypes, Type[] excludedTypes)
        {
            candidateTypes ??= ProcessorTypes;
            var excludedTypeSet = excludedTypes?.ToHashSet() ?? new HashSet<Type>();

            return candidateTypes
                .Where(candidateType => excludedTypeSet.All(excludedType =>
                {
                    if (excludedType == candidateType)
                    {
                        return false;
                    }

                    return !excludedType.IsGenericType || excludedType.GetGenericTypeDefinition() != candidateType;
                }))
                .ToArray();
        }

        private static ISerializationProcessor PureCreateProcessor(
            Type valueType,
            SerializationContext context,
            [CanBeNull] Type[] candidateTypes)
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
                if (candidateSet != null && !candidateSet.Contains(result.Candidate.SourceType))
                {
                    continue;
                }

                if (CanProcessType(result.MatchedType, valueType, context))
                {
                    return result.MatchedType.CreateInstance<ISerializationProcessor>();
                }
            }

            return null;
        }
    }
}
