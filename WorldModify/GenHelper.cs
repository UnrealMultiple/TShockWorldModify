using System;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;


namespace WorldModify
{
    class GenHelper
    {
        private static bool isTaskRunning = false;


        public static void Gen2(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                op.SendInfoMessage("/gen2 <大小> <腐化> <种子> [彩蛋特性], 重建地图");
                op.SendInfoMessage("大小：小/中/大 或 1/2/3, 0表示忽略");
                op.SendInfoMessage("腐化：腐化/猩红 或 1/2, 0表示随机");
                op.SendInfoMessage("种子：输入任意种子名，0表示随机");
                op.SendInfoMessage("彩蛋特性：种子名中间输入英文逗号，例如 2020,ftw");
                return;
            }

            if (args.Parameters.Count < 3)
            {
                op.SendErrorMessage("参数不够，请输入 /gen2 help 查询用法");
                return;
            }

            string sizeStr = args.Parameters[0];
            string evilStr = args.Parameters[1];
            string seedStr = args.Parameters[2];
            args.Parameters.RemoveAt(0);
            args.Parameters.RemoveAt(0);
            args.Parameters.RemoveAt(0);
            string eggStr = string.Join(" ", args.Parameters);

            int size = 0;
            if (sizeStr == "小" || sizeStr == "1")
                size = 1;
            else if (sizeStr == "中" || sizeStr == "2")
                size = 2;
            else if (sizeStr == "大" || sizeStr == "3")
                size = 3;

            int evil = -1;
            if (evilStr == "腐化" || evilStr == "1")
                evil = 0;
            else if (evilStr == "猩红" || evilStr == "2")
                evil = 1;

            GenWorld(seedStr, size, evil, eggStr);
        }

        // 参考：https://github.com/Illuminousity/WorldRefill/blob/master/WorldRefill/WorldRefill.cs#L997
        private static async void GenWorld(string seedStr = "", int size = 0, int evil = -1, string eggStr = "")
        {
            if (isTaskRunning) return;

            Console.WriteLine($"seed:{seedStr}");
            TSPlayer.All.SendErrorMessage("[i:556]世界正在解体~");
            int secondLast = utils.GetUnixTimestamp;

            ProcessSpecialWorldSeeds(seedStr);
            ProcessEggSeeds(eggStr);
            seedStr = seedStr.ToLowerInvariant();
            if (string.IsNullOrEmpty(seedStr) || seedStr == "0")
                seedStr = "random";
            if (Main.ActiveWorldFileData.Seed == 5162020)
                seedStr = "5162020";

            if (seedStr == "random")
                Main.ActiveWorldFileData.SetSeedToRandom();
            else
                Main.ActiveWorldFileData.SetSeed(seedStr);

            // 世界大小 腐化类型
            int tilesX = 0;
            int tilesY = 0;
            if (size == 1)
            {
                tilesX = 4200;
                tilesY = 1200;
            }
            else if (size == 2)
            {
                tilesX = 6400;
                tilesY = 1800;
            }
            else if (size == 3)
            {
                tilesX = 8400;
                tilesY = 2400;
            }
            if (tilesX > 0)
            {
                Main.maxTilesX = tilesX;
                Main.maxTilesY = tilesY;
                Main.ActiveWorldFileData.SetWorldSize(tilesX, tilesY);
            }
            WorldGen.WorldGenParam_Evil = evil;

            // 开始创建
            await AsyncGenerateWorld(Main.ActiveWorldFileData.Seed);
            isTaskRunning = false;

            // 创建完成
            int second = utils.GetUnixTimestamp - secondLast;
            string text = $"[i:3061]世界重建完成 （用时 {second}s, {WorldGen.currentWorldSeed}）；-）";
            TSPlayer.All.SendSuccessMessage(text);
            Console.WriteLine(text);


            // 传送到出生点 并同步视线内的图格
            foreach (TSPlayer plr in TShock.Players)
            {
                if (plr != null && Main.tile[plr.TileX, plr.TileY].active())
                {
                    plr.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48);
                    NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(plr.TPlayer.position, 1, 0, 10, -16));
                }
            }
            InformPlayers();
        }

        #region process seed
        private static void ProcessSpecialWorldSeeds(string processedSeed)
        {
            // UIWorldCreation.ProcessSpecialWorldSeeds(seedStr);
            WorldGen.notTheBees = false;
            WorldGen.getGoodWorldGen = false;
            WorldGen.tenthAnniversaryWorldGen = false;
            WorldGen.dontStarveWorldGen = false;
            ToggleSpecialWorld(processedSeed.ToLowerInvariant());
        }

        private static void ProcessEggSeeds(string seedstr)
        {
            string[] seeds = seedstr.ToLowerInvariant().Split(',');
            foreach (string newseed in seeds)
            {
                ToggleSpecialWorld(newseed);
            }
        }
        private static void ToggleSpecialWorld(string seedstr)
        {
            switch (seedstr)
            {
                case "2020":
                case "516":
                case "5162020":
                case "05162020":
                    Main.ActiveWorldFileData._seed = 5162020;
                    break;

                case "2021":
                case "5162011":
                case "5162021":
                case "05162011":
                case "05162021":
                case "celebrationmk10":
                    WorldGen.tenthAnniversaryWorldGen = true;
                    break;

                case "ntb":
                case "not the bees":
                case "not the bees!":
                    WorldGen.notTheBees = true;
                    break;

                case "ftw":
                case "for the worthy":
                    WorldGen.getGoodWorldGen = true;
                    break;

                case "dst":
                case "constant":
                case "theconstant":
                case "the constant":
                case "eye4aneye":
                case "eyeforaneye":
                    WorldGen.dontStarveWorldGen = true;
                    break;

                case "superegg":
                    Main.ActiveWorldFileData._seed = 5162020;

                    WorldGen.notTheBees = true;
                    WorldGen.getGoodWorldGen = true;
                    WorldGen.tenthAnniversaryWorldGen = true;
                    WorldGen.dontStarveWorldGen = true;
                    break;
            }
        }
        #endregion

        #region  gen
        private static Task AsyncGenerateWorld(int seed)
        {
            isTaskRunning = true;
            WorldGen.clearWorld();
            return Task.Run(() => WorldGen.GenerateWorld(seed)).ContinueWith((d) => FinishGen());
        }
        private static void FinishGen()
        {
            isTaskRunning = false;
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null)
                {
                    NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(player.TPlayer.position, 1, 0, 10, -16));
                }
            }
            TShock.Utils.SaveWorld();
        }

        //重载 玩家可视区域内的图格
        private static void InformPlayers()
        {
            foreach (TSPlayer person in TShock.Players)
            {
                if ((person != null) && (person.Active))
                {
                    for (int i = 0; i < 255; i++)
                    {
                        for (int j = 0; j < Main.maxSectionsX; j++)
                        {
                            for (int k = 0; k < Main.maxSectionsY; k++)
                            {
                                Netplay.Clients[i].TileSections[j, k] = false;
                            }
                        }
                    }
                }
            }

        }
        #endregion
    }
}