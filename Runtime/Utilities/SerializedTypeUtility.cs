using System;
using System.Reflection;

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
    }
}
