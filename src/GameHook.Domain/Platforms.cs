using GameHook.Domain.Interfaces;

namespace GameHook.Domain
{
    public class NES_PlatformOptions : IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; } = EndianTypeEnum.BigEndian;

        // TODO: Why can I only get back 0x400 or 1024 from RetroArch? Is this FCEUmm memory mapping only returning the first 1024 blocks?
        public IEnumerable<PlatformRange> Ranges { get; } = new List<PlatformRange>()
        {
            new PlatformRange("Internal RAM", 0x0000, 0x0400) // 2kB Internal RAM, mirrored 4 times
        };
    }

    public class SNES_PlatformOptions : IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; } = EndianTypeEnum.LittleEndian;

        public IEnumerable<PlatformRange> Ranges { get; } = new List<PlatformRange>()
        {
            new PlatformRange("?", 0x7E6D00, 0x7E7FFF)
        };
    }

    public class GB_PlatformOptions : IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; } = EndianTypeEnum.BigEndian;

        public IEnumerable<PlatformRange> Ranges { get; } = new List<PlatformRange>()
        {
            // new PlatformRange("External RAM (Part 1)", 0xA000, 0xAFFF),
            // new PlatformRange("External RAM (Part 2)", 0xB000, 0xBFFF),
            new PlatformRange("Work RAM (Part 1)", 0xC000, 0xCFFF),
            new PlatformRange("Work RAM (Part 2)", 0xD000, 0xDFFF),
            new PlatformRange("High RAM", 0xFF80, 0xFFFF)
        };
    }
}
