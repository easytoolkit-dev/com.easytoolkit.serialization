using System;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic)]
    public class TypeProcessor : SerializationProcessor<Type>
    {
        protected override void Process(string name, ref Type value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);

            string typeName = null;
            if (formatter.Operation == FormatterOperation.Write)
                typeName = TypeToName(value);

            formatter.Format(ref typeName);

            if (formatter.Operation == FormatterOperation.Read)
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
