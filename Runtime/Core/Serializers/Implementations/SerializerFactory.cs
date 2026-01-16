using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Factory for creating and retrieving serializers.
    /// </summary>
    internal sealed class SerializerFactory : ISerializerFactory
    {
        private readonly ITypeMatcher _serializerTypeMatcher;

        public SerializerFactory()
        {
            _serializerTypeMatcher = TypeMatcherFactory.CreateDefault();
            InitializeTypeMatcher();
        }

        private void InitializeTypeMatcher()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsInterface && !type.IsAbstract &&
                            type.IsInheritsFrom<IEasySerializer>())
                .ToArray();

            _serializerTypeMatcher.SetTypeMatchIndices(types.Select(type =>
            {
                var config = type.GetCustomAttribute<SerializerConfigurationAttribute>();
                config ??= SerializerConfigurationAttribute.Default;

                var argType = type.GetArgumentsOfInheritedOpenGenericType(typeof(IEasySerializer<>));
                return new TypeMatchIndex(type, config.Priority, argType);
            }));
        }

        public IEasySerializer<T> GetSerializer<T>()
        {
            return (IEasySerializer<T>)GetSerializer(typeof(T));
        }

        private IEasySerializer GetSerializer(Type valueType)
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
                    return result.MatchedType.CreateInstance<IEasySerializer>();
                }
            }

            return null;
        }

        private static bool CanSerializeType(Type serializerType, Type valueType)
        {
            var serializer = (IEasySerializer)FormatterServices.GetUninitializedObject(serializerType);
            return serializer.CanSerialize(valueType);
        }
    }
}
