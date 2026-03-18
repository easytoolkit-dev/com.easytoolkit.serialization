using System;
using System.Collections.Concurrent;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Resolvers;
using EasyToolkit.Serialization.Utilities;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Generic)]
    public class GenericProcessor<T> : SerializationProcessor<T>
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
        }

        protected override void Process(ref T value, IDataFormatter formatter)
        {
            var valueType = typeof(T);

            if (Parent?.UseRuntimeTypeSerialization == true)
            {
                if (formatter is IReadingFormatter readingFormatter)
                {
                    valueType = readingFormatter.PeekType(expectedType: valueType);
                    if (valueType != null && valueType != ValueType)
                    {
                        var processor = _processorByType.GetOrAdd(valueType, CreateProcessor);
                        object boxedValue = null;
                        processor.Parent = Parent;
                        processor.ProcessUntyped(ref boxedValue, formatter);
                        processor.Parent = this;
                        value = (T)boxedValue;
                        return;
                    }

                    valueType ??= typeof(T);
                }
                else
                {
                    if (value != null)
                    {
                        valueType = value.GetType();
                    }
                }
            }

            using var scope = formatter.EnterObject(valueType);

            foreach (var memberDefinition in _memberDefinitions)
            {
                object memberValue = null;

                if (formatter.Operation == FormatterOperation.Write)
                {
                    var getter = memberDefinition.ValueGetter;
                    if (getter == null)
                    {
                        throw new SerializationException(
                            $"Cannot serialize member '{memberDefinition.Name}' on type '{typeof(T)}'. " +
                            $"The member does not have a readable getter. Ensure the member is either a field or a property with a get accessor.");
                    }

                    try
                    {
                        memberValue = getter(value);
                    }
                    catch (Exception ex)
                    {
                        throw new SerializationException(
                            $"Failed to get value from member '{memberDefinition.Name}' on type '{typeof(T)}'. " +
                            $"The getter threw an exception of type '{ex.GetType()}'. " +
                            $"Check that the getter is not performing invalid operations during serialization.", ex);
                    }
                }

                formatter.BeginMember(memberDefinition.Name);
                var isNull = memberValue == null;
                if (!typeof(T).IsValueType)
                {
                    formatter.FormatNullable(ref isNull);
                }
                else
                {
                    isNull = false;
                }

                if (!isNull)
                {
                    // Use runtime type processor if enabled and value is not null
                    if (memberDefinition.UseRuntimeType && memberValue != null)
                    {
                        var runtimeType = memberValue.GetType();
                        // Only use runtime type if it differs from declared type
                        if (runtimeType != memberDefinition.MemberType)
                        {
                            // Check if runtime type is allowed based on member-level settings
                            if (runtimeType.IsAnonymousType() && !memberDefinition.AllowAnonymousTypes)
                            {
                                throw new SerializationException(
                                    $"Cannot serialize member '{memberDefinition.Name}' with runtime anonymous type '{runtimeType}'. " +
                                    $"Anonymous types are not allowed for this member. " +
                                    $"Enable AllowAnonymousTypes in EasySerializableAttribute or SerializationContext.");
                            }

                            if (!runtimeType.IsDefined<SerializableAttribute>() &&
                                SerializedTypeUtility.GetDefinedEasySerializableAttribute(runtimeType) == null)
                            {
                                if (runtimeType.IsStructType() && !memberDefinition.AllowUnmarkedStructs)
                                {
                                    throw new SerializationException(
                                        $"Cannot serialize member '{memberDefinition.Name}' with runtime struct type '{runtimeType}'. " +
                                        $"Structs must be marked with [Serializable] or [EasySerializable]. " +
                                        $"Enable AllowUnmarkedStructs in EasySerializableAttribute or SerializationContext.");
                                }
                            }

                            var runtimeProcessor = _processorByType.GetOrAdd(runtimeType, CreateProcessor);
                            if (runtimeProcessor == null)
                            {
                                throw new SerializationException(
                                    $"Cannot serialize member '{memberDefinition.Name}' with runtime type '{runtimeType}'. " +
                                    $"No suitable processor was found for this type. " +
                                    $"Ensure the runtime type is marked with [Serializable] or [EasySerializable], " +
                                    $"or disable UseRuntimeType in SerializationContext or EasySerializableAttribute.");
                            }
                            runtimeProcessor.ProcessUntyped(ref memberValue, formatter);
                        }
                        else
                        {
                            memberDefinition.Processor.ProcessUntyped(ref memberValue, formatter);
                        }
                    }
                    else
                    {
                        memberDefinition.Processor.ProcessUntyped(ref memberValue, formatter);
                    }
                }
                else
                {
                    memberValue = null;
                }

                if (formatter.Operation == FormatterOperation.Read)
                {
                    var setter = memberDefinition.ValueSetter;
                    if (setter == null)
                    {
                        throw new SerializationException(
                            $"Cannot deserialize member '{memberDefinition.Name}' on type '{typeof(T)}'. " +
                            $"The member does not have a writable setter. Ensure the member is either a field or a property with a set accessor.");
                    }

                    if (value == null)
                    {
                        if (ConstructorInvoker == null)
                        {
                            throw new SerializationException(
                                $"Cannot deserialize into type '{typeof(T)}' because the instance is null and cannot be automatically constructed. "
                                + $"The type must have an accessible parameterless constructor, or a constructor whose parameters can be filled with default values.");
                        }

                        value = ConstructorInvoker();
                    }

                    object boxedValue = value;
                    try
                    {
                        setter(ref boxedValue, memberValue);
                    }
                    catch (Exception ex)
                    {
                        throw new SerializationException(
                            $"Failed to set value on member '{memberDefinition.Name}' on type '{typeof(T)}'. " +
                            $"The setter threw an exception of type '{ex.GetType()}'. " +
                            $"Check that the setter is not performing invalid operations during deserialization " +
                            $"or that the deserialized value is compatible with the member type.", ex);
                    }

                    value = (T)boxedValue;
                }
            }
        }

        private ISerializationProcessor CreateProcessor(Type valueType)
        {
            return SerializationProcessorFactory.CreateProcessor(valueType, Context, this);
        }

        private SerializationMemberDefinition[] ResolverMemberDefinitions(Type valueType)
        {
            return SerializationStructureResolverFactory.GetResolver(valueType).Resolve(valueType, Context, this);
        }
    }
}
