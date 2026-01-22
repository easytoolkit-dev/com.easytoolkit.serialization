namespace EasyToolKit.Serialization.Implementations
{
    public class SerializationProcessorWrapper<T, TBase> : SerializationProcessor<T>
        where T : TBase
    {
        private readonly ISerializationProcessor<TBase> _baseProcessor;

        public SerializationProcessorWrapper(ISerializationProcessor<TBase> baseProcessor)
        {
            _baseProcessor = baseProcessor;
        }

        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            var casted = (TBase)value;
            _baseProcessor.Process(name, ref casted, formatter);
            value = (T)casted;
        }
    }
}
