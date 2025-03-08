using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace MovieApi.Converters
{
    public class CustomIntConverter : Int32Converter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text)) { return 0; }

            try
            {
                return base.ConvertFromString(text, row, memberMapData)!;
            }
            catch
            {
                return 0;
            }
        }
    }
}
