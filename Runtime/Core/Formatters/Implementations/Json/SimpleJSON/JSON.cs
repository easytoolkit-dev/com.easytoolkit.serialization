namespace EasyToolkit.Serialization.Formatters.Implementations
{
    internal static class JSON
    {
        public static JSONNode Parse(string aJSON)
        {
            return JSONNode.Parse(aJSON);
        }
    }
}
