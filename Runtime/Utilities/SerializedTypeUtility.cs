using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Utilities
{
    public static class SerializedTypeUtility
    {
        public static EasySerializableAttribute GetDefinedEasySerializableAttribute(Type valueType)
        {
            var currentType = valueType;
            do
            {
                var attribute = currentType.GetCustomAttribute<EasySerializableAttribute>();
                if (attribute != null)
                {
                    if (currentType != valueType)
                    {
                        if (attribute.AllocInherit)
                        {
                            return attribute;
                        }

                        return null;
                    }

                    return attribute;
                }

                currentType = currentType.BaseType;
            } while (currentType != null);

            return null;
        }

        public static string TypeToName(Type type)
        {
            return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
        }

        public static Type NameToType(string typeName)
        {
            return TypeUtility.FindType(typeName);
        }
    }
}
