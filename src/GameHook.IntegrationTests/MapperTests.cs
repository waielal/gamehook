﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class MapperTests : BaseTest
    {
        [TestMethod]

        #region Gameboy
        // TODO: Add Red + Blue SRM combinations, should be expanded to 8.
        [DataRow("gb_pokemon_red_blue", "gb_pokemon_red_blue_0")]
        [DataRow("gb_pokemon_red_blue", "gbc_pokemon_red_blue_0")]

        [DataRow("gb_pokemon_yellow", "gb_pokemon_red_blue_0")]
        [DataRow("gb_pokemon_yellow", "gbc_pokemon_red_blue_0")]
        #endregion

        #region Gameboy Color
        [DataRow("gbc_pokemon_crystal", "gbc_pokemon_crystal_0")]

        // TODO: Add Gold + Silver SRM combinations, should be expanded to 2.
        [DataRow("gbc_pokemon_gold_silver", "gbc_pokemon_gold_0")]

        //[DataRow("gbc_zelda_links_awakening_dx", "gbc_pokemon_crystal_0")]
        #endregion

        #region Gameboy Advance
        //[DataRow("gba_metroid_fusion", "gba_pokemon_emerald_0")]

        [DataRow("gba_pokemon_emerald", "gba_pokemon_emerald_0")]

        [DataRow("gba_pokemon_firered", "gba_pokemon_emerald_0")]

        [DataRow("gba_pokemon_rubysapphire", "gba_pokemon_emerald_0")]
        #endregion

        #region Nintendo 64
        //[DataRow("n64_mario_kart_64", "gba_pokemon_emerald_0")]
        #endregion

        #region Nintendo DS
        [DataRow("nds_pokemon_platinum", "nds_pokemon_platinum_0")]
        #endregion

        #region NES
        [DataRow("nes_dragon_warrior_1", "gba_pokemon_emerald_0")]
        #endregion

        #region Playstation
        //[DataRow("psx_resident_evil_3_nemesis", "gba_pokemon_emerald_0")]
        #endregion

        #region Super Nintendo
        //[DataRow("snes_bs_zelda", "gba_pokemon_emerald_0")]
        #endregion

        public async Task DoesMapperLoad(string mapperName, string srmName)
        {
            mapperName = $"official_{mapperName}";
            srmName = $"{srmName}.json";

            Logger.LogInformation(string.Empty);
            Logger.LogInformation("=================================");
            Logger.LogInformation($"Mapper:\t{mapperName}");
            Logger.LogInformation($"SRM:\t\t{srmName}");
            Logger.LogInformation("=================================");
            Logger.LogInformation(string.Empty);

            await LoadSrm(srmName);
            await LoadMapper(mapperName);
        }
    }
}
