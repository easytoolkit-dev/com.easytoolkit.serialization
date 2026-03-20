using System;
using System.Collections.Concurrent;
using EasyToolkit.Core.Diagnostics;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Resolvers;
using EasyToolkit.Serialization.Utilities;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Generic)]
    public partial class GenericProcessor<T> : SerializationProcessor<T>
    {
        private static readonly bool IsClassType = typeof(T).IsClass && typeof(T) != typeof(string);
        private static readonly bool IsInstantiableType = typeof(T).IsInstantiable(allowLenient: true);
        [CanBeNull] private static readonly ParameterlessConstructorInvoker<T> ConstructorInvoker;

        static GenericProcessor()
        {
            if (IsClassType && IsInstantiableType)
            {
                foreach (var constructorInfo in typeof(T).GetConstructors(MemberAccessFlags.AllInstance))
                {
                    try
                    {
                        ConstructorInvoker = ReflectionCompiler.CreateParameterlessConstructorInvoker<T>(
                            constructorInfo, autoFillParameters: true
                        );
                        break;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        private SerializationMemberDefinition[] _memberDefinitions;
        private readonly ConcurrentDictionary<Type, ISerializationProcessor> _processorByType = new();
        private bool _isNoSerializableType;

        public override bool CanProcess(Type valueType, SerializationContext context)
        {
            if (SerializationStructureResolverFactory.GetResolver(valueType) == null)
            {
                return false;
            }

            if (Nullable.GetUnderlyingType(valueType) != null)
            {
                return false;
            }

            if (valueType.IsAnonymousType() && context.AllowAnonymousTypes)
            {
                return true;
            }

            if (!valueType.IsDefined<SerializableAttribute>()
                && SerializedTypeUtility.GetDefinedEasySerializableAttribute(valueType) == null)
            {
                if (context.AllowUnmarkedStructs && valueType.IsStructType())
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        protected override void Initialize()
        {
            _memberDefinitions = ResolverMemberDefinitions(typeof(T));

            if (!typeof(T).IsValueType && !ValueType.IsAnonymousType())
            {
                var attribute = SerializedTypeUtility.GetDefinedEasySerializableAttribute(typeof(T));
                if (attribute is { Ignore: true })
                {
                    _isNoSerializableType = true;
                }
                else if (typeof(T).IsDefined<SerializableAttribute>())
                {
                    _isNoSerializableType = false;
                }
                else if (attribute == null)
                {
                    _isNoSerializableType = true;
                }
            }
        }

        protected override void Process(ref T value, IDataFormatter formatter)
        {
            var valueType = typeof(T);

            if (IsProcessByRuntime(ref value, formatter))
            {
                return;
            }

            using var scope = formatter.EnterObject(valueType);

            if (_isNoSerializableType)
            {
                return;
            }

            var readingFormatter = formatter as IReadingFormatter;
            var writingFormatter = formatter as IWritingFormatter;
            foreach (var memberDefinition in _memberDefinitions)
            {
                if (readingFormatter != null)
                {
                    ReadMember(ref value, memberDefinition, readingFormatter);
                }
                else
                {
                    WriteMember(ref value, memberDefinition, writingFormatter);
                }
            }
        }

        private ISerializationProcessor CreateProcessor(Type valueType)
        {
            return SerializationProcessorFactory.CreateProcessor(valueType, Context, this);
        }

        private bool IsProcessByRuntime(ref T value, IDataFormatter formatter)
        {
            if (formatter is IReadingFormatter readingFormatter)
            {
                var valueType = readingFormatter.PeekType(typeof(T));
                if (valueType != null && valueType != typeof(T))
                {
                    object boxedValue = null;
                    ProcessRuntimeValue(valueType, ref boxedValue);
                    value = (T)boxedValue;
                    return true;
                }
            }
            else
            {
                if (value != null)
                {
                    var valueType = value.GetType();
                    if (valueType != typeof(T))
                    {
                        object boxedValue = value;
                        ProcessRuntimeValue(valueType, ref boxedValue);
                        return true;
                    }
                }
            }

            return false;

            void ProcessRuntimeValue(Type valueType, ref object value)
            {
                var processor = _processorByType.GetOrAdd(valueType, CreateProcessor);
                processor.Parent = Parent;
                processor.ProcessUntyped(ref value, formatter);
                processor.Parent = this;
            }
        }

        private SerializationMemberDefinition[] ResolverMemberDefinitions(Type valueType)
        {
            return SerializationStructureResolverFactory.GetResolver(valueType).Resolve(valueType, Context, this);
        }
    }
}
