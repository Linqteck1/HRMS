using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Globalization;
using HRMSWeb.Models;

public static class CRM_Common
{

    public static string RenderViewToString(ControllerContext context, string viewName, object model)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = context.RouteData.GetRequiredString("action");

        var viewData = new ViewDataDictionary(model);

        using (var sw = new StringWriter())
        {
            var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
            var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
            viewResult.View.Render(viewContext, sw);

            return sw.GetStringBuilder().ToString();
        }
    }

    public static bool CheckPermissions(string Permissions, string Page)
    {
        Session_CRM sess = (Session_CRM)HttpContext.Current.Session["CRM_Session"];

        var Allow = sess.AllPermissions.Where(x => x.AT_Pages.Controller == Page && x.AT_Permission.Permission == Permissions).FirstOrDefault();
        return Allow == null ? false : true;

    }
    public static string GetControler(ViewContext vc)
    {
        return vc.Controller.ValueProvider.GetValue("controller").RawValue.ToString();
    }


    public static DataTable ListToDataTable<T>(this IList<T> data)
    {
        DataTable dt = new DataTable();
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
        for (int i = 0; i < props.Count; i++)
        {
            PropertyDescriptor prop = props[i];
            if (prop.PropertyType.Name.Contains("Nullable"))
                dt.Columns.Add(prop.Name, typeof(String));
            else
                dt.Columns.Add(prop.Name, prop.PropertyType);
        }
        object[] values = new object[props.Count];
        foreach (T t in data)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = props[i].GetValue(t);
            }
            dt.Rows.Add(values);
        }
        return dt;
    }
    public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
    {
        try
        {
            List<T> list = new List<T>();

            foreach (var row in table.AsEnumerable())
            {
                T obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                        propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(obj);
            }

            return list;
        }
        catch
        {
            return null;
        }
    }



    public static string Encrypt(string originalString)
    {
        return Encrypt(originalString, getKey);
    }

    public static string Encrypt(string originalString, byte[] bytes)
    {
        try
        {
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);

            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();

            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length).Replace("+", "-").Replace("/", "_");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static string Decrypt(string originalString)
    {
        return Decrypt(originalString.Replace("-", "+").Replace("_", "/"), getKey);
    }

    public static string Decrypt(string cryptedString, byte[] bytes)
    {
        try
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }

            cryptedString = Regex.Replace(cryptedString, "[ ]", "+");

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static string GetUniqueID()
    {
        int maxSize = 30;
        char[] chars = new char[62];
        string a;
        a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        chars = a.ToCharArray();
        int size = maxSize;
        byte[] data = new byte[1];
        System.Security.Cryptography.RNGCryptoServiceProvider crypto = new System.Security.Cryptography.RNGCryptoServiceProvider();
        //crypto.GetNonZeroBytes(data);

        data = new byte[size];
        crypto.GetNonZeroBytes(data);
        System.Text.StringBuilder result = new System.Text.StringBuilder(size);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length - 1)]);
        }
        return result.ToString();
    }

    public static bool WriteErrorFile(string contents, string strPath)
    {
        StreamWriter writer = new StreamWriter(strPath, true);
        writer.Write(contents);
        writer.Flush();
        writer.Close();
        writer.Dispose();
        writer = null;
        return true;
    }

    public static string getVisitorsIP()
    {
        string VisitorsIPAddr = string.Empty;
        //Users IP Address.                
        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            //To get the IP address of the machine and not the proxy
            VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        else if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
            VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
        else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;

        return VisitorsIPAddr;
    }

    public static string getVisitorsBrowserInfo()
    {
        string VisitorsBrowserInfo = string.Empty;
        HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
        string UserAgent = HttpContext.Current.Request.UserAgent;
        VisitorsBrowserInfo = "{Browser-Capabilities "
                 + "(Type = '" + browser.Type + "' "
                 + "Name = '" + browser.Browser + "' "
                 + "Version = '" + browser.Version + "' "
                 + "Major Version = '" + browser.MajorVersion + "' "
                 + "Minor Version = '" + browser.MinorVersion + "' "
                 + "Platform = '" + browser.Platform + "' "
                 + "Is Win32 = '" + browser.Win32 + "' "
                 + "Is Beta = '" + browser.Beta + "' "
                 + "Supports Cookies = '" + browser.Cookies + "' "
                 + "Supports ECMAScript = '" + browser.EcmaScriptVersion.ToString() + "' "
                 + "Supports JavaScript Version = '" + browser.JScriptVersion + "' "
                 + "UserAgent = '" + UserAgent + "')}";
        return VisitorsBrowserInfo;
    }

    public static string get_setting(string name, string default_value)
    {
        NameValueCollection name_values = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("appSettings");
        if (string.IsNullOrEmpty(name_values[name]))
        {
            return default_value;
        }
        else
        {
            return name_values[name];
        }
    }

    public static string SubStr(string str, int length)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        if (str.Length > length)
            return str.Substring(0, length) + "...";
        else
            return str;
    }

    public static string SubStrSimple(string str, int length)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        if (str.Length > length)
            return str.Substring(0, length);
        else
            return str;
    }

    public static string RemoveHTML(string StringWithHTML)
    {
        if (string.IsNullOrEmpty(StringWithHTML)) return string.Empty;
        return Regex.Replace(StringWithHTML, @"<(.|\n)*?>", string.Empty);
    }

    public static string Find(string input, string StartStr, string LastStr)
    {
        int Start = input.IndexOf(StartStr);
        int length = (input.LastIndexOf(LastStr) - Start) + LastStr.Length;
        return input.Substring(Start, length);
    }

    public static string GetByteContent(byte[] content)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        return encoding.GetString(content);
    }

    public static bool checkEmail(string EmailAddress)
    {
        string pattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";

        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return regex.IsMatch(EmailAddress);
    }

    public static byte[] getKey
    {
        get
        {
            return ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["EncryptKey"].ToString());
        }
    }

    public static string UppercaseFirst(string s, char separator)
    {
        s = s.ToLower().Trim();
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        string[] word = s.Split(separator);
        string NewStr = "";
        for (int i = 0; i < word.Length; i++)
        {
            char[] a = word[i].ToCharArray();
            if (a.Length > 0)
            {
                a[0] = char.ToUpper(a[0]);
                if (i == word.Length - 1)
                    NewStr += new string(a);
                else
                    NewStr += new string(a) + separator;
            }
        }
        return NewStr;
    }

    public static string DateDiffrence(DateTime StartTime, DateTime EndTime)
    {
        //DateTime dt = StartTime;
        
        //var tody = EndTime;
        //var Val = dt;
        //var DiffMs = (tody - Val).TotalMilliseconds;
        //var diffDays = Math.Floor(Convert.ToDecimal(DiffMs) / 86400000); // days
        //var diffHrs = Math.Floor((Convert.ToDecimal(DiffMs) % 86400000) / 3600000); // hours
        //var diffMins = Math.Round(((Convert.ToDecimal(DiffMs) % 86400000) % 3600000) / 60000);

        TimeSpan span = (EndTime - StartTime);
        string LastTouch = (span.Days != 0 ? span.Days + "days " : "") + (span.Hours != 0 ? span.Hours + "hours " : "") + (span.Minutes != 0 ? span.Minutes + "minutes " : "") + (span.Seconds != 0 ? span.Seconds + "seconds " : "0");
        
        return LastTouch;
    }
    public static string ConvertIdsName(String Val, char FirstDelimeter, char SecondDelimeter)
    {
        string AgentIds = "";
        string AgentName = "";
        foreach (var item in Val.Split(FirstDelimeter))
        {
            AgentName += item.Split(':')[0] + ",";
            AgentIds += Encrypt(item.Split(':')[1]) + ",";
        }
        if (AgentIds.Length > 0 && AgentName.Length > 0)
        {
            AgentIds = AgentIds.Substring(0, AgentIds.Length - 1);
            AgentName = AgentName.Substring(0, AgentName.Length - 1);
        }
        return AgentIds + ":" + AgentName;
    }
    public const string
                 LeadEmail = "Lead Email",
                 Email = "Email",
                 LoginInfo = "Login Info",
                 PropertyAlert = "Property Alert",
                DripCompaign = "Drip Compaign";
    public const string
        DailyPropertyAlert = "Daily",
        ASAPPropertyAlert = "ASAP",
        MonthlyPropertyAlert = "Monthly",
    NewsLetterEmail = "News Letter Email",
    LetterEmail = "Letter Email",
    PropertyListingEmail = "Property Listing Email";
    public static int[] tointarray(string value, char sep, DateTime StartDate)
    {
        string[] sa = value.Split(sep);
        int[] ia = new int[sa.Length];
        for (int i = 0; i < ia.Length; ++i)
        {
            DateTime endOfMonth = new DateTime(StartDate.Year, StartDate.Month, DateTime.DaysInMonth(StartDate.Year, StartDate.Month));
            int j;
            string s = sa[i].Trim();
            if (int.TryParse(s, out j))
            {
                ia[i] = j;
            }
            else if (s == "Last")
            {
                ia[i] = endOfMonth.Day;
            }
        }
        return ia;
    }
    public static IEnumerable<Tuple<int, int>> MonthsBetween(
             DateTime startDate,
             DateTime endDate)
    {
        DateTime iterator;
        DateTime limit;

        if (endDate > startDate)
        {
            iterator = new DateTime(startDate.Year, startDate.Month, 1);
            limit = endDate;
        }
        else
        {
            iterator = new DateTime(endDate.Year, endDate.Month, 1);
            limit = startDate;
        }

        var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        while (iterator <= limit)
        {
            yield return Tuple.Create(
                iterator.Month,
                iterator.Year);
            iterator = iterator.AddMonths(1);
        }
    }
    public static string GetGroupByType(DateTime StartDate, DateTime EndDate)
    {
        var Days = (EndDate - StartDate).Days;
        return Days < 8 ? "Days" : Days > 7 && Days < 31 ? "Week" : Days > 31 && Days < 365 ? "Month" : Days > 365 ? "Year" : "";
    }
    public static string GetNullMsg()
    {
        return "<p class='nullMsg'>Nothing To Show</p>";
    }
    public static bool IsPathExist(string path)
    {
        return File.Exists(path);
    }
}