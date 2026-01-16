using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyToolKit.Serialization
{
    [Serializable]
    public struct EasySerializationData
    {
        [SerializeField] public SerializationFormat Format;
        [SerializeField] public byte[] BinaryData;
        [SerializeField] public string StringData;
        [SerializeField] public List<UnityEngine.Object> ReferencedUnityObjects;

        public bool IsContainsData => BinaryData != null || StringData != null || ReferencedUnityObjects != null;

        public EasySerializationData(SerializationFormat format)
        {
            Format = format;
            if (format == SerializationFormat.Binary)
            {
                BinaryData = new byte[] { };
                StringData = null;
            }
            else
            {
                StringData = string.Empty;
                BinaryData = null;
            }

            ReferencedUnityObjects = new List<UnityEngine.Object>();
        }

        public EasySerializationData(byte[] binaryData, SerializationFormat format)
            : this(binaryData, new List<UnityEngine.Object>(), format)
        {
        }

        public EasySerializationData(string stringData, SerializationFormat format)
            : this(stringData, new List<UnityEngine.Object>(), format)
        {
        }

        public EasySerializationData(byte[] binaryData, List<UnityEngine.Object> referencedUnityObjects,
            SerializationFormat format)
        {
            if (format != SerializationFormat.Binary)
            {
                throw new ArgumentException("Binary data can only be serialized by the FormatterType.Binary mode");
            }

            BinaryData = binaryData;
            StringData = null;
            ReferencedUnityObjects = referencedUnityObjects;
            Format = format;
        }

        public EasySerializationData(string stringData, List<UnityEngine.Object> referencedUnityObjects,
            SerializationFormat format)
        {
            if (format == SerializationFormat.Binary)
            {
                throw new ArgumentException("String data can not be serialized by the FormatterType.Binary mode");
            }

            StringData = stringData;
            BinaryData = null;
            ReferencedUnityObjects = referencedUnityObjects;
            Format = format;
        }

        public byte[] GetData()
        {
            if (!IsContainsData)
            {
                return new byte[] { };
            }

            if (Format == SerializationFormat.Binary)
            {
                return BinaryData;
            }

            if (string.IsNullOrEmpty(StringData))
            {
                return new byte[] { };
            }

            return Encoding.UTF8.GetBytes(StringData);
        }

        public void SetData(byte[] data)
        {
            if (Format == SerializationFormat.Binary)
            {
                BinaryData = data;
            }
            else
            {
                StringData = Encoding.UTF8.GetString(data);
            }
        }
    }
}
