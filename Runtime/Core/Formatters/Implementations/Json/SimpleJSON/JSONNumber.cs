using System;
using System.Globalization;
using System.Text;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    internal partial class JSONNumber : JSONNode
    {
        private double m_Data;
        private long m_LongData;
        private bool m_IsLong;

        public override JSONNodeType Tag { get { return JSONNodeType.Number; } }
        public override bool IsNumber { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get
            {
                return m_IsLong
                    ? m_LongData.ToString(CultureInfo.InvariantCulture)
                    : m_Data.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                if (JSONNode.IsIntegerToken(value))
                {
                    long lv;
                    if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out lv))
                    {
                        m_LongData = lv;
                        m_IsLong = true;
                        return;
                    }
                }
                double v;
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                {
                    m_Data = v;
                    m_IsLong = false;
                }
            }
        }

        public override double AsDouble
        {
            get
            {
                return m_IsLong ? m_LongData : m_Data;
            }
            set
            {
                m_Data = value;
                m_IsLong = false;
            }
        }
        public override long AsLong
        {
            get
            {
                return m_IsLong ? m_LongData : (long)m_Data;
            }
            set
            {
                m_LongData = value;
                m_IsLong = true;
            }
        }
        public override ulong AsULong
        {
            get
            {
                if (!m_IsLong)
                    return (ulong)m_Data;
                unsafe
                {
                    // Use unsafe conversion to preserve original bits
                    long temp = m_LongData;
                    return *(ulong*)&temp;
                }
            }
            set
            {
                // Use unsafe conversion to preserve data when ulong exceeds long.MaxValue
                unsafe
                {
                    ulong temp = value;
                    m_LongData = *(long*)&temp;
                }
                m_IsLong = true;
            }
        }

        public JSONNumber(double aData)
        {
            m_Data = aData;
            m_IsLong = false;
        }

        public JSONNumber(long aData)
        {
            m_LongData = aData;
            m_IsLong = true;
        }

        public JSONNumber(ulong aData)
        {
            // Use unsafe conversion to preserve data when ulong exceeds long.MaxValue
            unsafe
            {
                ulong temp = aData;
                m_LongData = *(long*)&temp;
            }
            m_IsLong = true;
        }

        public JSONNumber(string aData)
        {
            Value = aData;
        }

        public override JSONNode Clone()
        {
            return m_IsLong ? new JSONNumber(m_LongData) : new JSONNumber(m_Data);
        }

        internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
        {
            aSB.Append(m_IsLong
                ? m_LongData.ToString(CultureInfo.InvariantCulture)
                : m_Data.ToString(CultureInfo.InvariantCulture));
        }
        private static bool IsNumeric(object value)
        {
            return value is int || value is uint
                || value is float || value is double
                || value is decimal
                || value is long || value is ulong
                || value is short || value is ushort
                || value is sbyte || value is byte;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (base.Equals(obj))
                return true;
            JSONNumber s2 = obj as JSONNumber;
            if (s2 != null)
            {
                if (m_IsLong && s2.m_IsLong)
                    return m_LongData == s2.m_LongData;
                return AsDouble == s2.AsDouble;
            }
            if (IsNumeric(obj))
                return Convert.ToDouble(obj) == AsDouble;
            return false;
        }
        public override int GetHashCode()
        {
            return m_IsLong ? m_LongData.GetHashCode() : m_Data.GetHashCode();
        }
        public override void Clear()
        {
            m_Data = 0;
            m_LongData = 0;
            m_IsLong = false;
        }
    }
}
