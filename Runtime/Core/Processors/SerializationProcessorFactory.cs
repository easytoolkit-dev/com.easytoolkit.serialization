using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core.Reflection;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization.Processors
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
        private static readonly ConcurrentDictionary<Type, ISerializationProcessor> ProcessorCache;
        private static readonly ConcurrentDictionary<Type, bool> InstantiableCache;

        static SerializationProcessorFactory()
        {
            TypeMatcher = TypeMatcherFactory.CreateDefault();
            ProcessorCache = new ConcurrentDictionary<Type, ISerializationProcessor>();
            InstantiableCache = new ConcurrentDictionary<Type, bool>();
            InitializeTypeMatcher();
        }

        private static void InitializeTypeMatcher()
        {
            TypeMatcher.SetTypeMatchCandidates(ProcessorTypes.Select(type =>
            {
                var config = type.GetCustomAttribute<ProcessorConfigurationAttribute>();
                config ??= ProcessorConfigurationAttribute.Default;

                var argType = type.GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>));
                return new TypeMatchCandidate(type, config.Priority, argType);
            }));
        }

        public static ISerializationProcessor GetProcessor(Type valueType)
        {
            var isInstantiable = InstantiableCache.GetOrAdd(valueType,
                type => type.IsInstantiable(allowLenient: true) || type.IsArray || type == typeof(string));

            if (!isInstantiable)
            {
                throw new InvalidOperationException($"Type '{valueType.FullName}' is not instantiable.");
            }

            return ProcessorCache.GetOrAdd(valueType, type =>
            {
                var processor = CreateProcessor(type);
                if (processor == null)
                    return null;
                InjectDependencyToProcessor(processor);

                var baseValueType = processor.GetType().GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>))[0];
                if (baseValueType != type)
                {
                    if (!type.IsDerivedFrom(baseValueType))
                    {
                        throw new InvalidOperationException(
                            $"Type '{type.FullName}' is not derived from '{baseValueType.FullName}'.");
                    }
                    var processorWrapperType = typeof(Implementations.SerializationProcessorWrapper<,>).MakeGenericType(type, baseValueType);
                    return processorWrapperType.CreateInstance<ISerializationProcessor>(processor);
                }

                return processor;
            });
        }

        private static void InjectDependencyToProcessor([NotNull] ISerializationProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            foreach (var memberInfo in processor.GetType().GetMembers(MemberAccessFlags.All))
            {
                if (memberInfo.GetCustomAttribute<DependencyProcessorAttribute>() != null)
                {
                    var memberType = memberInfo.GetMemberType();
                    if (!memberType.IsImplementsGenericDefinition(typeof(ISerializationProcessor<>)))
                    {
                        throw new InvalidOperationException(
                            $"Member '{memberInfo.Name}' of type '{memberType.FullName}' is not a ISerializationProcessor<T>.");
                    }
                    var valueType = memberType.GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>))[0];

                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        var dependency = GetProcessor(valueType);
                        fieldInfo.SetValue(processor, dependency);
                    }
                    else if (memberInfo is PropertyInfo propertyInfo)
                    {
                        var dependency = GetProcessor(valueType);
                        propertyInfo.GetSetMethod(true).Invoke(processor, new object[] { dependency });
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Member '{memberInfo.Name}' of type '{memberType.FullName}' is not a field or property.");
                    }
                }
            }
        }

        [CanBeNull]
        private static ISerializationProcessor CreateProcessor(Type valueType)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                TypeMatcher.GetMatches(Type.EmptyTypes),
                TypeMatcher.GetMatches(valueType)
            };
            var results = TypeMatcher.GetMergedResults(resultsList);
            foreach (var result in results)
            {
                if (result.Constraints[0] != valueType)
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

        private static bool CanProcessType(Type serializerType, Type valueType)
        {
            var serializer = (ISerializationProcessor)FormatterServices.GetUninitializedObject(serializerType);
            return serializer.CanProcess(valueType);
        }
    }
}
