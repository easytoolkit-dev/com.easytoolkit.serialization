using System;
using EasyToolKit.Core.Patterns;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Settings container for EasySerializer system.
    /// Provides configuration for member filtering and serialization behavior.
    /// </summary>
    public sealed class EasySerializeSettings : IDisposable
    {
        private readonly IMemberFilterConfiguration _memberFilterConfiguration;
        private readonly SerializationSharedContext _sharedContext;
        private bool _disposed;

        /// <summary>
        /// Gets the member filter configuration.
        /// </summary>
        public IMemberFilterConfiguration MemberFilterConfiguration => _memberFilterConfiguration;

        /// <summary>
        /// Gets the member filter delegate for compatibility with existing code.
        /// </summary>
        public MemberFilter MemberFilter => _memberFilterConfiguration.CreateFilter();

        /// <summary>
        /// Gets the serialized member info accessor (internal use).
        /// </summary>
        internal ISerializedMemberInfoAccessor SerializedMemberInfoAccessor => _sharedContext.GetService<ISerializedMemberInfoAccessor>();

        /// <summary>
        /// Gets the shared serialization context for accessing services.
        /// </summary>
        public SerializationSharedContext SharedContext => _sharedContext;

        /// <summary>
        /// Initializes a new instance with default configuration.
        /// </summary>
        public EasySerializeSettings()
            : this(CreateDefaultConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified member filter configuration.
        /// </summary>
        public EasySerializeSettings(IMemberFilterConfiguration memberFilterConfiguration)
        {
            _memberFilterConfiguration = memberFilterConfiguration ?? throw new ArgumentNullException(nameof(memberFilterConfiguration));
            _memberFilterConfiguration.ValidateOrThrow();
            _sharedContext = SerializationSharedContext.Create(MemberFilter);
        }

        /// <summary>
        /// Initializes a new instance with a preset configuration by name.
        /// </summary>
        public EasySerializeSettings(string presetName)
        {
            if (!MemberFilterPresetRegistry.Instance.TryGetPreset(presetName, out var config))
            {
                throw new ArgumentException($"Preset '{presetName}' not found.", nameof(presetName));
            }

            _memberFilterConfiguration = config;
            _sharedContext = SerializationSharedContext.Create(MemberFilter);
        }

        /// <summary>
        /// Creates settings from a configuration builder.
        /// </summary>
        public static EasySerializeSettings FromBuilder(MemberFilterConfigurationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var config = builder.BuildAndValidate();
            return new EasySerializeSettings(config);
        }

        /// <summary>
        /// Creates settings with a custom service container.
        /// </summary>
        /// <param name="memberFilterConfiguration">The member filter configuration.</param>
        /// <param name="serviceContainer">The custom service container.</param>
        /// <returns>A new settings instance with the custom container.</returns>
        public static EasySerializeSettings WithServiceContainer(
            IMemberFilterConfiguration memberFilterConfiguration,
            IServiceContainer serviceContainer)
        {
            if (memberFilterConfiguration == null)
                throw new ArgumentNullException(nameof(memberFilterConfiguration));
            if (serviceContainer == null)
                throw new ArgumentNullException(nameof(serviceContainer));

            memberFilterConfiguration.ValidateOrThrow();

            return new EasySerializeSettings(memberFilterConfiguration, serviceContainer);
        }

        /// <summary>
        /// Initializes a new instance with a custom service container.
        /// </summary>
        private EasySerializeSettings(IMemberFilterConfiguration memberFilterConfiguration, IServiceContainer serviceContainer)
        {
            _memberFilterConfiguration = memberFilterConfiguration;
            _sharedContext = SerializationSharedContext.Create(serviceContainer);
        }

        private static IMemberFilterConfiguration CreateDefaultConfiguration()
        {
            return MemberFilterPresetRegistry.Instance.GetPreset("Default");
        }

        /// <summary>
        /// Releases all resources used by the settings.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _sharedContext?.Dispose();
            _disposed = true;
        }
    }
}
