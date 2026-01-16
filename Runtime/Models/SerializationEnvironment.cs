using System;
using EasyToolKit.Core.Patterns;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Environment for serialization operations with dependency injection support.
    /// </summary>
    public sealed class SerializationEnvironment : Singleton<SerializationEnvironment>
    {
        private readonly IServiceContainer _serviceContainer;

        private SerializationEnvironment()
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

                ServiceDescriptor.Singleton<
                    ISerializerFactory,
                    Implementations.SerializerFactory>(),
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
