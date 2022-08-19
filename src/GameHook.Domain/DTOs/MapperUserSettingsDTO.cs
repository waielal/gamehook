namespace GameHook.Domain.DTOs
{
    public class OutputPropertyToFilesystemItem
    {
        public string Path { get; set; } = string.Empty;
        public string? Format { get; set; }
    }

    public class MapperUserSettingsDTO
    {
        public List<OutputPropertyToFilesystemItem> OutputPropertiesToFilesystem { get; set; } = new List<OutputPropertyToFilesystemItem>();
    }
}
