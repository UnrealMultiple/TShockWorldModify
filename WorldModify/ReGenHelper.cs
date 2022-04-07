using System;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;


namespace WorldModify
{
    class ReGenHelper
    {
        private static bool isTaskRunning = false;

        #region command
        public async static void iGen(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                op.SendInfoMessage("/igen world [种子] [腐化] [大小] [彩蛋特性], 重建地图");
                op.SendInfoMessage("/igen room <数量>，生成玻璃小房间（默认生成3个）");
                op.SendInfoMessage("/igen pond，生成玻璃鱼池");
                op.SendInfoMessage("/igen sm <w> <h>，盾构机");
                op.SendInfoMessage("/igen dig <w> <h>，钻井机");
                op.SendInfoMessage("/igen dirt，填土");
                op.SendInfoMessage("/igen clear，（测试）清空世界");
                op.SendInfoMessage("/igen info，（测试）当前物块信息");
                return;
            }

            bool isRight;
            int w;
            int h;
            int num;
            switch (args.Parameters[0].ToLowerInvariant())
            {
                // 玻璃小房间
                case "room":
                    if (!op.RealPlayer)
                    {
                        op.SendErrorMessage($"请进入游戏后再操作！");
                        return;
                    }
                    int total = 3;
                    if (args.Parameters.Count > 1)
                    {
                        if (!int.TryParse(args.Parameters[1], out total))
                        {
                            op.SendErrorMessage("输入的房间数量不对");
                            return;
                        }
                        if (total < 1 || total > 1000)
                        {
                            total = 3;
                        }
                    }
                    isRight = op.TPlayer.direction != -1;
                    int tryX = op.TileX;
                    int tryY = op.TileY + 4;
                    await (AsyncGenRoom(tryX, tryY, total, isRight, true));
                    return;

                // 鱼池
                case "pond":
                    if (!op.RealPlayer)
                    {
                        op.SendErrorMessage($"请进入游戏后再操作！");
                        return;
                    }
                    await (AsyncGenPond(op.TileX, op.TileY + 3));
                    break;

                // 盾构机
                case "shieldmachine":
                case "sm":
                    if (!op.RealPlayer)
                    {
                        op.SendErrorMessage($"请进入游戏后再操作！");
                        return;
                    }
                    isRight = op.TPlayer.direction != -1;
                    w = 100;
                    h = 10;
                    if (args.Parameters.Count > 1)
                    {
                        if (int.TryParse(args.Parameters[1], out num))
                            w = Math.Max(3, num);
                    }
                    if (args.Parameters.Count > 2)
                    {
                        if (int.TryParse(args.Parameters[2], out num))
                            h = Math.Max(3, num);
                    }
                    await (AsyncGenShieldMachine(op.TileX, op.TileY + 3, w, h, isRight));
                    break;


                // 挖掘机
                case "dig":
                    if (!op.RealPlayer)
                    {
                        op.SendErrorMessage($"请进入游戏后再操作！");
                        return;
                    }
                    isRight = op.TPlayer.direction != -1;
                    w = 3;
                    h = 100;
                    if (args.Parameters.Count > 1)
                    {
                        if (int.TryParse(args.Parameters[1], out num))
                            w = Math.Max(3, num);
                    }
                    if (args.Parameters.Count > 2)
                    {
                        if (int.TryParse(args.Parameters[2], out num))
                            h = Math.Max(3, num);
                    }
                    await (AsyncDigArea(op.TileX, op.TileY + 3, w, h, isRight));
                    break;

                // 填土
                case "dirt":
                    if (!op.RealPlayer)
                    {
                        op.SendErrorMessage($"请进入游戏后再操作！");
                        return;
                    }
                    await (AsyncPlaceDirt(op.TileX, op.TileY + 3));
                    return;


                // 重建世界
                case "world":
                case "w":
                    if (args.Parameters.Count == 1)
                    {
                        op.SendErrorMessage("参数不够，用法如下");
                        op.SendInfoMessage("/igen world <种子> [腐化] [大小] [彩蛋特性], 重建地图");
                        op.SendInfoMessage("种子：输入任意种子名，0表示随机");
                        op.SendInfoMessage("腐化：腐化/猩红 或 1/2, 0表示随机");
                        op.SendInfoMessage("大小：小/中/大 或 1/2/3, 0表示忽略");
                        op.SendInfoMessage("彩蛋特性：种子名中间输入英文逗号，例如 2020,ftw");
                        return;
                    }
                    string seedStr = args.Parameters.Count > 1 ? args.Parameters[1] : "";
                    string evilStr = args.Parameters.Count > 2 ? args.Parameters[2] : "";
                    string sizeStr = args.Parameters.Count > 3 ? args.Parameters[3] : "";

                    string eggStr = "";
                    if (args.Parameters.Count > 4)
                    {
                        args.Parameters.RemoveAt(0);
                        args.Parameters.RemoveAt(0);
                        args.Parameters.RemoveAt(0);
                        args.Parameters.RemoveAt(0);
                        eggStr = string.Join(" ", args.Parameters);
                    }

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

                    GenWorld(op, seedStr, size, evil, eggStr);
                    return;

                case "info":
                    if (!op.RealPlayer)
                    {
                        op.SendErrorMessage($"请进入游戏后再操作！");
                        return;
                    }
                    //OTAPI.Tile.ITileCollection 
                    int cx = op.TileX;
                    int cy = op.TileY + 3;
                    op.SendInfoMessage($"pos:{op.TileX},{op.TileY} || {op.TPlayer.position.X},{op.TPlayer.position.Y}");
                    op.SendInfoMessage($"type:{Main.tile[cx, cy].type}");
                    op.SendInfoMessage($"wall:{Main.tile[cx, cy].wall}");
                    op.SendInfoMessage($"frameX:{Main.tile[cx, cy].frameX}");
                    op.SendInfoMessage($"frameY:{Main.tile[cx, cy].frameY}");
                    op.SendInfoMessage($"blockType:{Main.tile[cx, cy].blockType()}");
                    op.SendInfoMessage($"slope:{Main.tile[cx, cy].slope()}");
                    break;
            }

        }
        #endregion


        #region GenWorld
        /// <summary>
        /// GenWorld
        /// 参考：https://github.com/Illuminousity/WorldRefill/blob/master/WorldRefill/WorldRefill.cs#L997
        /// </summary>
        /// <param name="op"></param>
        /// <param name="seedStr"></param>
        /// <param name="size"></param>
        /// <param name="evil"></param>
        /// <param name="eggStr"></param>
        private static async void GenWorld(TSPlayer op, string seedStr = "", int size = 0, int evil = -1, string eggStr = "")
        {
            if (isTaskRunning)
            {
                op.SendErrorMessage($"有创建任务在执行！");
                return;
            }

            BackupHelper.Backup(op);
            if (!op.RealPlayer)
            {
                Console.WriteLine($"seed:{seedStr}");
                op.SendErrorMessage($"[i:556]世界正在解体~");
            }
            TSPlayer.All.SendErrorMessage("[i:556]世界正在解体~");
            int secondLast = utils.GetUnixTimestamp;

            // 设置创建参数
            ProcessSeeds(seedStr);
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

            // 大小 腐化
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
            if (!op.RealPlayer)
                op.SendErrorMessage($"[i:3061]世界正在重建（{WorldGen.currentWorldSeed}）");
            TSPlayer.All.SendErrorMessage($"[i:3061]世界正在重建（{WorldGen.currentWorldSeed}）");
            await AsyncGenerateWorld(Main.ActiveWorldFileData.Seed);
            isTaskRunning = false;

            // 创建完成
            int second = utils.GetUnixTimestamp - secondLast;
            string text = $"[i:3061]世界重建完成 （用时 {second}s, {WorldGen.currentWorldSeed}）；-）";
            TSPlayer.All.SendSuccessMessage(text);
            if (!op.RealPlayer)
                op.SendErrorMessage(text);


            // 传送到出生点
            foreach (TSPlayer plr in TShock.Players)
            {
                if (plr != null && Main.tile[plr.TileX, plr.TileY].active())
                {
                    plr.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48);
                }
            }
            FinishGen();
            InformPlayers();
        }
        #endregion

        #region 处理种子
        /// <summary>
        /// 处理秘密世界种子
        /// </summary>
        /// <param name="seed"></param>
        private static void ProcessSeeds(string seed)
        {
            // UIWorldCreation.ProcessSpecialWorldSeeds(seedStr);
            WorldGen.notTheBees = false;
            WorldGen.getGoodWorldGen = false;
            WorldGen.tenthAnniversaryWorldGen = false;
            WorldGen.dontStarveWorldGen = false;
            ToggleSpecialWorld(seed.ToLowerInvariant());
        }

        /// <summary>
        /// 处理彩蛋
        /// </summary>
        /// <param name="seedstr">例如：2020,2021,ftw</param>
        private static void ProcessEggSeeds(string seedstr)
        {
            string[] seeds = seedstr.ToLowerInvariant().Split(',');
            foreach (string newseed in seeds)
            {
                ToggleSpecialWorld(newseed);
            }
        }
        /// <summary>
        /// 开关秘密世界（创建器的属性）
        /// </summary>
        /// <param name="seed"></param>
        private static void ToggleSpecialWorld(string seed)
        {
            switch (seed)
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

        #region  AsyncGenerateWorld
        private static Task AsyncGenerateWorld(int seed)
        {
            isTaskRunning = true;
            WorldGen.clearWorld();
            return Task.Run(() => WorldGen.GenerateWorld(seed)).ContinueWith((d) => FinishGen());
        }
        #endregion


        #region 创建完成后
        /// <summary>
        /// 完成创建
        /// </summary>
        private static void FinishGen()
        {
            isTaskRunning = false;
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && (player.Active))
                {
                    NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(player.TPlayer.position, 1, 0, 10, -16));
                }
            }
            TShock.Utils.SaveWorld();
        }
        // 更新物块
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

        #region 生成小房间
        public static Task AsyncGenRoom(int posX, int posY, int total = 1, bool isRight = true, bool needCenter = false)
        {
            isTaskRunning = true;
            return Task.Run(() => GenRooms(posX, posY, total, isRight, needCenter)).ContinueWith((d) => FinishGen());
        }

        public static void GenRooms(int posX, int posY, int total, bool isRight = true, bool needCenter = false)
        {
            int w = 5;
            int roomWidth = 1 + total * w;

            int startX = needCenter ? posX - (roomWidth / 2) : posX;
            Console.WriteLine($"npctotal:{total} posX:{posX} posY:{posY} startX:{startX}");
            for (int i = 0; i < total; i++)
            {
                GenRoom(startX, posY, isRight);
                startX += w;
            }
        }

        public static void GenRoom(int posX, int posY, bool isRight = true)
        {
            RoomTheme th = RoomTheme.GetGlass();

            ushort tile = th.tile;
            ushort wall = th.wall;
            TileInfo platform = th.platform;
            TileInfo chair = th.chair;
            TileInfo bench = th.bench;
            TileInfo torch = th.torch;

            int Xstart = posX;
            int Ystart = posY;
            int Width = 6;
            int height = 10;

            if (!isRight)
                Xstart += 2;

            Parallel.For(Xstart, Xstart + Width, (cx) =>
            {
                Parallel.For(Ystart - height, Ystart, (cy) =>
                {
                    // 清空区域
                    Main.tile[cx, cy].ClearEverything();

                    // 墙
                    if ((cx > Xstart) && (cy < Ystart - 5) && (cx < Xstart + Width - 1) && (cy > Ystart - height))
                    {
                        Main.tile[cx, cy].wall = wall;
                    }


                    if ((cx == Xstart && cy > Ystart - 5)
                    || (cx == Xstart + Width - 1 && cy > Ystart - 5)
                    || (cy == Ystart - 1))
                    {
                        // 平台
                        WorldGen.PlaceTile(cx, cy, platform.type, false, true, -1, platform.style);
                    }

                    else if ((cx == Xstart) || (cx == Xstart + Width - 1)
                    || (cy == Ystart - height)
                    || (cy == Ystart - 5))
                    {
                        // 方块
                        Main.tile[cx, cy].type = tile;
                        Main.tile[cx, cy].active(true);
                        Main.tile[cx, cy].slope(0);
                        Main.tile[cx, cy].halfBrick(false);
                    }
                });
            });

            if (isRight)
            {
                // 椅子
                WorldGen.PlaceTile(Xstart + 1, Ystart - 6, chair.type, false, true, 0, chair.style);
                Main.tile[Xstart + 1, posY - 6].frameX += 18;
                Main.tile[Xstart + 1, posY - 7].frameX += 18;

                // 工作台
                WorldGen.PlaceTile(Xstart + 2, Ystart - 6, bench.type, false, true, -1, bench.style);

                // 火把
                WorldGen.PlaceTile(Xstart + 4, Ystart - 5, torch.type, false, true, -1, torch.style);
            }
            else
            {
                WorldGen.PlaceTile(Xstart + 4, Ystart - 6, chair.type, false, true, 0, chair.style);
                WorldGen.PlaceTile(Xstart + 2, Ystart - 6, bench.type, false, true, -1, bench.style);
                WorldGen.PlaceTile(Xstart + 1, Ystart - 5, torch.type, false, true, -1, torch.style);
            }

            InformPlayers();
        }

        #endregion

        #region 盾构机
        public static Task AsyncGenShieldMachine(int posX, int posY, int w, int h, bool isRight = true)
        {
            isTaskRunning = true;
            return Task.Run(() => GenShieldMachine(posX, posY, w, h, isRight)).ContinueWith((d) => FinishGen());
        }
        public static void GenShieldMachine(int posX, int posY, int w, int h, bool isRight = true)
        {
            int Xstart = posX;
            int Xend;
            int Ystart = posY;
            int Width = Math.Max(3, w);
            int height = Math.Max(3, h);

            if (isRight)
            {
                Xend = Xstart + Width;
            }
            else
            {
                Xend = Xstart + 2;
                Xstart -= Width;
            }

            Parallel.For(Xstart, Xend, (cx) =>
            {
                Parallel.For(Ystart - height, Ystart, (cy) =>
                {
                    if (Main.tile[cx, cy].active())
                        WorldGen.KillTile(cx, cy, false, false, true);
                });

                WorldGen.PlaceTile(cx, Ystart, 19, false, true, -1, 43);
            });


            InformPlayers();
        }
        #endregion

        #region 挖掘机
        public static Task AsyncDigArea(int posX, int posY, int w, int h, bool isRight = true, bool isHell = false)
        {

            isTaskRunning = true;
            return Task.Run(() => DigArea(posX, posY, w, h, isRight, isHell)).ContinueWith((d) => FinishGen());
        }
        public static void DigArea(int posX, int posY, int w, int h, bool isRight = true, bool isHell = false)
        {
            int Width = Math.Max(3, w);
            int height = Math.Max(3, h);
            int Xstart = posX - (Width / 2);
            int Ystart = posY;

            Parallel.For(Xstart, Xstart + Width, (cx) =>
            {
                Parallel.For(Ystart, Ystart + height, (cy) =>
                {
                    if (Main.tile[cx, cy].active())
                        WorldGen.KillTile(cx, cy, false, false, true);

                    if (cx == posX)
                    {
                        Main.tile[cx, cy].type = TileID.SilkRope;
                        Main.tile[cx, cy].active(true);
                        Main.tile[cx, cy].slope(0);
                        Main.tile[cx, cy].halfBrick(false);
                    }
                });

                WorldGen.PlaceTile(cx, Ystart, 19, false, true, -1, 43);
            });

            InformPlayers();
        }
        #endregion

        #region 生成鱼池
        public static Task AsyncGenPond(int posX, int posY)
        {
            isTaskRunning = true;
            return Task.Run(() => GenPond(posX, posY)).ContinueWith((d) => FinishGen());
        }
        public static void GenPond(int posX, int posY)
        {
            RoomTheme th = RoomTheme.GetGlass();

            ushort tile = th.tile;
            ushort wall = th.wall;
            TileInfo platform = th.platform;
            TileInfo chair = th.chair;
            TileInfo bench = th.bench;
            TileInfo torch = th.torch;

            int Xstart = posX - 6;
            int Ystart = posY;
            int Width = 11 + 4;
            int height = 30 + 2;

            Parallel.For(Xstart, Xstart + Width, (cx) =>
            {
                Parallel.For(Ystart, Ystart + height, (cy) =>
                {
                    if (Main.tile[cx, cy].active())
                    {
                        WorldGen.KillTile(cx, cy, false, false, true);
                    }

                    if ((cx == Xstart) || (cx == Xstart + 1) || (cx == Xstart + Width - 1) || (cx == Xstart + Width - 2)
                    || (cy == Ystart + height - 1) || (cy == Ystart + height - 2))
                    {
                        // 方块
                        Main.tile[cx, cy].type = tile;
                        Main.tile[cx, cy].active(true);
                        Main.tile[cx, cy].slope(0);
                        Main.tile[cx, cy].halfBrick(false);
                    }
                });

                WorldGen.PlaceTile(cx, Ystart, platform.type, false, true, -1, platform.style);
            });

            InformPlayers();
        }
        #endregion

        #region 填土
        public static Task AsyncPlaceDirt(int posX, int posY)
        {
            isTaskRunning = true;
            return Task.Run(() => PlaceDirt(posX, posY)).ContinueWith((d) => FinishGen());
        }
        public static void PlaceDirt(int posX, int posY)
        {
            int Width = 122;
            int height = 68;
            int Xstart = posX - 60;
            int Ystart = posY;

            Parallel.For(Xstart, Xstart + Width, (cx) =>
            {
                Parallel.For(Ystart - height, Ystart, (cy) =>
                {
                    Main.tile[cx, cy].ClearEverything();
                });

                Parallel.For(Ystart, Ystart + height, (cy) =>
                {
                    Main.tile[cx, cy].type = TileID.Dirt;
                    Main.tile[cx, cy].active(true);
                    Main.tile[cx, cy].slope(0);
                    Main.tile[cx, cy].halfBrick(false);
                });

            });

            InformPlayers();
        }
        #endregion

        #region 清空世界
        public static void ClearWorld()
        {
            // WorldGen.clearWorld();
            if (Main.netMode != 1)
            {
                for (int l = 0; l < Main.maxTilesX; l++)
                {
                    //float num2 = (float)l / (float)Main.maxTilesX;
                    // Main.statusText = Lang.gen[47].Value + " " + (int)(num2 * 100f + 1f) + "%";
                    for (int m = 0; m < Main.maxTilesY; m++)
                    {
                        if (Main.tile[l, m] == null)
                        {
                            Main.tile[l, m] = new Tile();
                        }
                        else
                        {
                            Main.tile[l, m].ClearEverything();
                        }
                    }
                }
            }
            InformPlayers();
            TSPlayer.All.SendErrorMessage("世界已清空");
        }
        #endregion

    }
}