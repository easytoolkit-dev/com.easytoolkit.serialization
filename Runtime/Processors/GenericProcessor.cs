using System;
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
            _memberDefinitions = SerializationStructureResolverFactory.GetResolver(typeof(T)).Resolve(typeof(T), Context);
        }

        protected override void Process(ref T value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(ValueType);

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
                            $"The getter threw an exception of type '{ex.GetType().Name}'. " +
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
                    memberDefinition.Processor.ProcessUntyped(ref memberValue, formatter);
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
                                $"Cannot deserialize into type '{typeof(T)}' because the instance is null and cannot be automatically constructed. " +
                                $"The type must have an accessible parameterless constructor, or a constructor whose parameters can be filled with default values.");
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
                            $"The setter threw an exception of type '{ex.GetType().Name}'. " +
                            $"Check that the setter is not performing invalid operations during deserialization " +
                            $"or that the deserialized value is compatible with the member type.", ex);
                    }

                    value = (T)boxedValue;
                }
            }
        }
    }
}
