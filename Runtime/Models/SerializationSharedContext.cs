using System;
using EasyToolKit.Core.Patterns;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Shared context for serialization operations with dependency injection support.
    /// Provides access to shared services such as member info accessors during serialization.
    /// </summary>
    public sealed class SerializationSharedContext : IDisposable
    {
        private readonly IServiceContainer _serviceContainer;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance with the specified service container.
        /// </summary>
        /// <param name="serviceContainer">The service container to use.</param>
        private SerializationSharedContext(IServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));
        }

        /// <summary>
        /// Gets the default service descriptors for serialization.
        /// </summary>
        /// <returns>Collection of default service descriptors.</returns>
        public static ServiceDescriptor[] GetDefaultServiceDescriptors()
        {
            return new ServiceDescriptor[]
            {
                ServiceDescriptor.Singleton<ISerializedMemberInfoAccessor, SerializedMemberInfoAccessor>()
            };
        }

        /// <summary>
        /// Creates a default service container with the specified member filter.
        /// </summary>
        /// <param name="memberFilter">The member filter to use for serialization.</param>
        /// <returns>A configured service container.</returns>
        public static IServiceContainer CreateDefaultServiceContainer(MemberFilter memberFilter)
        {
            var descriptors = GetDefaultServiceDescriptors();
            var customDescriptor = ServiceDescriptor.Singleton<ISerializedMemberInfoAccessor>(
                provider => new SerializedMemberInfoAccessor(memberFilter));

            return ServiceContainerBuilder.Build(customDescriptor);
        }

        /// <summary>
        /// Creates a shared context with the specified member filter.
        /// </summary>
        /// <param name="memberFilter">The member filter to use.</param>
        /// <returns>A new shared context instance.</returns>
        public static SerializationSharedContext Create(MemberFilter memberFilter)
        {
            var container = CreateDefaultServiceContainer(memberFilter);
            return new SerializationSharedContext(container);
        }

        /// <summary>
        /// Creates a shared context with a custom service container.
        /// </summary>
        /// <param name="serviceContainer">The custom service container.</param>
        /// <returns>A new shared context instance.</returns>
        public static SerializationSharedContext Create(IServiceContainer serviceContainer)
        {
            return new SerializationSharedContext(serviceContainer);
        }

        /// <summary>
        /// Gets a service of the specified type from the container.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <returns>The service instance, or null if not found.</returns>
        public T GetService<T>() where T : class
        {
            ThrowIfDisposed();
            return _serviceContainer.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Gets a service of the specified type from the container.
        /// </summary>
        /// <param name="serviceType">The type of service to retrieve.</param>
        /// <returns>The service instance, or null if not found.</returns>
        public object GetService(Type serviceType)
        {
            ThrowIfDisposed();
            return _serviceContainer.GetService(serviceType);
        }

        /// <summary>
        /// Releases all resources used by the shared context.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _serviceContainer?.Dispose();
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SerializationSharedContext));
        }
    }
}
