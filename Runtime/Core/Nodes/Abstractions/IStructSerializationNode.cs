using System.Collections.Generic;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Represents a structure node with members.
    /// </summary>
    public interface IStructSerializationNode : ISerializationNode
    {
        /// <summary>
        /// Gets the members of this structure node. Lazy-initialized on first access.
        /// </summary>
        IReadOnlyList<ISerializationNode> Members { get; }

        /// <summary>
        /// Tries to get a member by name.
        /// </summary>
        bool TryGetMember(string name, out ISerializationNode member);
    }
}
