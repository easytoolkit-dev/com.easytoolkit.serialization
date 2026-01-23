using System;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic)]
    public class TypeProcessor : SerializationProcessor<Type>
    {
        public override void Process(string name, ref Type value, IDataFormatter formatter)
        {
            using var memberScope = formatter.EnterMember(name);

            string typeName = null;
            if (formatter.Operation == FormatterOperation.Write)
                typeName = TypeToName(value);

            formatter.Format(ref typeName);

            if (formatter.Operation == FormatterOperation.Read)
                value = NameToType(typeName);
        }

        private static string TypeToName(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            return type.AssemblyQualifiedName ?? type.FullName;
        }

        private static Type NameToType(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            return TypeUtility.FindType(name);
        }
    }
}
