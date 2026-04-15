using System;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Resolvers;
using UnityEngine.Assertions;

namespace EasyToolkit.Serialization.Processors
{
    public partial class GenericProcessor<T>
    {
        private void ReadMember(ref T instance, SerializationMemberDefinition memberDefinition, IReadingFormatter formatter)
        {
            formatter.BeginMember(memberDefinition.Name);

            var isNull = false;
            if (!typeof(T).IsValueType)
            {
                formatter.FormatNullable(ref isNull);
            }

            object memberValue = null;
            if (!isNull)
            {
                if (memberDefinition.UseRuntimeType && memberDefinition.Processor == null)
                {
                    var valueType = formatter.PeekType(memberDefinition.MemberType);
                    if (valueType == null)
                    {
                        throw new SerializationException(
                            $"Cannot read type information during deserialization. "
                            + $"The formatter cannot retrieve the runtime type for member '{memberDefinition.Name}' (declared type: {memberDefinition.MemberType.ToCodeString()}). "
                            + $"Ensure that '{formatter.FormatType}FormatterOptions.{formatter.FormatType}FormatterOptions.IncludeObjectType' is enabled in the formatter options to include type metadata in the serialized data.");
                    }

                    Assert.AreNotEqual(valueType, memberDefinition.MemberType);
                    var processor = _processorByType.GetOrAdd(valueType, CreateProcessor);
                    processor.ProcessUntyped(ref memberValue, formatter);
                }
                else
                {
                    memberDefinition.Processor.ProcessUntyped(ref memberValue, formatter);
                }
            }

            var setter = memberDefinition.ValueSetter;
            if (setter == null)
            {
                throw new SerializationException(
                    $"Cannot deserialize member '{memberDefinition.Name}' on type '{typeof(T)}'. "
                    + $"The member does not have a writable setter. Ensure the member is either a field or a property with a set accessor.");
            }

            if (instance == null)
            {
                if (ConstructorInvoker == null)
                {
                    throw new SerializationException(
                        $"Cannot deserialize into type '{typeof(T)}' because the instance is null and cannot be automatically constructed. "
                        + $"The type must have an accessible parameterless constructor, or a constructor whose parameters can be filled with default values.");
                }

                instance = ConstructorInvoker();
            }

            object boxedInstance = instance;
            try
            {
                setter(ref boxedInstance, memberValue);
            }
            catch (Exception ex)
            {
                throw new SerializationException(
                    $"Failed to set value on member '{memberDefinition.Name}' on type '{typeof(T)}'. "
                    + $"The setter threw an exception of type '{ex.GetType()}'. "
                    + $"Check that the setter is not performing invalid operations during deserialization "
                    + $"or that the deserialized value is compatible with the member type.", ex);
            }

            instance = (T)boxedInstance;
        }
    }
}
