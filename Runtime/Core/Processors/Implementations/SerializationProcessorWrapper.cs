using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors.Implementations
{
    public class SerializationProcessorWrapper<T, TBase> : SerializationProcessor<T>
        where T : TBase
    {
        private readonly ISerializationProcessor<TBase> _baseProcessor;

        public SerializationProcessorWrapper(ISerializationProcessor<TBase> baseProcessor)
        {
            _baseProcessor = baseProcessor;
        }

        protected override void Process(ref T value, IDataFormatter formatter)
        {
            var casted = (TBase)value;
            _baseProcessor.Process(ref casted, formatter);
            value = (T)casted;
        }
    }
}
