using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    public sealed class SerializationProcessorFactory : ISerializationProcessorFactory
    {
        private readonly ITypeMatcher _serializerTypeMatcher;

        public SerializationProcessorFactory()
        {
            _serializerTypeMatcher = TypeMatcherFactory.CreateDefault();
            InitializeTypeMatcher();
        }

        private void InitializeTypeMatcher()
        {
            _serializerTypeMatcher.SetTypeMatchIndices(SerializationProcessorUtility.ProcessorTypes.Select(type =>
            {
                var config = type.GetCustomAttribute<SerializerConfigurationAttribute>();
                config ??= SerializerConfigurationAttribute.Default;

                var argType = type.GetArgumentsOfInheritedOpenGenericType(typeof(ISerializationProcessor<>));
                return new TypeMatchIndex(type, config.Priority, argType);
            }));
        }

        public ISerializationProcessor<T> GetSerializer<T>()
        {
            return (ISerializationProcessor<T>)GetSerializer(typeof(T));
        }

        private ISerializationProcessor GetSerializer(Type valueType)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                _serializerTypeMatcher.GetMatches(Type.EmptyTypes),
                _serializerTypeMatcher.GetMatches(valueType)
            };
            var results = _serializerTypeMatcher.GetMergedResults(resultsList);
            foreach (var result in results)
            {
                if (CanSerializeType(result.MatchedType, valueType))
                {
                    return result.MatchedType.CreateInstance<ISerializationProcessor>();
                }
            }

            return null;
        }

        private static bool CanSerializeType(Type serializerType, Type valueType)
        {
            var serializer = (ISerializationProcessor)FormatterServices.GetUninitializedObject(serializerType);
            return serializer.CanSerialize(valueType);
        }
    }
}
