
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyToolkit.Core.Textual;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// JSON reading formatter implementation. Deserializes data from JSON format
    /// using a node-based traversal approach.
    /// </summary>
    public class JsonReadingFormatter : ReadingFormatterBase
    {
        /// <inheritdoc />
        public override SerializationFormat FormatType => SerializationFormat.Json;

        /// <inheritdoc />
        /// <remarks>
        /// JSON is a tree-based format, not a stream-based format, so it does not require
        /// stream validation before reading values.
        /// </remarks>
        protected override bool RequiresStreamValidation => false;

        private string _jsonText;
        private string _currentMemberName;
        private readonly Stack<int> _arrayIndexStack = new();
        private JSONNode _root;
        private readonly Stack<JSONNode> _nodeStack = new();
        private bool _isNullScope;
        private JsonFormatterSettings _jsonSettings;

        /// <inheritdoc />
        protected override void SetBuffer(ReadOnlySpan<byte> buffer)
        {
            _jsonText = Encoding.UTF8.GetString(buffer);
            _nodeStack.Clear();
            _currentMemberName = null;
            _arrayIndexStack.Clear();
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
        }

        /// <inheritdoc />
        protected override void BeginObject(Type type)
        {
            if (_root == null)
            {
                _root = JSON.Parse(_jsonText);
            }

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
            unityObject = ResolveReference(ReadAndValidateInt());
        }

        /// <inheritdoc />
        protected override void Format(ref int value)
        {
            value = ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref sbyte value)
        {
            value = (sbyte)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref short value)
        {
            value = (short)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref long value)
        {
            value = ReadAndValidateLong();
        }

        /// <inheritdoc />
        protected override void Format(ref byte value)
        {
            value = (byte)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref ushort value)
        {
            value = (ushort)ReadAndValidateInt();
        }

        /// <inheritdoc />
        protected override void Format(ref uint value)
        {
            value = (uint)ReadAndValidateULong();
        }

        /// <inheritdoc />
        protected override void Format(ref ulong value)
        {
            value = ReadAndValidateULong();
        }

        /// <inheritdoc />
        protected override void Format(ref bool value)
        {
            value = ReadAndValidateBool();
        }

        /// <inheritdoc />
        protected override void FormatNullable(ref bool isNull)
        {
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
            value = ReadAndValidateFloat();
        }

        /// <inheritdoc />
        protected override void Format(ref double value)
        {
            value = ReadAndValidateDouble();
        }

        /// <inheritdoc />
        protected override void Format(ref decimal value)
        {
            value = ReadAndValidateDecimal();
        }

        /// <inheritdoc />
        protected override void Format(ref string str)
        {
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
        }

        /// <summary>
        /// Gets the current JSON node based on whether we're in an array or object context.
        /// Validates scope consistency using <see cref="ReadingFormatterBase.IsInArrayScope"/> and <see cref="ReadingFormatterBase.IsInObjectScope"/>.
        /// </summary>
        private JSONNode GetCurrentNode()
        {
            if (_nodeStack.Count == 0)
            {
                // Auto-parse atomic values when enabled and no root exists
                if (_root == null)
                {
                    if (_jsonSettings?.AutoWrapAtomicValueInArray == true)
                    {
                        var root = JSON.Parse(_jsonText);
                        if (!root.IsArray)
                        {
                            throw new DataFormatException(
                            "JSON data format mismatch: expected array root when AutoWrapAtomicValueInArray is enabled. " +
                            "The data was likely serialized with different settings or has been modified externally. " +
                            "Ensure the reading configuration matches the writing configuration, or disable AutoWrapAtomicValueInArray.");
                        }

                        _root = root;
                        return _root[0];
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Cannot read value without a root node. Call BeginArray() or BeginObject() first to create the root element.");
                    }
                }

                return _root;
            }

            var parent = _nodeStack.Peek();

            // If parent is an array, access by index
            if (parent.IsArray)
            {
                Assert.IsTrue(IsInArrayScope,
                    "Attempting to read array element but not in array scope. Check BeginArray/EndArray pairing.");
                Assert.IsTrue(_arrayIndexStack.Count > 0, "Array index stack is empty when in array scope.");
                var arrayNode = parent.AsArray;
                var index = _arrayIndexStack.Peek();
                if (index >= arrayNode.Count)
                {
                    throw new DataFormatException(
                        $"Array index {index} is out of bounds. Array has {arrayNode.Count} elements. " +
                        "This may indicate corrupted data or a mismatch between the serialized format and the expected structure.");
                }
                return arrayNode[_arrayIndexStack.Peek()];
            }

            // At root level, we can access members without being in object scope
            // (the root JSON object is implicitly in object context)
            bool isAtRootLevel = _nodeStack.Count == 1;

            Assert.IsTrue(IsInObjectScope || isAtRootLevel,
                "Attempting to read object member but not in object scope. Check BeginObject/EndObject pairing.");

            if (_currentMemberName.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    "Cannot read field member without a member name. " +
                    "BeginMember() must be called before reading field members to set the member name.");
            }
            return parent[_currentMemberName];
        }

        /// <summary>
        /// Advances the array index after reading an element.
        /// Only advances when currently in array scope.
        /// </summary>
        private void AdvanceArrayIndex()
        {
            if (_nodeStack.Count == 0)
            {
                return;
            }

            var parent = _nodeStack.Peek();
            if (parent.IsArray)
            {
                Assert.IsTrue(IsInArrayScope,
                    "Attempting to advance array index but not in array scope. Check BeginArray/EndArray pairing.");
                Assert.IsTrue(_arrayIndexStack.Count > 0, "Array index stack is empty when in array scope.");
                var currentIndex = _arrayIndexStack.Pop();
                _arrayIndexStack.Push(currentIndex + 1);
            }
        }

        private double ReadAndValidateDouble()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException($"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsDouble;
        }

        private decimal ReadAndValidateDecimal()
        {
            var stringValue = ReadAndValidateString();
            // Parse from string to preserve decimal precision
            return decimal.Parse(stringValue, System.Globalization.CultureInfo.InvariantCulture);
        }

        private float ReadAndValidateFloat()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException($"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsFloat;
        }

        private long ReadAndValidateLong()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException($"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsLong;
        }

        private ulong ReadAndValidateULong()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException($"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsULong;
        }

        private int ReadAndValidateInt()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException($"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsInt;
        }

        private bool ReadAndValidateBool()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsBoolean)
            {
                throw new DataFormatException($"Expected boolean at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsBool;
        }

        private string ReadAndValidateString()
        {
            var node = GetCurrentNode();
            if (node == null)
            {
                return null;
            }

            if (!node.IsString)
            {
                throw new DataFormatException($"Expected string at '{_currentMemberName}', found {node.Tag}.");
            }

            AdvanceArrayIndex();
            return node.Value;
        }
    }
}
