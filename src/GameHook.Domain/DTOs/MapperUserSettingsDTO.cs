namespace GameHook.Domain.DTOs
{
    public class OutputPropertyOverrideItem
    {
        public string Path { get; set; } = string.Empty;
        public string? Format { get; set; }
    }

    public class MapperUserSettingsDTO
    {
        public List<OutputPropertyOverrideItem> OutputPropertyOverrides { get; set; } = new List<OutputPropertyOverrideItem>();
    }
}
