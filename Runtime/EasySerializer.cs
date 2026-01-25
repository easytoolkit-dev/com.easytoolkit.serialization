using System;
using System.Collections.Generic;
using EasyToolKit.Serialization.Formatters;
using EasyToolKit.Serialization.Processors;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Provides static methods for serializing and deserializing objects using different formats.
    /// </summary>
    public static class EasySerializer
    {
        /// <summary>
        /// Serializes the specified value using the specified format and populates the serialization data.
        /// </summary>
        public static void Serialize<T>(ref T value, SerializationFormat format, ref SerializationData serializationData)
        {
            var untypedValue = (object)value;
            Serialize(untypedValue, format, ref serializationData);
            value = (T)untypedValue;
        }

        /// <summary>
        /// Serializes the specified value using the specified format and populates the serialization data.
        /// </summary>
        public static void Serialize(object value, SerializationFormat format, ref SerializationData serializationData)
        {
            if (value == null)
            {
                serializationData.Clear();
                return;
            }

            var valueType = value.GetType();
            // Create writing formatter with internal buffer
            using var formatter = FormatterFactory.GetWriter(format);
            if (formatter == null)
            {
                throw new SerializationException(
                    $"Failed to create writer for format '{format}'. The format may not be supported.");
            }

            var processor = SerializationProcessorFactory.GetProcessor(valueType);
            if (processor == null)
            {
                throw new SerializationException(
                    $"Cannot serialize type '{valueType}'. No serialization processor found for this type. " +
                    $"Ensure the type is either a primitive type, a collection, or marked with [Serializable] or [EasySerializable].");
            }

            processor.IsRoot = true;
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
            serializationData.SetBuffer(format, formatter.ToArray());
        }

        /// <summary>
        /// Serializes the specified value to binary format.
        /// </summary>
        public static byte[] SerializeToBinary(object value)
        {
            List<UnityEngine.Object> referencedUnityObjects = null;
            return SerializeToBinary(value, ref referencedUnityObjects);
        }

        /// <summary>
        /// Serializes the specified value to binary format and captures referenced Unity objects.
        /// </summary>
        public static byte[] SerializeToBinary(object value, ref List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(Array.Empty<byte>(), referencedUnityObjects);
            Serialize(value, SerializationFormat.Binary, ref serializationData);
            referencedUnityObjects = serializationData.ReferencedUnityObjects;
            return serializationData.BinaryData;
        }

        /// <summary>
        /// Serializes the specified value to binary format.
        /// </summary>
        public static byte[] SerializeToBinary<T>(ref T value)
        {
            List<UnityEngine.Object> referencedUnityObjects = null;
            return SerializeToBinary(ref value, ref referencedUnityObjects);
        }

        /// <summary>
        /// Serializes the specified value to binary format and captures referenced Unity objects.
        /// </summary>
        public static byte[] SerializeToBinary<T>(ref T value, ref List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(Array.Empty<byte>(), referencedUnityObjects);
            Serialize(ref value, SerializationFormat.Binary, ref serializationData);
            referencedUnityObjects = serializationData.ReferencedUnityObjects;
            return serializationData.BinaryData;
        }

        /// <summary>
        /// Deserializes a value of the specified type from the serialization data.
        /// </summary>
        public static T Deserialize<T>(SerializationFormat format, ref SerializationData serializationData)
        {
            return (T)Deserialize(typeof(T), format, ref serializationData);
        }

        /// <summary>
        /// Deserializes a value of the specified type from the serialization data.
        /// </summary>
        public static object Deserialize(Type type, SerializationFormat format, ref SerializationData serializationData)
        {
            var buffer = serializationData.GetBuffer(format);
            if (buffer == null)
            {
                return null;
            }

            object result = null;
            using var formatter = FormatterFactory.GetReader(format);
            if (formatter == null)
            {
                throw new SerializationException(
                    $"Failed to create reader for format '{format}'. The format may not be supported.");
            }

            // Set the buffer on the formatter after creation
            formatter.SetBuffer(buffer);

            formatter.SetObjectTable(serializationData.ReferencedUnityObjects);

            var processor = SerializationProcessorFactory.GetProcessor(type);
            if (processor == null)
            {
                throw new SerializationException(
                    $"Cannot deserialize type '{type}'. No serialization processor found for this type. " +
                    $"Ensure the type is either a primitive type, a collection, or marked with [Serializable] or [EasySerializable].");
            }

            processor.IsRoot = true;
            processor.ProcessUntyped(ref result, formatter);

            return result;
        }

        /// <summary>
        /// Deserializes a value of the specified type from binary data.
        /// </summary>
        public static object DeserializeFromBinary(Type type, byte[] data)
        {
            return DeserializeFromBinary(type, data, null);
        }

        /// <summary>
        /// Deserializes a value of the specified type from binary data with referenced Unity objects.
        /// </summary>
        public static object DeserializeFromBinary(Type type, byte[] data, List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(data, referencedUnityObjects);
            return Deserialize(type, SerializationFormat.Binary, ref serializationData);
        }

        /// <summary>
        /// Deserializes a value of type T from binary data.
        /// </summary>
        public static T DeserializeFromBinary<T>(byte[] data)
        {
            return DeserializeFromBinary<T>(data, null);
        }

        /// <summary>
        /// Deserializes a value of type T from binary data with referenced Unity objects.
        /// </summary>
        public static T DeserializeFromBinary<T>(byte[] data, List<UnityEngine.Object> referencedUnityObjects)
        {
            var serializationData = new SerializationData(data, referencedUnityObjects);
            return Deserialize<T>(SerializationFormat.Binary, ref serializationData);
        }
    }
}
