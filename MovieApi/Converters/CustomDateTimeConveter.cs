using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace MovieApi.Converters
{
    public class CustomDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text)) { return DateTime.MinValue; }
               
            try
            {
                return base.ConvertFromString(text, row, memberMapData)!;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}
