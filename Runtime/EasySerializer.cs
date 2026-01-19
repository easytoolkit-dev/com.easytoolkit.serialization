using System;
using System.Collections.Generic;
using System.IO;

namespace EasyToolKit.Serialization
{
    public static class EasySerializer
    {
        public static void Serialize<T>(ref T value, SerializationFormat format, ref EasySerializationData serializationData)
        {
            if (value == null)
            {
                serializationData.Clear();
                return;
            }

            // Build node tree first
            using var stream = new MemoryStream();
            var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateWriter(format, stream);

            var serializer = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetProcessor<T>();
            if (serializer == null)
            {
                throw new ArgumentException();
            }

            serializer.Process(ref value, formatter);

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

        public static byte[] SerializeToBinary<T>(ref T value)
        {
            List<UnityEngine.Object> referencedUnityObjects = null;
            return SerializeToBinary(ref value, ref referencedUnityObjects);
        }

        public static byte[] SerializeToBinary<T>(ref T value, ref List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new EasySerializationData(Array.Empty<byte>(), referencedUnityObjects);
            Serialize(ref value, SerializationFormat.Binary, ref serializationData);
            referencedUnityObjects = serializationData.ReferencedUnityObjects;
            return serializationData.BinaryData;
        }

        public static T Deserialize<T>(SerializationFormat format, ref EasySerializationData serializationData)
        {
            var buffer = serializationData.GetBuffer(format);
            if (buffer == null)
            {
                return default;
            }

            T result = default;
            using var stream = new MemoryStream(buffer);
            var formatter = SerializationEnvironment.Instance.GetFactory<IFormatterFactory>()
                .CreateReader(format, stream);
            formatter.SetObjectTable(serializationData.ReferencedUnityObjects);

            var serializer = SerializationEnvironment.Instance.GetFactory<ISerializationProcessorFactory>()
                .GetProcessor<T>();
            if (serializer == null)
            {
                throw new ArgumentException();
            }
            serializer.Process(ref result, formatter);

            return result;
        }

        public static T DeserializeFromBinary<T>(byte[] data)
        {
            return DeserializeFromBinary<T>(data, null);
        }

        public static T DeserializeFromBinary<T>(byte[] data, List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new EasySerializationData(data, referencedUnityObjects);
            return Deserialize<T>(SerializationFormat.Binary, ref serializationData);
        }
    }
}
