using System;
using System.Linq;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization
{
    public static class SerializationProcessorUtility
    {
        private static Type[] s_processorTypes;

        public static Type[] ProcessorTypes
        {
            get
            {
                if (s_processorTypes == null)
                {
                    s_processorTypes = AssemblyUtility.GetTypes(AssemblyCategory.Custom)
                        .Where(type => type.IsClass && !type.IsInterface && !type.IsAbstract &&
                                       type.IsDerivedFrom<ISerializationProcessor>())
                        .ToArray();
                }
                return s_processorTypes;
            }
        }
    }
}
