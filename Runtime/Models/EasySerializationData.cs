using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyToolKit.Serialization
{
    [Serializable]
    public struct EasySerializationData
    {
        [SerializeField] public FormatterType FormatterType;
        [SerializeField] public byte[] BinaryData;
        [SerializeField] public string StringData;
        [SerializeField] public List<UnityEngine.Object> ReferencedUnityObjects;

        public bool IsContainsData => BinaryData != null || StringData != null || ReferencedUnityObjects != null;

        public EasySerializationData(FormatterType formatterType)
        {
            FormatterType = formatterType;
            if (formatterType == FormatterType.Binary)
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

        public EasySerializationData(byte[] binaryData, FormatterType formatterType)
            : this(binaryData, new List<UnityEngine.Object>(), formatterType)
        {
        }

        public EasySerializationData(string stringData, FormatterType formatterType)
            : this(stringData, new List<UnityEngine.Object>(), formatterType)
        {
        }

        public EasySerializationData(byte[] binaryData, List<UnityEngine.Object> referencedUnityObjects,
            FormatterType formatterType)
        {
            if (formatterType != FormatterType.Binary)
            {
                throw new ArgumentException("Binary data can only be serialized by the FormatterType.Binary mode");
            }

            BinaryData = binaryData;
            StringData = null;
            ReferencedUnityObjects = referencedUnityObjects;
            FormatterType = formatterType;
        }

        public EasySerializationData(string stringData, List<UnityEngine.Object> referencedUnityObjects,
            FormatterType formatterType)
        {
            if (formatterType == FormatterType.Binary)
            {
                throw new ArgumentException("String data can not be serialized by the FormatterType.Binary mode");
            }

            StringData = stringData;
            BinaryData = null;
            ReferencedUnityObjects = referencedUnityObjects;
            FormatterType = formatterType;
        }

        public byte[] GetData()
        {
            if (!IsContainsData)
            {
                return new byte[] { };
            }

            if (FormatterType == FormatterType.Binary)
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
            if (FormatterType == FormatterType.Binary)
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
