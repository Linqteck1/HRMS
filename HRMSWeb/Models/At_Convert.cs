using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSWeb.Models
{
    public class At_Convert
    {
        public static int ToInt(object value)
        {
            int parseVal;
            return ((value == null) || (value == DBNull.Value)) ? 0 : int.TryParse(value.ToString(), out parseVal) ? parseVal : 0;
        }

        public static byte ToByte(object value)
        {
            byte parseVal;
            return ((value == null) || (value == DBNull.Value)) ? (byte)0 : byte.TryParse(value.ToString(), out parseVal) ? parseVal : (byte)0;
        }

        public static double ToDouble(object value)
        {            
            double parseVal;
            return ((value == null) || (value == DBNull.Value)) ? 0 : double.TryParse(value.ToString(), out parseVal) ? parseVal : 0;
        }

        public static decimal ToDecimal(object value)
        {
            decimal parseVal;
            return ((value == null) || (value == DBNull.Value)) ? 0 : decimal.TryParse(value.ToString(), out parseVal) ? parseVal : 0;
        }

        public static DateTime ToDateTime(object value)
        {
            DateTime parseVal;
            return ((value == null) || (value == DBNull.Value)) ? GetDefaultDate() : DateTime.TryParse(value.ToString(), out parseVal) ? parseVal : GetDefaultDate();
        }

        public static string ToString(object value)
        {
            return ((value == null) || (value == DBNull.Value)) ? GetDefaultString() : value.ToString();
        }

        public static bool ToBoolean(object value)
        {
            bool parseVal;
            return ((value == null) || (value == DBNull.Value)) ? GetDefaultBoolean() : bool.TryParse(value.ToString(), out parseVal) ? parseVal : GetDefaultBoolean();
        }

        public static DateTime GetDefaultDate()
        {
            return new DateTime(1900, 1, 1);
        }

        public static bool GetDefaultBoolean()
        {
            return false;
        }

        public static string GetDefaultString()
        {
            return string.Empty;
        }
    }
}