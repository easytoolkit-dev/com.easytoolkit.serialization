namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Size prefix tag for collections and arrays.
    /// </summary>
    public struct SizeTag
    {
        /// <summary>The size of the collection.</summary>
        public uint Size { get; set; }

        /// <summary>Creates a new SizeTag with the specified size.</summary>
        public SizeTag(uint size)
        {
            Size = size;
        }
    }
}
