namespace GameHook.Domain.Infrastructure
{
    public class CustomStringFormat : IFormatProvider, ICustomFormatter
    {
        public object? GetFormat(Type? formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            else return null;
        }

        public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        {
            var result = arg?.ToString();

            switch (format?.ToUpper())
            {
                case "TOUPPER": return result?.ToUpper() ?? string.Empty;
                case "TOLOWER": return result?.ToLower() ?? string.Empty;
                case "TRIM": return result?.Trim() ?? string.Empty;
                default: return result ?? string.Empty;
            }
        }
    }
}
