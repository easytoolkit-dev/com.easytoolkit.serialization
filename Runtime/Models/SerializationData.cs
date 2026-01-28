using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolkit.Serialization
{
    [Serializable]
    public struct SerializationData
    {
        [SerializeField, CanBeNull] private byte[] _binaryData;
        [SerializeField, CanBeNull] private string _stringData;
        [SerializeField, CanBeNull] private List<UnityEngine.Object> _referencedUnityObjects;

        public SerializationData(byte[] binaryData, List<UnityEngine.Object> referencedUnityObjects = null)
        {
            _binaryData = binaryData;
            _stringData = null;
            _referencedUnityObjects = referencedUnityObjects;
        }

        public SerializationData(string stringData, List<UnityEngine.Object> referencedUnityObjects = null)
        {
            _binaryData = null;
            _stringData = stringData;
            _referencedUnityObjects = referencedUnityObjects;
        }

        [CanBeNull] public byte[] BinaryData
        {
            get => _binaryData;
            set => _binaryData = value;
        }

        [CanBeNull] public string StringData
        {
            get => _stringData;
            set => _stringData = value;
        }

        [CanBeNull] public List<UnityEngine.Object> ReferencedUnityObjects
        {
            get => _referencedUnityObjects;
            set => _referencedUnityObjects = value;
        }

        public void Clear()
        {
            _binaryData = null;
            _stringData = null;
            _referencedUnityObjects?.Clear();
        }

        public byte[] GetBuffer(SerializationFormat format)
        {
            if (format == SerializationFormat.Binary)
            {
                return BinaryData;
            }
            else
            {
                if (StringData == null)
                {
                    return null;
                }
                return Encoding.UTF8.GetBytes(StringData);
            }
        }

        public void SetBuffer(SerializationFormat format, byte[] buffer)
        {
            if (format == SerializationFormat.Binary)
            {
                BinaryData = buffer;
            }
            else
            {
                StringData = Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
