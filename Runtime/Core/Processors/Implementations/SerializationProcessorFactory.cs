using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core.Reflection;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization.Implementations
{
    public sealed class SerializationProcessorFactory : ISerializationProcessorFactory
    {
        private readonly ITypeMatcher _typeMatcher;

        public SerializationProcessorFactory()
        {
            _typeMatcher = TypeMatcherFactory.CreateDefault();
            InitializeTypeMatcher();
        }

        private void InitializeTypeMatcher()
        {
            _typeMatcher.SetTypeMatchCandidates(SerializationProcessorUtility.ProcessorTypes.Select(type =>
            {
                var config = type.GetCustomAttribute<ProcessorConfigurationAttribute>();
                config ??= ProcessorConfigurationAttribute.Default;

                var argType = type.GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>));
                return new TypeMatchCandidate(type, config.Priority, argType);
            }));
        }

        public ISerializationProcessor<T> GetProcessor<T>()
        {
            return (ISerializationProcessor<T>)GetProcessor(typeof(T));
        }

        private ISerializationProcessor GetProcessor(Type valueType)
        {
            if (!valueType.IsInstantiable() && !valueType.IsArray && valueType != typeof(string))
            {
                throw new InvalidOperationException($"Type '{valueType.FullName}' is not instantiable.");
            }

            var processor = GetProcessorInstance(valueType);
            if (processor == null)
                return null;
            InjectDependencyToProcessor(processor);

            var baseValueType = processor.GetType().GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>))[0];
            if (baseValueType != valueType)
            {
                if (!valueType.IsDerivedFrom(baseValueType))
                {
                    throw new InvalidOperationException(
                        $"Type '{valueType.FullName}' is not derived from '{baseValueType.FullName}'.");
                }
                var processorWrapperType = typeof(SerializationProcessorWrapper<,>).MakeGenericType(valueType, baseValueType);
                return processorWrapperType.CreateInstance<ISerializationProcessor>(processor);
            }
            return processor;
        }

        private void InjectDependencyToProcessor([NotNull] ISerializationProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            //TODO: circular dependency processor
            foreach (var memberInfo in processor.GetType().GetMembers(MemberAccessFlags.All))
            {
                if (memberInfo.GetCustomAttribute<DependencyProcessorAttribute>() != null)
                {
                    var memberType = memberInfo.GetMemberType();
                    if (!memberType.IsDerivedFromGenericDefinition(typeof(ISerializationProcessor<>)))
                    {
                        throw new InvalidOperationException(
                            $"Member '{memberInfo.Name}' of type '{memberType.FullName}' is not a ISerializationProcessor<T>.");
                    }
                    var valueType = memberType.GetGenericArgumentsRelativeTo(typeof(ISerializationProcessor<>))[0];

                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        var setter = ReflectionUtility.CreateInstanceFieldSetter(fieldInfo)
                            .AsTyped<ISerializationProcessor, ISerializationProcessor>();
                        var dependency = GetProcessor(valueType);
                        setter(ref processor, dependency);
                    }
                    else if (memberInfo is PropertyInfo propertyInfo)
                    {
                        var setter = ReflectionUtility.CreateInstancePropertySetter(propertyInfo)
                            .AsTyped<ISerializationProcessor, ISerializationProcessor>();
                        var dependency = GetProcessor(valueType);
                        setter(ref processor, dependency);
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
        private ISerializationProcessor GetProcessorInstance(Type valueType)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                _typeMatcher.GetMatches(Type.EmptyTypes),
                _typeMatcher.GetMatches(valueType)
            };
            var results = _typeMatcher.GetMergedResults(resultsList);
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
