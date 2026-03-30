using System;
using System.Collections.Generic;
using System.Text;
using EasyToolkit.Serialization.Utilities;
using UnityEngine.Assertions;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// JSON writing formatter implementation. Serializes data to JSON format
    /// using a node-based tree building approach.
    /// </summary>
    public class JsonWritingFormatter : WritingFormatterBase
    {
        private readonly Stack<JSONNode> _nodeStack = new();
        private string _currentMemberName;
        private JSONNode _root;
        private byte[] _cachedBuffer;
        private string _cachedJsonText;
        private JsonFormatterSettings _jsonSettings;

        /// <inheritdoc />
        public override SerializationFormat FormatType => SerializationFormat.Json;

        /// <inheritdoc />
        protected override byte[] GetBuffer()
        {
            if (_cachedBuffer == null)
            {
                SerializeToJson();
            }
            return _cachedBuffer;
        }

        /// <inheritdoc />
        protected override int GetPosition()
        {
            throw new NotSupportedException(
                "GetPosition is not supported for JSON format. JSON is a tree-based format, not a stream-based format.");
        }

        /// <inheritdoc />
        protected override int GetLength()
        {
            return GetBuffer().Length;
        }

        /// <inheritdoc />
        protected override byte[] ToArray()
        {
            return GetBuffer();
        }

        /// <summary>
        /// Serializes the JSON tree to a byte buffer.
        /// </summary>
        private void SerializeToJson()
        {
            if (_root == null)
            {
                _cachedJsonText = "{}";
                _cachedBuffer = Encoding.UTF8.GetBytes(_cachedJsonText);
                return;
            }

            _cachedJsonText = _root.ToString();
            _cachedBuffer = Encoding.UTF8.GetBytes(_cachedJsonText);
        }

        /// <inheritdoc />
        protected override void BeginMember(string name)
        {
            _currentMemberName = name;
        }

        /// <inheritdoc />
        protected override void BeginObject(Type type)
        {
            var newObject = new JSONObject();

            if (type != null && _jsonSettings?.Options.HasFlag(JsonFormatterOptions.IncludeObjectType) == true)
            {
                var typeNameField = _jsonSettings?.TypeNameField ?? "__meta_type__";
                newObject[typeNameField] = SerializedTypeUtility.TypeToName(type);
            }

            AddToCurrentNode((JSONNode)newObject);
            _nodeStack.Push(newObject);
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            var popped = _nodeStack.Pop();
            if (popped != _root && _nodeStack.Count > 0)
            {
                var parent = _nodeStack.Peek();
                if (parent.IsArray)
                {
                    // This object was added to an array, nothing more to do
                }
            }
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            var newArray = new JSONArray();
            AddToCurrentNode((JSONNode)newArray);
            _nodeStack.Push(newArray);
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            _nodeStack.Pop();
        }

        /// <inheritdoc />
        protected override void Format(ref UnityEngine.Object unityObject)
        {
            var index = unityObject != null ? RegisterReference(unityObject) : 0;
            AddToCurrentNode(index);
        }

        /// <inheritdoc />
        protected override void Format(ref int value)
        {
            AddToCurrentNode(value);
        }

        /// <inheritdoc />
        protected override void Format(ref sbyte value)
        {
            AddToCurrentNode((int)value);
        }

        /// <inheritdoc />
        protected override void Format(ref short value)
        {
            AddToCurrentNode((int)value);
        }

        /// <inheritdoc />
        protected override void Format(ref long value)
        {
            AddToCurrentNode(value);
        }

        /// <inheritdoc />
        protected override void Format(ref byte value)
        {
            AddToCurrentNode((int)value);
        }

        /// <inheritdoc />
        protected override void Format(ref ushort value)
        {
            AddToCurrentNode((int)value);
        }

        /// <inheritdoc />
        protected override void Format(ref uint value)
        {
            AddToCurrentNode((long)value);
        }

        /// <inheritdoc />
        protected override void Format(ref ulong value)
        {
            AddToCurrentNode(value);
        }

        /// <inheritdoc />
        protected override void Format(ref bool value)
        {
            AddToCurrentNode(value);
        }

        /// <inheritdoc />
        protected override void FormatNullable(ref bool isNull)
        {
            if (isNull)
            {
                AddToCurrentNode((JSONNode)JSONNull.CreateOrGet());
            }
            // Non-null: do nothing, actual value will be written by subsequent Format calls
        }

        /// <inheritdoc />
        protected override void Format(ref float value)
        {
            AddToCurrentNode(value);
        }

        /// <inheritdoc />
        protected override void Format(ref double value)
        {
            AddToCurrentNode(value);
        }

        /// <inheritdoc />
        protected override void Format(ref decimal value)
        {
            AddToCurrentNode(value);
        }

        /// <inheritdoc />
        protected override void Format(ref string str)
        {
            if (str == null)
            {
                AddToCurrentNode((JSONNode)JSONNull.CreateOrGet());
            }
            else
            {
                AddToCurrentNode(str);
            }
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(int value)
        {
            var node = GetCurrentNode();
            if (node.IsArray)
            {
                node.AsArray.Add(value);
            }
            else if (_currentMemberName != null)
            {
                node.AsObject.Add(_currentMemberName, value);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(long value)
        {
            var node = GetCurrentNode();
            if (node.IsArray)
            {
                node.AsArray.Add(value);
            }
            else if (_currentMemberName != null)
            {
                node.AsObject.Add(_currentMemberName, value);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(ulong value)
        {
            var node = GetCurrentNode();
            if (node.IsArray)
            {
                node.AsArray.Add(value);
            }
            else if (_currentMemberName != null)
            {
                node.AsObject.Add(_currentMemberName, value);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(float value)
        {
            var node = GetCurrentNode();
            if (node.IsArray)
            {
                node.AsArray.Add(value);
            }
            else if (_currentMemberName != null)
            {
                node.AsObject.Add(_currentMemberName, value);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(double value)
        {
            var node = GetCurrentNode();
            if (node.IsArray)
            {
                node.AsArray.Add(value);
            }
            else if (_currentMemberName != null)
            {
                node.AsObject.Add(_currentMemberName, value);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(decimal value)
        {
            var stringValue = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            AddToCurrentNode(stringValue);
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(bool value)
        {
            var node = GetCurrentNode();
            if (node.IsArray)
            {
                node.AsArray.Add(value);
            }
            else if (_currentMemberName != null)
            {
                node.AsObject.Add(_currentMemberName, value);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Adds a value node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void AddToCurrentNode(string value)
        {
            var node = GetCurrentNode();
            if (node.IsArray)
            {
                node.AsArray.Add(value);
            }
            else if (_currentMemberName != null)
            {
                node.AsObject.Add(_currentMemberName, value);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Adds a JSON node to the current JSON node context.
        /// Handles both object member assignment and array element addition.
        /// </summary>
        /// <param name="node">The JSON node to add.</param>
        private void AddToCurrentNode(JSONNode node)
        {
            if (_root == null)
            {
                Assert.IsTrue(_nodeStack.Count == 0);
                _root = node;
                return;
            }

            var current = GetCurrentNode();
            if (current.IsArray)
            {
                current.AsArray.Add(node);
            }
            else if (_currentMemberName != null)
            {
                current.AsObject.Add(_currentMemberName, node);
                _currentMemberName = null;
            }
        }

        /// <summary>
        /// Gets the current JSON node from the top of the stack.
        /// </summary>
        /// <returns>The current JSON node.</returns>
        private JSONNode GetCurrentNode()
        {
            if (_nodeStack.Count > 0)
            {
                return _nodeStack.Peek();
            }

            if (_root == null)
            {
                EnsureRootNodeForAtomicValue();
                if (_root == null)
                {
                    throw new InvalidOperationException(
                        "Cannot write value without a root node. Call BeginArray() or BeginObject() first to create the root element.");
                }
            }

            return _root;
        }

        /// <inheritdoc />
        protected override void OnSettingsChanged(DataFormatterSettings settings)
        {
            _jsonSettings = settings as JsonFormatterSettings;
            base.OnSettingsChanged(settings);
        }

        /// <summary>
        /// Automatically creates a root array for atomic values if enabled.
        /// </summary>
        private void EnsureRootNodeForAtomicValue()
        {
            if (_root == null && _jsonSettings?.AutoWrapAtomicValueInArray == true)
            {
                var array = new JSONArray();
                _root = array;
                _nodeStack.Push(array);
            }
        }

        /// <inheritdoc />
        protected override void Dispose()
        {
            _currentMemberName = null;
            _root = null;
            _nodeStack.Clear();
            _cachedBuffer = null;
            _cachedJsonText = null;
            _jsonSettings = null;
        }
    }
}
