using GameHook.Domain;
using GameHook.Domain.Interfaces;

namespace GameHook.Application
{
    public class NES_PlatformOptions : IPlatformOptions
    {
        public EndianTypes EndianType { get; } = EndianTypes.BigEndian;

        public MemoryAddressBlock[] Ranges { get; } = new List<MemoryAddressBlock>()
        {
            new MemoryAddressBlock(0, "Internal RAM", 0x0000, 0x0400) // 2kB Internal RAM, mirrored 4 times
        }.ToArray();
    }

    public class SNES_PlatformOptions : IPlatformOptions
    {
        public EndianTypes EndianType { get; } = EndianTypes.LittleEndian;

        public MemoryAddressBlock[] Ranges { get; } = new List<MemoryAddressBlock>()
        {
            new MemoryAddressBlock(0, "?", 0x7E6D00, 0x7E7FFF)
        }.ToArray();
    }

    public class GB_PlatformOptions : IPlatformOptions
    {
        public EndianTypes EndianType { get; } = EndianTypes.LittleEndian;

        public MemoryAddressBlock[] Ranges { get; } = new List<MemoryAddressBlock>()
        {
            new MemoryAddressBlock(0, "ROM Bank 00", 0x0000, 0x3FFF),
            new MemoryAddressBlock(1, "ROM Bank 01", 0x4000, 0x7FFF),
            new MemoryAddressBlock(2, "VRAM", 0x8000, 0x9FFF),
            new MemoryAddressBlock(3, "External RAM (Part 1)", 0xA000, 0xAFFF),
            new MemoryAddressBlock(4, "External RAM (Part 2)", 0xB000, 0xBFFF),
            new MemoryAddressBlock(5, "Work RAM (Part 1)", 0xC000, 0xCFFF),
            new MemoryAddressBlock(6, "Work RAM (Part 2)", 0xD000, 0xDFFF),
            new MemoryAddressBlock(7, "I/O Registers", 0xFF00, 0xFF7F),
            new MemoryAddressBlock(8, "High RAM", 0xFF80, 0xFFFF)
        }.ToArray();
    }

    public class GBA_PlatformOptions : IPlatformOptions
    {
        public EndianTypes EndianType { get; } = EndianTypes.BigEndian;

        public MemoryAddressBlock[] Ranges { get; } = new List<MemoryAddressBlock>()
        {
            new MemoryAddressBlock(0, "Partial EWRAM 1", 0x00000000, 0x00003FFF),
            new MemoryAddressBlock(1, "Partial EWRAM 2", 0x02021000, 0x02022FFF),
            new MemoryAddressBlock(2, "Partial EWRAM 3", 0x02023000, 0x02023FFF),
            new MemoryAddressBlock(3, "Partial EWRAM 4", 0x02024000, 0x02027FFF),
            new MemoryAddressBlock(4, "Partial EWRAM 5", 0x02030000, 0x02033FFF),
            new MemoryAddressBlock(5, "Partial EWRAM 6", 0x02037000, 0x02039999),
            new MemoryAddressBlock(6, "Partial EWRAM 7", 0x0203A000, 0x0203AFFF),
            new MemoryAddressBlock(7, "IWRAM", 0x03001000, 0x03004FFF),
            new MemoryAddressBlock(8, "IWRAM", 0x03005000, 0x03005FFF),
            new MemoryAddressBlock(9, "IWRAM", 0x03006000, 0x03006000 + 9999)
        }.ToArray();
    }

    /* ===== PlayStation ===== */
    /* http://www.raphnet.net/electronique/psx_adaptor/Playstation.txt */
    /* https://problemkaputt.de/psx-spx.htm */
    public class PSX_PlatformOptions : IPlatformOptions
    {
        public EndianTypes EndianType { get; } = EndianTypes.BigEndian;
        public MemoryAddressBlock[] Ranges { get; } = new List<MemoryAddressBlock>()
        {
            new MemoryAddressBlock(0, "Kernel", 0x00000000, 0x0000ffff),
            new MemoryAddressBlock(1, "User Memory", 0x00010000, 0x001fffff),
            new MemoryAddressBlock(2, "Parallel Port", 0x1f000000, 0x001fffff),
            new MemoryAddressBlock(3, "Scratch Pad", 0x1f800000, 0x1f8003ff),
            new MemoryAddressBlock(4, "User Memory", 0x1f801000, 0x1f802fff),
            new MemoryAddressBlock(5, "Kernel and User Memory Mirror", 0x80000000, 0x801fffff),
            new MemoryAddressBlock(6, "Kernel and User Memory Mirror", 0xa0000000, 0xa01fffff),
            new MemoryAddressBlock(7, "BIOS", 0xbfc00000, 0xbfc7ffff)
        }.ToArray();
    }
}
