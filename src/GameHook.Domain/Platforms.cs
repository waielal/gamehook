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

    // All known N64 cores do not have a memory map defined.
    public class N64_PlatformOptions : IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; } = EndianTypeEnum.BigEndian;

        public IEnumerable<PlatformRange> Ranges { get; } = new List<PlatformRange>()
        {
            new PlatformRange("KSEG0", 0x80000000, 0x9FFFFFFF)
        };
    }

    // All known PSX cores do not have a memory map defined.
    public class PSX_PlatformOptions : IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; } = EndianTypeEnum.BigEndian;

        public IEnumerable<PlatformRange> Ranges { get; } = new List<PlatformRange>()
        {
        };
    }
}
