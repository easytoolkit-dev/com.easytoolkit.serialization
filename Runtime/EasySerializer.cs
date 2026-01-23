using System;
using System.Collections.Generic;
using System.IO;

namespace EasyToolKit.Serialization
{
    public static class EasySerializer
    {
        public static void Serialize<T>(ref T value, SerializationFormat format, ref SerializationData serializationData)
        {
            var untypedValue = (object)value;
            Serialize(untypedValue, format, ref serializationData);
            value = (T)untypedValue;
        }

        public static void Serialize(object value, SerializationFormat format, ref SerializationData serializationData)
        {
            if (value == null)
            {
                serializationData.Clear();
                return;
            }

            var valueType = value.GetType();
            // Build node tree first
            using var stream = new MemoryStream();
            using var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateWriter(format, stream);
            if (formatter == null)
            {
                throw new SerializationException(
                    $"Failed to create writer for format '{format}'. The format may not be supported.");
            }

            var processor = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetProcessor(valueType);
            if (processor == null)
            {
                throw new SerializationException(
                    $"Cannot serialize type '{valueType}'. No serialization processor found for this type. " +
                    $"Ensure the type is either a primitive type, a collection, or marked with [Serializable] or [EasySerializable].");
            }

            processor.ProcessUntyped(ref value, formatter);

            var objectTable = formatter.GetObjectTable();
            if (objectTable.Count > 0)
            {
                serializationData.ReferencedUnityObjects = new List<UnityEngine.Object>(objectTable);
            }
            else
            {
                serializationData.ReferencedUnityObjects = null;
            }
            serializationData.SetBuffer(format, stream.ToArray());
        }

        public static byte[] SerializeToBinary(object value)
        {
            List<UnityEngine.Object> referencedUnityObjects = null;
            return SerializeToBinary(value, ref referencedUnityObjects);
        }

        public static byte[] SerializeToBinary(object value, ref List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(Array.Empty<byte>(), referencedUnityObjects);
            Serialize(value, SerializationFormat.Binary, ref serializationData);
            referencedUnityObjects = serializationData.ReferencedUnityObjects;
            return serializationData.BinaryData;
        }

        public static byte[] SerializeToBinary<T>(ref T value)
        {
            List<UnityEngine.Object> referencedUnityObjects = null;
            return SerializeToBinary(ref value, ref referencedUnityObjects);
        }

        public static byte[] SerializeToBinary<T>(ref T value, ref List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(Array.Empty<byte>(), referencedUnityObjects);
            Serialize(ref value, SerializationFormat.Binary, ref serializationData);
            referencedUnityObjects = serializationData.ReferencedUnityObjects;
            return serializationData.BinaryData;
        }

        public static T Deserialize<T>(SerializationFormat format, ref SerializationData serializationData)
        {
            return (T)Deserialize(typeof(T), format, ref serializationData);
        }

        public static object Deserialize(Type type, SerializationFormat format, ref SerializationData serializationData)
        {
            var buffer = serializationData.GetBuffer(format);
            if (buffer == null)
            {
                return null;
            }

            object result = null;
            using var stream = new MemoryStream(buffer);
            using var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateReader(format, stream);
            if (formatter == null)
            {
                throw new SerializationException(
                    $"Failed to create reader for format '{format}'. The format may not be supported.");
            }

            formatter.SetObjectTable(serializationData.ReferencedUnityObjects);

            var processor = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetProcessor(type);
            if (processor == null)
            {
                throw new SerializationException(
                    $"Cannot deserialize type '{type}'. No serialization processor found for this type. " +
                    $"Ensure the type is either a primitive type, a collection, or marked with [Serializable] or [EasySerializable].");
            }

            processor.ProcessUntyped(ref result, formatter);

            return result;
        }

        public static object DeserializeFromBinary(Type type, byte[] data)
        {
            return DeserializeFromBinary(type, data, null);
        }

        public static object DeserializeFromBinary(Type type, byte[] data, List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(data, referencedUnityObjects);
            return Deserialize(type, SerializationFormat.Binary, ref serializationData);
        }

        public static T DeserializeFromBinary<T>(byte[] data)
        {
            return DeserializeFromBinary<T>(data, null);
        }

        public static T DeserializeFromBinary<T>(byte[] data, List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(data, referencedUnityObjects);
            return Deserialize<T>(SerializationFormat.Binary, ref serializationData);
        }
    }
}
