using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;


namespace mvc_EF_访问数据.Commons
{
    /// <summary>
    /// Serialize and Deserialize Lists of any object type to CSV.
    /// </summary>
    public class CsvSerializer<T> where T : class, new()
    {
        #region Fields

        private bool _ignoreEmptyLines = true;

        private bool _ignoreReferenceTypesExceptString = true;

        private string _newlineReplacement = ((char)0x254).ToString();

        private List<PropertyInfo> _properties;

        private string _replacement = ((char)0x255).ToString();

        private string _rowNumberColumnTitle = "RowNumber";

        private char _separator = ',';

        private bool _useEofLiteral = false;

        private bool _useLineNumbers = true;

        #endregion Fields

        #region Properties

        public bool IgnoreEmptyLines
        {
            get { return _ignoreEmptyLines; }
            set { _ignoreEmptyLines = value; }
        }

        public bool IgnoreReferenceTypesExceptString
        {
            get { return _ignoreReferenceTypesExceptString; }
            set { _ignoreReferenceTypesExceptString = value; }
        }

        public string NewlineReplacement
        {
            get { return _newlineReplacement; }
            set { _newlineReplacement = value; }
        }

        public string Replacement
        {
            get { return _replacement; }
            set { _replacement = value; }
        }

        public string RowNumberColumnTitle
        {
            get { return _rowNumberColumnTitle; }
            set { _rowNumberColumnTitle = value; }
        }

        public char Separator
        {
            get { return _separator; }
            set { _separator = value; }
        }

        public bool UseEofLiteral
        {
            get { return _useEofLiteral; }
            set { _useEofLiteral = value; }
        }

        public bool UseLineNumbers
        {
            get { return _useLineNumbers; }
            set { _useLineNumbers = value; }
        }

        #endregion Properties

        /// <summary>
        /// Csv Serializer
        /// Initialize by selected properties from the type to be de/serialized
        /// </summary>
        public CsvSerializer()
        {
            var type = typeof(T);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance
                                                | BindingFlags.GetProperty | BindingFlags.SetProperty);


            var q = properties.AsQueryable();

            if (IgnoreReferenceTypesExceptString)
            {
                q = q.Where(a => a.PropertyType.IsValueType || a.PropertyType.Name == "String");
            }

            var r = from a in q
                    where a.GetCustomAttributes(typeof(CsvIgnoreAttribute), false) == null || a.GetCustomAttributes(typeof(CsvIgnoreAttribute), false).Length == 0
                    orderby a.Name
                    select a;

            _properties = r.ToList();
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        public IList<T> Deserialize(Stream stream)
        {
            string[] columns;
            string[] rows;

            try
            {
                using (var sr = new StreamReader(stream))
                {
                    columns = sr.ReadLine().Split(Separator);
                    rows = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }

            }
            catch (Exception ex)
            {
                throw new InvalidCsvFormatException("The CSV File is Invalid. See Inner Exception for more inoformation.", ex);
            }

            var data = new List<T>();

            for (int row = 0; row < rows.Length; row++)
            {
                var line = rows[row];

                if (IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (!IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                {
                    throw new InvalidCsvFormatException(string.Format(@"Error: Empty line at line number: {0}", row));
                }

                var parts = line.Split(Separator);

                var firstColumnIndex = UseLineNumbers ? 2 : 1;
                if (parts.Length == firstColumnIndex && parts[firstColumnIndex - 1] != null && parts[firstColumnIndex - 1] == "EOF")
                {
                    break;
                }

                var datum = new T();

                var start = UseLineNumbers ? 1 : 0;
                for (int i = start; i < parts.Length; i++)
                {
                    var value = parts[i];
                    var column = columns[i];

                    // continue of deviant RowNumber column condition
                    // this allows for the deserializer to implicitly ignore the RowNumber column
                    if (column.Equals(RowNumberColumnTitle) && !_properties.Any(a => a.Name.Equals(RowNumberColumnTitle)))
                    {
                        continue;
                    }

                    value = value
                        .Replace(Replacement, Separator.ToString())
                        .Replace(NewlineReplacement, Environment.NewLine).Trim();

                    var p = _properties.FirstOrDefault(a => a.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase));

                    /// ignore property csv column, Property not found on targing type
                    if (p == null)
                    {
                        continue;
                    }

                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var convertedvalue = converter.ConvertFrom(value);

                    p.SetValue(datum, convertedvalue, null);
                }

                data.Add(datum);
            }

            return data;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="data">data</param>
        public void Serialize(Stream stream, IList<T> data, string[] propertyNames)
        {
            string s = Serialize(data, propertyNames);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(s.Trim());
            }
        }

        public string Serialize(IList<T> data, string[] propertyNames)
        {
            var sb = new StringBuilder();
            var values = new List<string>();

            sb.AppendLine(GetHeader(propertyNames));

            var row = 1;
            foreach (var item in data)
            {
                values.Clear();

                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }
                var properties = propertyNames == null ? _properties : OrderProperties(_properties, propertyNames);
                foreach (var p in properties)
                {
                    var raw = p.GetValue(item, null);
                    var value = raw == null ? "" :
                        raw.ToString()
                            .Replace(Separator.ToString(), Replacement)
                            .Replace(Environment.NewLine, NewlineReplacement);
                    values.Add(value);
                }
                sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
            }

            if (UseEofLiteral)
            {
                values.Clear();

                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }

                values.Add("EOF");

                sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
            }
            return sb.ToString();
        }

        public IEnumerable<PropertyInfo> OrderProperties(IEnumerable<PropertyInfo> properties, string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                var property = properties.SingleOrDefault(p => p.Name == propertyName);
                if (property != null)
                {
                    yield return property;
                }
            }
        }

        public IEnumerable<string> GetHeaders(IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((attributes != null) && (attributes.Length > 0))
                {
                    var descriptionAttribute = attributes[0] as System.ComponentModel.DescriptionAttribute;
                    var description = descriptionAttribute.Description;
                    yield return description;
                }
                else
                {
                    yield return property.Name;
                }
            }
        }

        /// <summary>
        /// Get Header
        /// </summary>
        /// <returns></returns>
        private string GetHeader(string[] propertyNames)
        {
            var properties = propertyNames == null ? _properties : OrderProperties(_properties, propertyNames);
            var header = GetHeaders(properties);

            if (UseLineNumbers)
            {
                header = new string[] { RowNumberColumnTitle }.Union(header);
            }

            return string.Join(Separator.ToString(), header.ToArray());
        }
    }

    public class CsvIgnoreAttribute : Attribute { }

    public class InvalidCsvFormatException : Exception
    {
        /// <summary>
        /// Invalid Csv Format Exception
        /// </summary>
        /// <param name="message">message</param>
        public InvalidCsvFormatException(string message)
            : base(message) { }

        public InvalidCsvFormatException(string message, Exception ex)
            : base(message, ex) { }
    }
}