namespace EasyToolKit.Serialization
{
    public interface ISerializationArrayNode : ISerializationNode
    {
        /// <summary>
        /// Gets the array rank (dimension count).
        /// </summary>
        int Rank { get; }
    }
}
