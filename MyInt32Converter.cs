using CsvHelper.Configuration;
using CsvHelper;

namespace RefreshDistrictDb
{
    internal class MyInt32Converter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            int retval;
            if ( Int32.TryParse(text, out retval))
            {
                return retval;
            }
            else
            {
                return int.MinValue;
            }    
        }
    }
}
