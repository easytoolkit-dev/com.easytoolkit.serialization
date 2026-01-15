using System;

namespace EasyToolKit.Serialization
{
    public interface IEasySerializer
    {
        bool CanSerialize(Type valueType);
        bool IsRoot { get; set; }
        Type ValueType { get; }

        void Process(ref object value, IDataFormatter formatter);
        void Process(string name, ref object value, IDataFormatter formatter);
    }

    public abstract class EasySerializer<T> : IEasySerializer
    {
        bool IEasySerializer.CanSerialize(Type valueType)
        {
            return CanSerialize(valueType);
        }

        bool IEasySerializer.IsRoot
        {
            get => IsRoot;
            set => IsRoot = value;
        }

        public Type ValueType => typeof(T);

        void IEasySerializer.Process(ref object value, IDataFormatter formatter)
        {
            ProcessImpl(null, ref value, formatter);
        }

        void IEasySerializer.Process(string name, ref object value, IDataFormatter formatter)
        {
            ProcessImpl(name, ref value, formatter);
        }

        private void ProcessImpl(string name, ref object value, IDataFormatter formatter)
        {
            T val = default;

            var direction = formatter.Direction;
            if (direction == FormatterDirection.Output)
            {
                val = (T)value;
            }

            Process(name, ref val, formatter);

            if (direction == FormatterDirection.Input)
            {
                value = val;
            }
        }

        protected bool IsRoot { get; private set; }

        protected EasySerializeSettings Settings => EasySerialize.CurrentSettings;

        /// <summary>
        /// Gets the shared serialization context for accessing services.
        /// </summary>
        protected SerializationSharedContext Context => Settings.SharedContext;

        public virtual bool CanSerialize(Type valueType) => true;

        public void Process(ref T value, IDataFormatter formatter)
        {
            Process(null, ref value, formatter);
        }

        public abstract void Process(string name, ref T value, IDataFormatter formatter);

        public static EasySerializer<E> GetSerializer<E>() => EasySerializerUtility.GetSerializer<E>();
    }
}
