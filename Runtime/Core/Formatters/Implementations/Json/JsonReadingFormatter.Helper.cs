using System;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Core.Textual;
using EasyToolkit.Serialization.Utilities;
using UnityEngine.Assertions;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    public sealed partial class JsonReadingFormatter
    {
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
                                "JSON data format mismatch: expected array root when AutoWrapAtomicValueInArray is enabled. "
                                +
                                "The data was likely serialized with different settings or has been modified externally. "
                                +
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
                throw new DataFormatException(
                    $"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
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
                throw new DataFormatException(
                    $"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsFloat;
        }

        private long ReadAndValidateLong()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException(
                    $"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsLong;
        }

        private ulong ReadAndValidateULong()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException(
                    $"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsULong;
        }

        private int ReadAndValidateInt()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsNumber)
            {
                throw new DataFormatException(
                    $"Expected number at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
            }

            AdvanceArrayIndex();
            return node.AsInt;
        }

        private bool ReadAndValidateBool()
        {
            var node = GetCurrentNode();
            if (node == null || !node.IsBoolean)
            {
                throw new DataFormatException(
                    $"Expected boolean at '{_currentMemberName}', found {node?.Tag ?? JSONNodeType.NullValue}.");
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

        private Type ReadAndValidateType(Type expectedType, bool enableValidate = true)
        {
            var node = GetCurrentNode();
            if (!node.IsObject && !node.IsNull)
            {
                throw new DataFormatException($"Expected JSON object at '{_currentMemberName}', found {node.Tag}.");
            }

            // Only read type field if IncludeObjectType option is enabled
            if ((_jsonSettings?.Options & JsonFormatterOptions.IncludeObjectType) == 0)
            {
                return expectedType;
            }

            var typeNameField = _jsonSettings?.TypeNameField ?? "__meta_type__";
            var metaTypeNode = node[typeNameField];
            if (metaTypeNode == null)
            {
                throw new DataFormatException(
                    $"Missing required type field '{typeNameField}' in JSON object at '{_currentMemberName}'. " +
                    $"This field is required when '{nameof(JsonFormatterOptions)}.{nameof(JsonFormatterOptions.IncludeObjectType)}' is enabled. " +
                    $"Either include the '{typeNameField}' field in your JSON data, or disable '{nameof(JsonFormatterOptions.IncludeObjectType)}' in the formatter options.");
            }

            if (!metaTypeNode.IsString || metaTypeNode.Value.IsNullOrWhiteSpace())
            {
                throw new DataFormatException(
                    $"The '{typeNameField}' field must be a non-empty string containing a valid type name. " +
                    $"Expected location: '{_currentMemberName}'. Ensure the JSON data includes a valid type name in the '{typeNameField}' field.");
            }

            var metaType = SerializedTypeUtility.NameToType(metaTypeNode.Value);
            if (enableValidate && !metaType.IsDerivedFrom(expectedType))
            {
                throw new DataFormatException(
                    $"Type mismatch in json data. Expected type '{expectedType}', found '{metaType}'.");
            }

            return metaType;
        }

        /// <summary>
        /// Enters a missing member scope, pushing the current state to the stack.
        /// </summary>
        private void EnterMissingMemberScope()
        {
            _missingMemberStack.Push(_isInMissingMember);
            _isInMissingMember = true;
        }

        /// <summary>
        /// Exits the current missing member scope, restoring the previous state from the stack.
        /// </summary>
        private void ExitMissingMemberScope()
        {
            if (_missingMemberStack.Count > 0)
            {
                _isInMissingMember = _missingMemberStack.Pop();
            }
            else
            {
                _isInMissingMember = false;
            }
        }

        /// <summary>
        /// Checks if currently in a missing member scope.
        /// </summary>
        private bool IsInMissingMemberScope()
        {
            return _isInMissingMember;
        }

        /// <summary>
        /// Checks if the next member is marked as missing.
        /// </summary>
        private bool IsNextMemberMissing()
        {
            return _isNextMemberMissing;
        }

        /// <summary>
        /// Marks the next member as missing.
        /// </summary>
        private void MarkNextMemberMissing()
        {
            _isNextMemberMissing = true;
        }

        /// <summary>
        /// Clears the missing member flag for the next member.
        /// </summary>
        private void ClearNextMemberMissing()
        {
            _isNextMemberMissing = false;
        }

        /// <summary>
        /// Checks if we should assign default value instead of reading from JSON.
        /// </summary>
        private bool ShouldAssignDefaultValue()
        {
            if (!_returnDefaultOnEmptyMember)
                return false;

            if (IsNextMemberMissing())
            {
                ClearNextMemberMissing();
                return true;
            }

            if (IsInMissingMemberScope())
            {
                return true;
            }

            if (_jsonText.IsNullOrEmpty())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if we should skip reading the current member.
        /// </summary>
        private bool ShouldSkipMemberReading()
        {
            if (!_returnDefaultOnEmptyMember)
                return false;

            if (IsInMissingMemberScope())
            {
                return true;
            }

            if (_jsonText.IsNullOrEmpty())
            {
                return true;
            }

            if (_nodeStack.Count > 0)
            {
                var parent = _nodeStack.Peek();
                if (_currentMemberName != null && parent.IsObject && parent[_currentMemberName] == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
