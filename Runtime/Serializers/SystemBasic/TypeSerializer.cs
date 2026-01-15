using System;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.SystemBasic)]
    public class TypeSerializer : EasySerializer<Type>
    {
        public override void Process(string name, ref Type value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);

            string typeName = null;
            var direction = formatter.Direction;

            if (direction == FormatterDirection.Output)
                typeName = TypeToName(value);

            formatter.Format(ref typeName);

            if (direction == FormatterDirection.Input)
                value = NameToType(typeName);

            formatter.EndMember();
        }

        private static string TypeToName(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        private static Type NameToType(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            return Type.GetType(name);
        }
    }
}
