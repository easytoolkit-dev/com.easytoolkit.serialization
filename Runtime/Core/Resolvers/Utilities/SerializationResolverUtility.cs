using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core.Reflection;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Utility class for discovering and matching serialization resolvers.
    /// Resolvers are discovered via reflection, sorted by priority obtained from <see cref="SerializationResolverPriorityAttribute"/>,
    /// and registered in a <see cref="s_typeMatcher"/> for type-based matching.
    /// </summary>
    public static class SerializationResolverUtility
    {
        private static Type[] s_resolverTypes;
        private static ITypeMatcher s_typeMatcher;
        private static bool s_typeMatcherInitialized;
        private static readonly object InitializationLock = new object();

        /// <summary>
        /// Gets the type matcher used for resolver type discovery.
        /// </summary>
        public static ITypeMatcher TypeMatcher
        {
            get
            {
                EnsureTypeMatcherInitialized();
                return s_typeMatcher;
            }
        }

        private static void EnsureTypeMatcherInitialized()
        {
            if (s_typeMatcherInitialized) return;
            lock (InitializationLock)
            {
                if (s_typeMatcherInitialized) return;
                InitializeTypeMatcher();
                s_typeMatcherInitialized = true;
            }
        }

        private static void InitializeTypeMatcher()
        {
            if (s_resolverTypes == null)
            {
                s_resolverTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsClass && !type.IsInterface && !type.IsAbstract &&
                                   type.IsInheritsFrom<ISerializationResolver>())
                    .ToArray();
            }

            s_typeMatcher = TypeMatcherFactory.CreateDefault();
            s_typeMatcher.SetTypeMatchCabdudates(s_resolverTypes
                .OrderByDescending(GetResolverPriority)
                .Select((type, i) =>
                {
                    Type[] constraints = null;
                    if (type.BaseType != null && type.BaseType.IsGenericType)
                    {
                        // For generic resolvers, extract the target generic type from the inheritance chain.
                        constraints = type.GetArgumentsOfInheritedGenericTypeDefinition(type.BaseType.GetGenericTypeDefinition());
                    }
                    return new TypeMatchCandidate(type, s_resolverTypes.Length - i, constraints);
                }));
        }

        /// <summary>
        /// Gets the resolver type for the specified target type.
        /// </summary>
        /// <param name="type">The target type to find a resolver for.</param>
        /// <returns>The first resolver type found that can resolve the target type, or null if none found.</returns>
        [CanBeNull]
        public static Type GetResolverType(Type type)
        {
            var results = TypeMatcher.GetMatches(Type.EmptyTypes);
            foreach (var result in results)
            {
                var resolverType = result.MatchedType;
                if (!CanResolveType(resolverType, type))
                {
                    continue;
                }

                return resolverType;
            }

            return null;
        }

        /// <summary>
        /// Creates a resolver instance for the specified target type.
        /// </summary>
        /// <param name="type">The target type to create a resolver for.</param>
        /// <returns>A new resolver instance, or null if no suitable resolver is found.</returns>
        [CanBeNull]
        public static ISerializationResolver CreateResolver(Type type)
        {
            var resolverType = GetResolverType(type);
            return resolverType != null
                ? Activator.CreateInstance(resolverType) as ISerializationResolver
                : null;
        }

        /// <summary>
        /// Gets the priority of a resolver type.
        /// Priority is obtained from the <see cref="SerializationResolverPriorityAttribute"/> if present.
        /// </summary>
        /// <param name="resolverType">The resolver type to examine.</param>
        /// <returns>The priority of the resolver.</returns>
        private static double GetResolverPriority(Type resolverType)
        {
            if (resolverType.GetCustomAttributes(true)
                    .FirstOrDefault(attr => attr is SerializationResolverPriorityAttribute) is SerializationResolverPriorityAttribute priorityAttribute)
            {
                return priorityAttribute.Priority;
            }

            return 0.0;
        }

        /// <summary>
        /// Determines whether the specified resolver type can resolve the given type.
        /// Creates an instance of the resolver type and calls its <see cref="ISerializationResolver.CanResolve(Type)"/> method.
        /// </summary>
        /// <param name="resolverType">The resolver type to test.</param>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the resolver can resolve the type; otherwise, false.</returns>
        private static bool CanResolveType(Type resolverType, Type type)
        {
            var resolver = (ISerializationResolver)FormatterServices.GetUninitializedObject(resolverType);
            return resolver.CanResolve(type);
        }
    }
}
