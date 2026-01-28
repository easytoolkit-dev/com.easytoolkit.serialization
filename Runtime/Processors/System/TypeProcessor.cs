using System;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.System)]
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
