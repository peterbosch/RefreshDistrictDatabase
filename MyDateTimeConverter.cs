using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace RefreshDistrictDb
{
    public class MyDateTimeConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var formatProvider = new CultureInfo("en-US");
            DateTime dateTime;
            DateTime.TryParseExact(text.ToUpper(), "dd-MMM-yyyy", formatProvider, DateTimeStyles.None, out dateTime);
            return dateTime;
        }
    }
}