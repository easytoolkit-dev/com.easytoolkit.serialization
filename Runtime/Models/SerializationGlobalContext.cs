using System;
using EasyToolKit.Core.Patterns;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Global context for serialization operations with dependency injection support.
    /// Provides access to global services such as member info accessors during serialization.
    /// </summary>
    public sealed class SerializationGlobalContext : Singleton<SerializationGlobalContext>
    {
        private readonly IServiceContainer _serviceContainer;

        private SerializationGlobalContext()
        {
            var descriptors = GetDefaultServiceDescriptors();
            _serviceContainer = ServiceContainerBuilder.Build(descriptors);
        }

        /// <summary>
        /// Gets the default service descriptors for serialization.
        /// </summary>
        /// <returns>Collection of default service descriptors.</returns>
        private static ServiceDescriptor[] GetDefaultServiceDescriptors()
        {
            return new ServiceDescriptor[]
            {
                ServiceDescriptor.Singleton<
                    ISerializationNodeBuilder,
                    Implementations.SerializationNodeBuilder>(),

                ServiceDescriptor.Singleton<
                    ISerializationStructureResolverFactory,
                    Implementations.DefaultSerializationStructureResolverFactory>(),

                ServiceDescriptor.Singleton<
                    IFormatterFactory,
                    Implementations.FormatterFactory>(),
            };
        }

        /// <summary>
        /// Gets a service of the specified type from the container.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <returns>The service instance, or null if not found.</returns>
        public T GetService<T>() where T : class
        {
            return _serviceContainer.GetService(typeof(T)) as T;
        }
    }
}
