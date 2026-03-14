using System;
using System.Collections.Concurrent;
using EasyToolkit.Serialization.Processors;

namespace EasyToolkit.Serialization
{
    /// <summary>
    /// Serialization context containing reflection settings and processor cache.
    /// </summary>
    /// <remarks>
    /// The context allows runtime configuration of serialization behavior, including
    /// which members are serialized and how processors are cached. Each context instance
    /// maintains its own processor cache to ensure isolation.
    /// </remarks>
    public sealed class SerializationContext
    {
        /// <summary>
        /// Global shared context using default reflection settings and independent cache.
        /// </summary>
        /// <remarks>
        /// This is the default context used when no context is explicitly provided
        /// to serialization methods. It provides default behavior and maintains
        /// its own cache for performance.
        /// </remarks>
        public static readonly SerializationContext Shared = new();

        private SerializableMemberFlags _memberFlags = SerializableMemberFlags.Default;

        /// <summary>
        /// Gets or sets the flags that control which members are filtered for serialization.
        /// </summary>
        /// <remarks>
        /// This setting controls which types of members (fields/properties, public/non-public)
        /// are included in serialization by default when a type does not explicitly
        /// specify these settings via <see cref="EasySerializableAttribute"/>.
        /// Setting this property clears the processor cache to ensure new settings take effect.
        /// </remarks>
        public SerializableMemberFlags MemberFlags
        {
            get => _memberFlags;
            set
            {
                _memberFlags = value;
                _processorCache.Clear();
            }
        }

        private bool _requireSerializeFieldOnNonPublic = true;

        /// <summary>
        /// Gets or sets whether non-public fields must have the <c>SerializeField</c> attribute
        /// to be serialized.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, non-public fields are only serialized if they are explicitly marked
        /// with <c>UnityEngine.SerializeField</c>. This mimics Unity's serialization behavior.
        /// When <c>false</c>, non-public fields are serialized based on <see cref="MemberFlags"/> alone.
        /// Setting this property clears the processor cache to ensure new settings take effect.
        /// </remarks>
        public bool RequireSerializeFieldOnNonPublic
        {
            get => _requireSerializeFieldOnNonPublic;
            set
            {
                _requireSerializeFieldOnNonPublic = value;
                _processorCache.Clear();
            }
        }

        private bool _excludeNonSerialized = true;

        /// <summary>
        /// Gets or sets whether to exclude members marked with <c>NonSerializedAttribute</c>
        /// from serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, fields marked with <c>System.NonSerializedAttribute</c> are excluded
        /// from serialization regardless of other settings. When <c>false</c>, the <c>NonSerialized</c>
        /// attribute is ignored and members are serialized based on other flags.
        /// Setting this property clears the processor cache to ensure new settings take effect.
        /// </remarks>
        public bool ExcludeNonSerialized
        {
            get => _excludeNonSerialized;
            set
            {
                _excludeNonSerialized = value;
                _processorCache.Clear();
            }
        }

        private readonly ConcurrentDictionary<Type, ISerializationProcessor> _processorCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext"/> class.
        /// </summary>
        public SerializationContext()
        {
            _processorCache = new ConcurrentDictionary<Type, ISerializationProcessor>();
        }

        /// <summary>
        /// Gets or creates a processor for the specified type using the provided factory function.
        /// </summary>
        /// <param name="type">The type to get a processor for.</param>
        /// <param name="factory">The factory function to create a new processor if not cached.</param>
        /// <returns>The cached or newly created processor.</returns>
        /// <remarks>
        /// This method is internal to the serialization system and is used by
        /// <see cref="Processors.SerializationProcessorFactory"/> to ensure processors
        /// are cached per context instance.
        /// </remarks>
        internal ISerializationProcessor GetProcessor(Type type, Func<Type, ISerializationProcessor> factory)
        {
            return _processorCache.GetOrAdd(type, factory);
        }
    }
}
