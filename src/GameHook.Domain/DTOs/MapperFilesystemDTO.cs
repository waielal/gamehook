namespace GameHook.Domain.DTOs
{
    public enum MapperFilesystemTypes
    {
        Official,
        Custom
    }

    public class MapperFilesystemDTO
    {
        public string Id { get; set; } = string.Empty;
        public MapperFilesystemTypes Type { get; set; }
        public string AbsolutePath { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
