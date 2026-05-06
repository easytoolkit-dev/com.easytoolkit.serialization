using System;
using System.Collections.Generic;
using System.Text;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Core.Foundation;
using EasyToolkit.Serialization.Utilities;
using UnityEngine.Assertions;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// JSON reading formatter implementation. Deserializes data from JSON format
    /// using a node-based traversal approach.
    /// </summary>
    public partial class JsonReadingFormatter : ReadingFormatterBase
    {
        /// <inheritdoc />
        public override SerializationFormat FormatType => SerializationFormat.Json;

        private string _jsonText;
        private string _currentMemberName;
        private readonly Stack<int> _arrayIndexStack = new();
        private JSONNode _root;
        private readonly Stack<JSONNode> _nodeStack = new();
        private bool _isNullScope;
        private JsonFormatterSettings _jsonSettings;
        private bool _returnDefaultOnEmptyMember;
        private readonly Stack<bool> _missingMemberStack = new();
        private bool _isInMissingMember;
        private bool _isNextMemberMissing;

        /// <inheritdoc />
        protected override void SetBuffer(ReadOnlySpan<byte> buffer)
        {
            _jsonText = Encoding.UTF8.GetString(buffer);
            _nodeStack.Clear();
            _currentMemberName = null;
            _arrayIndexStack.Clear();
            _missingMemberStack.Clear();
            _isInMissingMember = false;
            _isNextMemberMissing = false;
        }

        /// <inheritdoc />
        protected override ReadOnlySpan<byte> GetBuffer()
        {
            return Encoding.UTF8.GetBytes(_jsonText);
        }

        /// <inheritdoc />
        protected override void OnSettingsChanged(DataFormatterSettings settings)
        {
            _jsonSettings = settings as JsonFormatterSettings;
            _returnDefaultOnEmptyMember = settings.ReturnDefaultOnEmptyMember;
            base.OnSettingsChanged(settings);
        }

        /// <inheritdoc />
        protected override int GetPosition()
        {
            throw new NotSupportedException(
                "GetPosition is not supported for JSON format. JSON is a tree-based format, not a stream-based format.");
        }

        /// <inheritdoc />
        protected override int GetRemainingLength()
        {
            throw new NotSupportedException(
                "GetRemainingLength is not supported for JSON format. JSON is a tree-based format, not a stream-based format.");
        }

        /// <inheritdoc />
        protected override void BeginMember(string name)
        {
            _currentMemberName = name;

            // For JSON, check if the member exists in the current parent object
            if (ShouldSkipMemberReading())
            {
                MarkNextMemberMissing();
            }
        }

        /// <inheritdoc />
        protected override void BeginObject(Type expectedType)
        {
            if (ShouldSkipMemberReading())
            {
                EnterMissingMemberScope();
                return;
            }

            if (_root == null)
            {
                _root = JSON.Parse(_jsonText);
            }

            var enableValidate = expectedType != null;
            ReadAndValidateType(expectedType, enableValidate);

            var node = GetCurrentNode();
            if (!node.IsObject && !node.IsNull)
            {
                throw new DataFormatException($"Expected JSON object at '{_currentMemberName}', found {node.Tag}.");
            }

            // Advance parent array index if this object is nested inside an array
            AdvanceArrayIndex();

            _isNullScope = node.IsNull;
            if (!node.IsNull)
            {
                _nodeStack.Push(node);
            }
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            // Check if we're in a missing member scope
            if (_returnDefaultOnEmptyMember && IsInMissingMemberScope())
            {
                ExitMissingMemberScope();
                return;
            }

            // If we're in a null scope, we didn't add to hierarchy, so just reset the flag
            if (_isNullScope)
            {
                _isNullScope = false;
                return;
            }

            var endNode = _nodeStack.Peek();
            if (!endNode.IsObject)
            {
                throw new InvalidOperationException($"Mismatched BeginObject/EndObject. Current node is not an object.");
            }

            _nodeStack.Pop();
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            if (ShouldSkipMemberReading())
            {
                length = 0;
                EnterMissingMemberScope();
                return;
            }

            if (_root == null)
            {
                _root = JSON.Parse(_jsonText);
            }

            var node = GetCurrentNode();
            if (!node.IsArray)
            {
                throw new DataFormatException($"Expected JSON array at '{_currentMemberName}', found {node.Tag}.");
            }

            // Advance parent array index if this array is nested inside another array
            AdvanceArrayIndex();

            var arrayNode = node.AsArray;
            length = arrayNode.Count;
            _nodeStack.Push(arrayNode);
            _arrayIndexStack.Push(0);
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            // Check if we're in a missing member scope
            if (_returnDefaultOnEmptyMember && IsInMissingMemberScope())
            {
                ExitMissingMemberScope();
                return;
            }

            var endNode = _nodeStack.Peek();
            if (!endNode.IsArray)
            {
                throw new InvalidOperationException("Mismatched BeginArray/EndArray. Current node is not an array.");
            }

            _nodeStack.Pop();
            _arrayIndexStack.Pop();
        }

        /// <inheritdoc />
        protected override void Format(ref UnityEngine.Object unityObject)
        {
            if (ShouldAssignDefaultValue())
            {
                unityObject = default;
                return;
            }

            unityObject = ResolveReference(ReadAndValidateInt());
        }

        /// <inheritdoc />
        protected override void Format(ref int value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref sbyte value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = (sbyte)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref short value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = (short)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref long value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = ReadAndValidateLong();
        }

        /// <inheritdoc />
        protected override void Format(ref byte value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = (byte)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref ushort value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = (ushort)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref uint value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = (uint)ReadAndValidateULong();
        }

        /// <inheritdoc />
        protected override void Format(ref ulong value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = ReadAndValidateULong();
        }

        /// <inheritdoc />
        protected override void Format(ref bool value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = ReadAndValidateBool();
        }

        /// <inheritdoc />
        protected override void FormatNullable(ref bool isNull)
        {
            if (ShouldAssignDefaultValue())
            {
                isNull = true; // Treat as null
                return;
            }

            var node = GetCurrentNode();
            isNull = node.IsNull;
            if (node.IsNull)
            {
                AdvanceArrayIndex();
            }
        }

        /// <inheritdoc />
        protected override void Format(ref float value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = ReadAndValidateFloat();
        }

        /// <inheritdoc />
        protected override void Format(ref double value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = ReadAndValidateDouble();
        }

        /// <inheritdoc />
        protected override void Format(ref decimal value)
        {
            if (ShouldAssignDefaultValue())
            {
                value = default;
                return;
            }

            value = ReadAndValidateDecimal();
        }

        /// <inheritdoc />
        protected override void Format(ref string str)
        {
            if (ShouldAssignDefaultValue())
            {
                str = default;
                return;
            }

            str = ReadAndValidateString();
        }

        /// <inheritdoc />
        protected override void Dispose()
        {
            _currentMemberName = null;
            _arrayIndexStack.Clear();
            _jsonText = null;
            _root = null;
            _nodeStack.Clear();
            _jsonSettings = null;
            _missingMemberStack.Clear();
            _isInMissingMember = false;
            _isNextMemberMissing = false;
        }

        /// <inheritdoc />
        protected override Type PeekType(Type expectedType)
        {
            if (ShouldSkipMemberReading())
            {
                return null;
            }

            if (_root == null)
            {
                _root = JSON.Parse(_jsonText);
            }

            var enableValidate = expectedType != null;
            return ReadAndValidateType(expectedType, enableValidate);
        }
    }
}
