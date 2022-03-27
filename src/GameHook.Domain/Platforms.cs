using GameHook.Domain.Interfaces;

namespace GameHook.Domain
{
    public class NES_PlatformOptions : IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; } = EndianTypeEnum.BigEndian;

        public IEnumerable<PlatformRange> Ranges { get; } = new List<PlatformRange>()
        {
            new PlatformRange("WRAM", 0x0000, 0x0400)
        };
    }

    public class GB_PlatformOptions : IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; } = EndianTypeEnum.BigEndian;

        public IEnumerable<PlatformRange> Ranges { get; } = new List<PlatformRange>()
        {
            new PlatformRange("WRAM0", 0xC000, 0xCFFF),
            new PlatformRange("WRAM1", 0xD000, 0xDFFF)
        };
    }
}
