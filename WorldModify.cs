using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.GameContent.Creative;


namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "hufang360";

        public override string Description => "简易的世界修改器";

        public override string Name => "WorldModify";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(new List<string>() {"worldmodify"}, WorldModify, "worldmodify", "wm") { HelpText = "简易的世界修改器"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"moonphase"}, ChangeMoonPhase, "moonphase", "mp", "moon") { HelpText = "月相管理"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"moonstyle"}, ChangeMoonStyle, "moonstyle", "ms") { HelpText = "月亮样式管理"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"bossmanage"}, BossManage, "bossmanage", "bm", "boss" ) { HelpText = "boss管理"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"npcmanage"}, NpcManage, "npcmanage", "nm", "npc") { HelpText = "npc管理"});
            Commands.ChatCommands.Add(new Command(new List<string>() {""}, BossInfo, "bossinfo", "bi") { HelpText = "boss进度信息"});
        }


        private void WorldModify(CommandArgs args)
        {
            void ShowHelpText()
            {
                args.Player.SendInfoMessage("/wm info，查看世界信息");
                args.Player.SendInfoMessage("/wm name <世界名>，修改世界名字");
                args.Player.SendInfoMessage("/wm id <id>，修改世界ID");
                args.Player.SendInfoMessage("/wm seed <种子>，修改世界种子");
                args.Player.SendInfoMessage("/wm 0516，开启/关闭 05162020 秘密世界");
                args.Player.SendInfoMessage("/wm 05162021，开启/关闭 05162021 秘密世界");
                args.Player.SendInfoMessage("/wm ftw，开启/关闭 for the worthy 秘密世界");
                args.Player.SendInfoMessage("/wm dst，开启/关闭 永恒领域 秘密世界（饥荒联动）");
                args.Player.SendInfoMessage("/wm research, 解锁全物品研究");
                args.Player.SendInfoMessage("/moon help,  月相管理");
                args.Player.SendInfoMessage("/moonstyle help,  月亮样式管理");
                args.Player.SendInfoMessage("/boss help, boss管理");
                args.Player.SendInfoMessage("/npc help, npc管理");
            }

            if (args.Parameters.Count<string>() == 0)
            {
                args.Player.SendErrorMessage("语法错误，/wm help 可查询相关用法");
                return;
            }


            switch (args.Parameters[0].ToLowerInvariant())
            {
                // 帮助
                case "help":
                    ShowHelpText();
                    return;

                default:
                    args.Player.SendErrorMessage("语法不正确！");
                    break;

                // 世界信息
                case "info":
                    args.Player.SendInfoMessage("当前世界信息");
                    args.Player.SendInfoMessage("名字: {0}", Main.worldName);
                    args.Player.SendInfoMessage("大小: {0}（{1}x{2}）", Main.ActiveWorldFileData.WorldSizeName, Main.maxTilesX, Main.maxTilesY);
                    args.Player.SendInfoMessage("ID: {0}", Main.worldID);
                    args.Player.SendInfoMessage("难度: {0}", _worldModes.Keys.ElementAt(Main.GameMode));
                    args.Player.SendInfoMessage("种子: {0}（{1}）", WorldGen.currentWorldSeed, Main.ActiveWorldFileData.GetFullSeedText());

                    if(this.GetSecretWorldDescription()!="")
                        args.Player.SendInfoMessage(this.GetSecretWorldDescription());

                    args.Player.SendInfoMessage(this.GetCorruptionDescription());
                    args.Player.SendInfoMessage("困难模式: {0}", (Main.ActiveWorldFileData.IsHardMode ? "是" : "否"));
                    args.Player.SendInfoMessage("月相: {0}", _moonPhases.Keys.ElementAt(Main.moonPhase));
                    args.Player.SendInfoMessage("月亮样式: {0}", _moonTypes.Keys.ElementAt(Main.moonType));
                    break;


                // 名字
                case "name":
                    Main.worldName = args.Parameters[1];
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    args.Player.SendSuccessMessage("世界的名字已改成 {0}", args.Parameters[1]);
                    break;


                // 种子
                case "seed":
                    Main.ActiveWorldFileData.SetSeed(args.Parameters[1]);
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    args.Player.SendSuccessMessage("世界的种子已改成 {0}", args.Parameters[1]);
                    break;


                // worldId 800875906
                case "id":
                    int worldId;
                    if ( int.TryParse(args.Parameters[1],out worldId) )
                    {
                        Main.worldID = worldId;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("世界的ID已改成 {0}", args.Parameters[1]);
                    } else {
                        args.Player.SendErrorMessage("世界ID只能由数字组成");
                    }
                    break;


                // 醉酒世界
                case "0516":
                case "05162020":
                case "516":
                case "5162020":
                case "9":
                case "2020":
                case "drunk":
                    if (Main.drunkWorld) {
                        Main.drunkWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已关闭 05162020 秘密世界（醉酒世界 / DrunkWorld）");
                    } else {
                        Main.drunkWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已开启 05162020 秘密世界（醉酒世界 / DrunkWorld）");
                    }
                    break;
                

                // 10周年庆典,tenthAnniversaryWorld
                case "5162021":
                case "05162021":
                case "5162011":
                case "05162011":
                case "10":
                case "2011":
                case "2021":
                case "celebrationmk10":
                    if (Main.tenthAnniversaryWorld) {
                        Main.tenthAnniversaryWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已关闭 05162021 秘密世界（10周年庆典）");
                    } else {
                        Main.tenthAnniversaryWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已开启 05162021 秘密世界（10周年庆典）");
                    }
                    break;


                // ftw
                case "ftw":
                case "for the worthy":
                    if (Main.getGoodWorld) {
                        Main.getGoodWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已关闭 for the worthy 秘密世界");
                    } else  {
                        Main.getGoodWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已开启 for the worthy 秘密世界");
                    }
                    break;
                

                //  饥荒联动
                case "eye":
                case "dst":
                case "constant":
                    if (Main.dontStarveWorld) {
                        Main.dontStarveWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已关闭 永恒领域 秘密世界（饥荒联动）");
                    } else {
                        Main.dontStarveWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已开启 永恒领域 秘密世界（饥荒联动）");
                    }
                    break;


                // 全物品研究（有待测试）
                case "research":
                    List<int> ids = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys.ToList<int>();

                    if( ids.Count>0 ){
                        args.Player.SendInfoMessage("正在解锁，需要一点时间，请稍等……");
                    }

                    int amountNeeded;
                    for (int i = 0; i < ids.Count; i++)
                    {
                        CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(ids[i], out amountNeeded);
                        TShock.ResearchDatastore.SacrificeItem(ids[i], amountNeeded, args.Player);
                    }
                    args.Player.SendSuccessMessage("已解锁 {0} 个物品研究，重新开服可生效", ids.Count);
                    args.Player.SendSuccessMessage("研究数据仅保存在服务器上，每张地图的研究数据是分开的");
                    TSPlayer.All.SendData(PacketTypes.PlayerInfo);
                    break;

            }
        }



        // 修改月相
        private void ChangeMoonPhase(CommandArgs args)
        {
            if(args.Parameters.Count<string>()==0 || args.Parameters[0].ToLowerInvariant()=="help")
            {
                args.Player.SendInfoMessage("当前月相: {0}", _moonPhases.Keys.ElementAt(Main.moonPhase));
                args.Player.SendInfoMessage("用法：/moon <月相>");
                args.Player.SendInfoMessage("月相：{0} （可用数字 1~8 代替）", String.Join(", ", _moonPhases.Keys));
                return;
            }

            int moon;
            if (int.TryParse(args.Parameters[0], out moon))
            {
                if (moon < 1 || moon > 8)
                {
                    args.Player.SendErrorMessage("语法错误！用法：/moon <月相>");
                    args.Player.SendErrorMessage("月相：{0} （可用数字 1~8 代替）", String.Join(", ", _moonPhases.Keys));
                    return;
                }
            }
            else if (_moonPhases.ContainsKey(args.Parameters[0]))
            {
                moon = _moonPhases[args.Parameters[0]];
            }
            else
            {
                args.Player.SendErrorMessage("语法错误！用法：/moon <月相>");
                args.Player.SendErrorMessage("月相：{0} （可用数字 1~8 代替）", String.Join(", ", _moonPhases.Keys));
                return;
            }

            Main.dayTime = false;
            Main.moonPhase = moon-1;
            Main.time = 0.0;
            TSPlayer.All.SendData(PacketTypes.WorldInfo);
            args.Player.SendSuccessMessage("月相已改为 {0}", _moonPhases.Keys.ElementAt(moon-1));
        }

        // 修改月亮样式
        private void ChangeMoonStyle(CommandArgs args)
        {
            if(args.Parameters.Count<string>()==0){
                args.Player.SendInfoMessage("当前月亮样式: {0}", _moonTypes.Keys.ElementAt(Main.moonType));
                args.Player.SendInfoMessage("用法：/moonstyle <月亮样式>");
                args.Player.SendInfoMessage("月亮样式：{0} （可用数字 1~9 代替）", String.Join(", ", _moonTypes.Keys));
                return;
            }

            if( args.Parameters[0].ToLowerInvariant() ==  "help" ){
                args.Player.SendInfoMessage("用法：/moonstyle <月亮样式>");
                args.Player.SendInfoMessage("月亮样式：{0} （可用数字 1~9 代替）", String.Join(", ", _moonTypes.Keys));
                return;
            }

            int moontype;
            if (int.TryParse(args.Parameters[0], out moontype))
            {
                if (moontype < 1 || moontype > 9)
                {
                    args.Player.SendErrorMessage("语法错误！用法：/moonstyle <月亮样式>");
                    args.Player.SendErrorMessage("月亮样式：{0} （可用数字 1~9 代替）", String.Join(", ", _moonTypes.Keys));
                    return;
                }
            }
            else if (_moonTypes.ContainsKey(args.Parameters[0]))
            {
                moontype = _moonTypes[args.Parameters[0]];
            }
            else
            {
                args.Player.SendErrorMessage("语法错误！用法：/moonstyle <月亮样式>");
                args.Player.SendErrorMessage("月亮样式：{0} （可用数字 1~9 代替）", String.Join(", ", _moonTypes.Keys));
                return;
            }
            Main.dayTime = false;
            Main.moonType = moontype-1;
            Main.time = 0.0;
            TSPlayer.All.SendData(PacketTypes.WorldInfo);
            args.Player.SendSuccessMessage("月亮样式已改为 {0}", _moonTypes.Keys.ElementAt(moontype-1));
        }

        # region Boss 管理
             
        /// <summary>
        /// BOSS管理
        /// </summary>
        private void BossManage(CommandArgs args)
        {
            if(args.Parameters.Count<string>()==0 || args.Parameters[0].ToLowerInvariant()=="help")
            {
                args.Player.SendInfoMessage("/boss info, 查看boss进度");
                args.Player.SendInfoMessage("/boss sb, sb 召唤指令备注（SpawnBoss boss召唤指令）");
                args.Player.SendInfoMessage("/boss toggle <boss名>, 切换boss击败状态");

                return;
            }


            switch (args.Parameters[0].ToLowerInvariant())
            {
                default:
                    args.Player.SendErrorMessage("语法不正确！");
                    break;


                // boss
                case "sb":
                case "spawn":
                    string [] part1 = {
                        "\"king slime\" (史莱姆王)",
                        "\"eye of cthulhu\" (克苏鲁之眼)",
                        "deerclops (鹿角怪)",
                        "\"brain of cthulhu\" (克苏鲁之脑)",
                        "\"eater of worlds\" (世界吞噬怪)",
                        "\"queen bee\" (蜂王)",
                        "skeletron (骷髅王)",
                        "\"wall of flesh\" (血肉墙)"
                    };

                    string [] part2 = {
                        "\"queen slime\" (史莱姆皇后)",
                        "prime (机械骷髅王)",
                        "twins (双子魔眼)",
                        "destroyer (毁灭者)",
                        "plantera (世纪之花)",
                        "golem (石巨人)",
                        "\"empress of light\" (光之女皇)",
                        "\"duke fishron\" (猪龙鱼公爵)",
                        "\"lunatic cultist\" (拜月教邪教徒)",
                        "\"moon lord\" (月亮领主)"
                    };

                    string [] part3 = {
                        "\"mourning wood\" (哀木)",
                        "pumpking (南瓜王)",
                        "everscream (常绿尖叫怪)",
                        "\"ice queen\" (冰雪女王)",
                        "santa (圣诞坦克)",
                        "\"flying dutchman\" (荷兰飞盗船)",
                        "\"martian saucer\" (火星飞碟)",
                        "betsy (双足翼龙)",
                        "\"solar pillar\" (日耀柱)",
                        "\"vortex pillar\" (星旋柱)",
                        "\"nebula pillar\" (星云柱)",
                        "\"stardust pillar\" (星尘柱)"
                    };
                    args.Player.SendInfoMessage("以下是 boss 生成指令, sb = spawnboss：");
                    args.Player.SendInfoMessage("/sb " + String.Join(", /sb ", part1));
                    args.Player.SendInfoMessage("/sb " + String.Join(", /sb ", part2));
                    args.Player.SendInfoMessage("/sb " + String.Join(", /sb ", part3));
                    break;
                                

                // 查看boss进度
                case "info":
                    BossInfo(args);
                    break;
                

                // 切换boss击败状态
                case "toggle":
                    ToggleBoss(args);
                    break;

            }

        }

        /// <summary>
        /// 切换BOSS击败状态
        /// </summary>
        private void ToggleBoss(CommandArgs args)
        {
            if (args.Parameters.Count < 2){
                args.Player.SendInfoMessage("语法不正确 /boss toggle <boss名>");
                return;
            }

            switch ( args.Parameters[1].ToLowerInvariant() )
            {
                default:
                    args.Player.SendErrorMessage("语法不正确！，请使用 /boss toggle help, 进行查询");
                    break;
                
                case "help":
                    string [] li1 = {
                        "史莱姆王",
                        "克苏鲁之眼",
                        "鹿角怪",
                        "世界吞噬怪",
                        "克苏鲁之脑",
                        "蜂王",
                        "骷髅王",
                        "血肉墙"
                    };

                    string [] li2 = {    
                        "毁灭者",
                        "双子魔眼",
                        "机械骷髅王",
                        "世纪之花",
                        "石巨人",
                        "史莱姆皇后",
                        "光之女皇",
                        "猪龙鱼公爵",
                        "拜月教邪教徒",
                        "月亮领主"
                    };
                    
                    string [] li3 = { 
                        "小丑",
                        "哥布林军队",
                        "海盗入侵",
                        "火星暴乱",
                        "哀木",
                        "南瓜王",
                        "冰雪女王",
                        "常绿尖叫怪",
                        "圣诞坦克",
                        "日耀柱",
                        "星旋柱",
                        "星云柱",
                        "星尘柱"
                    };

                    args.Player.SendInfoMessage("支持切换的BOSS击败状态的有");
                    args.Player.SendInfoMessage("肉前：{0}", String.Join(", ", li1));
                    args.Player.SendInfoMessage("肉后：{0}", String.Join(", ", li2));
                    args.Player.SendInfoMessage("事件：{0}", String.Join(", ", li3));
                    break;

                // 史莱姆王
                case "史莱姆王":
                case "史莱姆国王":
                case "king slime":
                case "king":
                case "ks":
                    NPC.downedSlimeKing = !NPC.downedSlimeKing;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedSlimeKing)
                        args.Player.SendSuccessMessage("已标记 史莱姆王 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 史莱姆王 为未击败");
                    break;


                //  鹿角怪
                case "鹿角怪":
                case "deerclops":
                case "deer":
                case "独眼巨鹿":
                case "巨鹿":
                    NPC.downedDeerclops = !NPC.downedDeerclops;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedDeerclops)
                        args.Player.SendSuccessMessage("已标记 鹿角怪 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 鹿角怪 为未击败");
                    break;


                // 克苏鲁之眼
                case "克苏鲁之眼":
                case "克眼":
                case "eye of cthulhu":
                case "eye":
                case "eoc":
                    NPC.downedBoss1 = !NPC.downedBoss1;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedBoss1)
                        args.Player.SendSuccessMessage("已标记 克苏鲁之眼 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 克苏鲁之眼 为未击败");
                    break;


                // 世界吞噬怪 或 克苏鲁之脑
                case "世界吞噬怪":
                case "世吞":
                case "黑长直":
                case "克苏鲁之脑":
                case "克脑":
                case "brain of cthulhu":
                case "boc":
                case "brain":
                case "eater of worlds":
                case "eow":
                case "eater":
                case "boss2":
                    NPC.downedBoss2 = !NPC.downedBoss2;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    string boss2Name2 = "";
                    if(Main.ActiveWorldFileData.HasCrimson && Main.ActiveWorldFileData.HasCorruption)
                        boss2Name2 = "世界吞噬怪 或 克苏鲁之脑";
                    else if(Main.ActiveWorldFileData.HasCrimson)
                        boss2Name2 = "克苏鲁之脑";
                    else if(Main.ActiveWorldFileData.HasCorruption)
                        boss2Name2 = "世界吞噬怪";

                    if (NPC.downedBoss1)
                        args.Player.SendSuccessMessage("已标记 {0} 为已击败", boss2Name2);
                    else
                        args.Player.SendSuccessMessage("已标记 {0} 为未击败", boss2Name2);
                    break;


                // 骷髅王
                case "骷髅王":
                case "skeletron":
                case "boss3":
                    NPC.downedBoss3 = !NPC.downedBoss3;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedBoss3)
                        args.Player.SendSuccessMessage("已标记 骷髅王 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 骷髅王 为未击败");
                    break;


                // 蜂王
                case "蜂王":
                case "蜂后":
                case "queen bee":
                case "qb":
                    NPC.downedQueenBee = !NPC.downedQueenBee;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedQueenBee)
                        args.Player.SendSuccessMessage("已标记 蜂王 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 蜂王 为未击败");
                    break;


                // 血肉墙
                case "血肉墙":
                case "血肉之墙":
                case "肉山":
                case "wall of flesh":
                case "wof":
                    if (Main.hardMode)
                    {
                        Main.hardMode = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        args.Player.SendSuccessMessage("已标记 血肉墙 为未击败（困难模式 已关闭）");
                    }
                    else if (!TShock.Config.Settings.DisableHardmode)
                    {
                        WorldGen.StartHardmode();
                        args.Player.SendSuccessMessage("已标记 血肉墙 为已击败（困难模式 已开启）");
                    }
                    break;


                // 毁灭者
                case "毁灭者":
                case "铁长直":
                case "destroyer":
                    NPC.downedMechBoss1 = !NPC.downedMechBoss1;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedMechBoss1)
                        args.Player.SendSuccessMessage("已标记 毁灭者 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 毁灭者 为未击败");
                    break;


                // 双子魔眼
                case "双子魔眼":
                case "twins":
                    NPC.downedMechBoss2 = !NPC.downedMechBoss2;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedMechBoss2)
                        args.Player.SendSuccessMessage("已标记 双子魔眼 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 双子魔眼 为未击败");
                    break;


                // 机械骷髅王
                case "机械骷髅王":
                case "skeletron prime":
                case "prime":
                    NPC.downedMechBoss3 = !NPC.downedMechBoss3;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedMechBoss3)
                        args.Player.SendSuccessMessage("已标记 机械骷髅王 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 机械骷髅王 为未击败");
                    break;


                // 世纪之花
                case "世纪之花":
                case "plantera":
                    NPC.downedPlantBoss = !NPC.downedPlantBoss;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedPlantBoss)
                        args.Player.SendSuccessMessage("已标记 世纪之花 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 世纪之花 为未击败");
                    break;


                // 石巨人
                case "石巨人":
                case "golem":
                    NPC.downedGolemBoss = !NPC.downedGolemBoss;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedGolemBoss)
                        args.Player.SendSuccessMessage("已标记 石巨人 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 石巨人 为未击败");
                    break;


                // 史莱姆皇后
                case "史莱姆皇后":
                case "史莱姆女王":
                case "史莱姆王后":
                case "queen slime":
                case "qs":
                    NPC.downedQueenSlime = !NPC.downedQueenSlime;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedQueenSlime)
                        args.Player.SendSuccessMessage("已标记 史莱姆皇后 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 史莱姆皇后 为未击败");
                    break;


                // 光之女皇
                case "光之女皇":
                case "光女":
                case "光之女神":
                case "光之皇后":
                case "empress of light":
                case "empress":
                case "eol":
                    NPC.downedEmpressOfLight = !NPC.downedEmpressOfLight;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedEmpressOfLight)
                        args.Player.SendSuccessMessage("已标记 光之女皇 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 光之女皇 为未击败");
                    break;

                
                // 猪龙鱼公爵
                case "猪龙鱼公爵":
                case "猪鲨":
                case "duke fishron":
                case "duke":
                case "fishron":
                    NPC.downedFishron = !NPC.downedFishron;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedFishron)
                        args.Player.SendSuccessMessage("已标记 猪龙鱼公爵 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 猪龙鱼公爵 为未击败");
                    break;



                // 拜月教邪教徒
                case "拜月教邪教徒":
                case "拜月教":
                case "邪教徒":
                case "lunatic cultist":
                case "lunatic":
                case "cultist":
                case "lc":
                    NPC.downedAncientCultist = !NPC.downedAncientCultist;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedAncientCultist)
                        args.Player.SendSuccessMessage("已标记 拜月教邪教徒 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 拜月教邪教徒 为未击败");
                    break;


                // 月亮领主
                case "月亮领主":
                case "月总":
                case "moon lord":
                case "moon":
                case "ml":
                    NPC.downedMoonlord = !NPC.downedMoonlord;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedMoonlord)
                        args.Player.SendSuccessMessage("已标记 月亮领主 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 月亮领主 为未击败");
                    break;


                // 小丑
                case "小丑":
                case "clown":
                    NPC.downedClown = !NPC.downedClown;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedClown)
                        args.Player.SendSuccessMessage("已标记 小丑 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 小丑 为未击败");
                    break;


                // 哥布林军队
                case "哥布林军队":
                case "哥布林":
                case "goblin":
                case "goblins":
                    NPC.downedGoblins = !NPC.downedGoblins;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedGoblins)
                        args.Player.SendSuccessMessage("已标记 哥布林军队 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 哥布林军队 为未击败");
                    break;


                // 海盗入侵
                case "海盗入侵":
                case "荷兰飞盗船":
                case "海盗船":
                case "pirate":
                case "pirates":
                case "flying dutchman":
                case "flying":
                case "dutchman":
                    NPC.downedPirates = !NPC.downedPirates;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedPirates)
                        args.Player.SendSuccessMessage("已标记 海盗入侵 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 海盗入侵 为未击败");
                    break;


                // 火星暴乱
                case "火星暴乱":
                case "火星人入侵":
                case "火星飞碟":
                case "ufo":
                case "martian saucer":
                case "martian":
                case "martians":
                    NPC.downedMartians = !NPC.downedMartians;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedMartians)
                        args.Player.SendSuccessMessage("已标记 火星暴乱 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 火星暴乱 为未击败");
                    break;


                // 哀木
                case "哀木":
                case "mourning wood":
                case "wood":
                case "halloween tree":
                case "ht":
                    NPC.downedHalloweenTree = !NPC.downedHalloweenTree;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedHalloweenTree)
                        args.Player.SendSuccessMessage("已标记 哀木 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 哀木 为未击败");
                    break;


                // 南瓜王
                case "南瓜王":
                case "pumpking":
                case "halloween king":
                case "hk":
                    NPC.downedHalloweenKing = !NPC.downedHalloweenKing;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedHalloweenKing)
                        args.Player.SendSuccessMessage("已标记 南瓜王 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 南瓜王 为未击败");
                    break;


                // 冰雪女王
                case "冰雪女王":
                case "冰雪皇后":
                case "ice queen":
                    NPC.downedChristmasIceQueen = !NPC.downedChristmasIceQueen;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedChristmasIceQueen)
                        args.Player.SendSuccessMessage("已标记 冰雪女王 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 冰雪女王 为未击败");
                    break;


                // 常绿尖叫怪
                case "常绿尖叫怪":
                case "everscream":
                    NPC.downedChristmasTree = !NPC.downedChristmasTree;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedChristmasTree)
                        args.Player.SendSuccessMessage("已标记 常绿尖叫怪 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 常绿尖叫怪 为未击败");
                    break;


                // 圣诞坦克
                case "圣诞坦克":
                case "santa":
                case "santa-nk1":
                case "tank":
                    NPC.downedChristmasSantank = !NPC.downedChristmasSantank;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedChristmasSantank)
                        args.Player.SendSuccessMessage("已标记 圣诞坦克 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 圣诞坦克 为未击败");
                    break;


                // 日耀柱
                case "日耀柱":
                case "日耀":
                case "日曜柱":
                case "日曜":
                case "solar pillar":
                case "solar":
                    NPC.downedTowerSolar = !NPC.downedTowerSolar;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedTowerSolar)
                        args.Player.SendSuccessMessage("已标记 日曜柱 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 日曜柱 为未击败");
                    break;


                // 星旋柱
                case "星旋柱":
                case "星旋":
                case "vortex pillar":
                case "vortex":
                    NPC.downedTowerVortex = !NPC.downedTowerVortex;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedTowerVortex)
                        args.Player.SendSuccessMessage("已标记 星旋柱 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 星旋柱 为未击败");
                    break;


                // 星云柱
                case "星云柱":
                case "星云":
                case "nebula pillar":
                case "nebula":
                    NPC.downedTowerNebula = !NPC.downedTowerNebula;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedTowerNebula)
                        args.Player.SendSuccessMessage("已标记 星云柱 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 星云柱 为未击败");
                    break;


                // 星尘柱
                case "星尘柱":
                case "星尘":
                case "stardust pillar":
                case "stardust":
                    NPC.downedTowerStardust = !NPC.downedTowerStardust;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.downedTowerStardust)
                        args.Player.SendSuccessMessage("已标记 星尘柱 为已击败");
                    else
                        args.Player.SendSuccessMessage("已标记 星尘柱 为未击败");
                    break;


                // 双足翼龙
                case "betsy":
                    args.Player.SendErrorMessage("暂时不支持标记 双足翼龙");
                    break;
            }
        }

        private void BossInfo(CommandArgs args)
        {
            string CFlag(bool foo){
                if (foo)
                    return "✔";
                else
                    return "-";
            }

            List<string> li = new List<string>();
            List<string> li1 = new List<string>();
            List<string> li2 = new List<string>();
            List<string> li3 = new List<string>();

            li1.Add(CFlag(NPC.downedSlimeKing) + "史莱姆王");
            li1.Add(CFlag(NPC.downedBoss1) + "克苏鲁之眼");
            li1.Add(CFlag(NPC.downedDeerclops ) + "鹿角怪");

            string boss2Name = "";
            if(Main.ActiveWorldFileData.HasCrimson && Main.ActiveWorldFileData.HasCorruption)
                boss2Name = "世界吞噬怪 或 克苏鲁之脑";
            else if(Main.ActiveWorldFileData.HasCrimson)
                boss2Name = "克苏鲁之脑";
            else if(Main.ActiveWorldFileData.HasCorruption)
                boss2Name = "世界吞噬怪";
            li1.Add(CFlag(NPC.downedBoss2) + boss2Name);

            li1.Add(CFlag(NPC.downedBoss3) + "骷髅王");
            li1.Add(CFlag(NPC.downedQueenBee) + "蜂王");
            li1.Add(CFlag(Main.hardMode) + "血肉墙");


            li2.Add(CFlag(NPC.downedMechBoss1) + "毁灭者");
            li2.Add(CFlag(NPC.downedMechBoss2) + "双子魔眼");
            li2.Add(CFlag(NPC.downedMechBoss3) + "机械骷髅王");

            li2.Add(CFlag(NPC.downedPlantBoss) + "世纪之花");
            li2.Add(CFlag(NPC.downedGolemBoss) + "石巨人");

            li2.Add(CFlag(NPC.downedQueenSlime) + "史莱姆皇后");
            li2.Add(CFlag(NPC.downedEmpressOfLight) + "光之女皇");
            
            li2.Add(CFlag(NPC.downedFishron) + "猪龙鱼公爵");
            li2.Add(CFlag(NPC.downedAncientCultist) + "拜月教邪教徒");
            li2.Add(CFlag(NPC.downedMoonlord) + "月亮领主");


            li3.Add(CFlag(NPC.downedClown) + "小丑");
            li3.Add(CFlag(NPC.downedGoblins) + "哥布林军队");
            li3.Add(CFlag(NPC.downedPirates) + "海盗入侵");
            li3.Add(CFlag(NPC.downedMartians) + "火星暴乱");

            li3.Add(CFlag(NPC.downedHalloweenTree) + "哀木");
            li3.Add(CFlag(NPC.downedHalloweenKing) + "南瓜王");

            li3.Add(CFlag(NPC.downedChristmasIceQueen) + "冰雪女王");
            li3.Add(CFlag(NPC.downedChristmasTree) + "常绿尖叫怪");
            li3.Add(CFlag(NPC.downedChristmasSantank) + "圣诞坦克");

            li3.Add(CFlag(NPC.downedTowerSolar) + "日耀柱");
            li3.Add(CFlag(NPC.downedTowerVortex) + "星旋柱");
            li3.Add(CFlag(NPC.downedTowerNebula) + "星云柱");
            li3.Add(CFlag(NPC.downedTowerStardust) + "星尘柱");


            args.Player.SendInfoMessage("肉前：{0}", String.Join(", ", li1));
            args.Player.SendInfoMessage("肉后：{0}", String.Join(", ", li2));
            args.Player.SendInfoMessage("事件：{0}", String.Join(", ", li3));
        }

        # endregion


        # region NPC 管理

        /// <summary>
        /// npc 管理
        /// </summary>
        private void NpcManage(CommandArgs args)
        {
            if(args.Parameters.Count<string>()==0 || args.Parameters[0].ToLowerInvariant()=="help")
            {
                args.Player.SendInfoMessage("/npc info, 查看npc解救情况");
                args.Player.SendInfoMessage("/npc sm, sm召唤指令备注（SpawnMob npc召唤指令）");
                args.Player.SendInfoMessage("/npc toggle <解救npc名 或 猫/狗/兔 >, 切换NPC解救状态");
                args.Player.SendInfoMessage("/npc clear <NPC名>, 移除一个NPC");
                return;
            }

            List<string> li = new List<string>();
            List<string> li1 = new List<string>();
            List<string> li2 = new List<string>();
            switch (args.Parameters[0].ToLowerInvariant())
            {
                default:
                    args.Player.SendErrorMessage("语法不正确！");
                    break;

                case "sm":
                case "spawn":
                    List<string> names = _NPCTypes.Keys.ToList<string>();
                    List<string> newStrs = new List<string>();
                    for (int i = 0; i < names.Count; i++)
                    {
                        newStrs.Add( string.Format("/sm {0} ({1})", _NPCTypes[names[i]], names[i]) );
                    }
                    args.Player.SendInfoMessage("以下是 npc 生成指令, sm = spawnmob：");
                    args.Player.SendInfoMessage(String.Join(", ", newStrs));
                    break;
                

                // 查看npc解救情况
                case "info":
                    li = NPC.savedAngler ? li1 : li2;
                    li.Add("渔夫");

                    li = NPC.savedGoblin ? li1 : li2;
                    li.Add("哥布林工匠");

                    li = NPC.savedMech ? li1 : li2;
                    li.Add("机械师");

                    li = NPC.savedStylist ? li1 : li2;
                    li.Add("发型师");

                    li = NPC.savedBartender ? li1 : li2;
                    li.Add("酒馆老板");

                    li = NPC.savedGolfer ? li1 : li2;
                    li.Add("高尔夫球手");

                    li = NPC.savedWizard ? li1 : li2;
                    li.Add("巫师");

                    li = NPC.savedTaxCollector ? li1 : li2;
                    li.Add("税收官");


                    if (li1.Count > 0)
                        args.Player.SendInfoMessage("已解救：{0}", String.Join(", ", li1));
                    if (li2.Count > 0 )
                        args.Player.SendInfoMessage("待解救：{0}", String.Join(", ", li2));


                    // 城镇宠物
                    li1 = new List<string>();
                    li2 = new List<string>();

                    li = NPC.boughtCat ? li1 : li2;
                    li.Add("猫咪");

                    li = NPC.boughtDog ? li1 : li2;
                    li.Add("狗狗");

                    li = NPC.boughtBunny ? li1 : li2;
                    li.Add("兔兔");
                    if (li1.Count > 0)
                        args.Player.SendInfoMessage("已使用：{0}许可证", String.Join(", ", li1));
                    if (li2.Count > 0 )
                        args.Player.SendInfoMessage("待使用：{0}许可证", String.Join(", ", li2));

                    break;

                // 切换npc解救状态
                case "toggle":
                    ToggleNPC(args);
                    break;
                
                // 移除一个npc
                case "clear":
                    ClearNPC(args);
                    break;
            }
        }

        
         /// <summary>
         /// 切换npc解救状态
         /// </summary>
        private void ToggleNPC(CommandArgs args)
        {
            if (args.Parameters.Count < 2){
                args.Player.SendInfoMessage("语法不正确 /npc toggle <解救npc名，或 猫/狗/兔>");
                return;
            }

            switch ( args.Parameters[1].ToLowerInvariant() )
            {
                default:
                    args.Player.SendErrorMessage("语法不正确！，请使用 /npc toggle help, 进行查询");
                    break;
                
                case "help":
                    string [] li = {
                        "渔夫",
                        "哥布林工匠",
                        "机械师",
                        "发型师",
                        "酒馆老板",
                        "高尔夫球手",
                        "巫师",
                        "税收官", 
                        "猫",
                        "狗",
                        "兔"
                    };
                    args.Player.SendInfoMessage("支持切换的NPC拯救/购买状态的有: ");
                    args.Player.SendInfoMessage("{0}", String.Join(", ", li));
                    break;


                // 渔夫
                case "渔夫":
                case "沉睡渔夫":
                case "angler":
                    NPC.savedAngler = !NPC.savedAngler;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedAngler)
                        args.Player.SendSuccessMessage("沉睡渔夫 已解救");
                    else
                        args.Player.SendSuccessMessage("渔夫 已标记为 未解救");
                    break;


                // 哥布林工匠
                case "哥布林工匠":
                case "受缚哥布林":
                case "goblin":
                    NPC.savedGoblin = !NPC.savedGoblin;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedGoblin)
                        args.Player.SendSuccessMessage("受缚哥布林 已解救");
                    else
                        args.Player.SendSuccessMessage("哥布林工匠 已标记为 未解救");
                    break;


                // 机械师
                case "机械师":
                case "受缚机械师":
                case "mech":
                case "mechanic":
                    NPC.savedMech = !NPC.savedMech;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedMech)
                        args.Player.SendSuccessMessage("受缚机械师 已解救");
                    else
                        args.Player.SendSuccessMessage("机械师 已标记为 未解救");
                    break;


                // 发型师
                case "发型师":
                case "受缚发型师":
                case "stylist":
                    NPC.savedStylist = !NPC.savedStylist;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedStylist)
                        args.Player.SendSuccessMessage("被网住的发型师 已解救");
                    else
                        args.Player.SendSuccessMessage("发型师 已标记为 未解救");
                    break;


                // 酒馆老板
                case "酒馆老板":
                case "昏迷男子":
                case "酒保":
                case "bartender":
                case "tavernkeep":
                case "tavern":
                    NPC.savedBartender = !NPC.savedBartender;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedBartender)
                        args.Player.SendSuccessMessage("昏迷男子 已解救");
                    else
                        args.Player.SendSuccessMessage("酒馆老板 已标记为 未解救");
                    break;


                // 高尔夫球手
                case "高尔夫球手":
                case "高尔夫":
                case "golfer":
                    NPC.savedGolfer = !NPC.savedGolfer;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedGolfer)
                        args.Player.SendSuccessMessage("高尔夫球手 已解救");
                    else
                        args.Player.SendSuccessMessage("高尔夫球手 已标记为 未解救");
                    break;


                // 巫师
                case "巫师":
                case "受缚巫师":
                case "wizard":
                    NPC.savedWizard = !NPC.savedWizard;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedWizard)
                        args.Player.SendSuccessMessage("受缚巫师 已解救");
                    else
                        args.Player.SendSuccessMessage("巫师 已标记为 未解救");
                    break;


                // 税收官
                case "税收官":
                case "痛苦亡魂":
                case "tax":
                case "tax collector":
                    NPC.savedTaxCollector = !NPC.savedTaxCollector;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedTaxCollector)
                        args.Player.SendSuccessMessage("痛苦亡魂 已净化");
                    else
                        args.Player.SendSuccessMessage("税收官 已标记为 未解救");
                    break;


                // 猫
                case "猫":
                case "cat":
                    NPC.boughtCat = !NPC.boughtCat;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.boughtCat)
                        args.Player.SendSuccessMessage("猫咪许可证 已生效");
                    else
                        args.Player.SendSuccessMessage("猫咪许可证 已标记为 未使用");
                    break;


                // 狗
                case "狗":
                case "dog":
                    NPC.boughtDog = !NPC.boughtDog;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.boughtDog)
                        args.Player.SendSuccessMessage("狗狗许可证 已生效");
                    else
                        args.Player.SendSuccessMessage("狗狗许可证 已标记为 未使用");
                    break;


                // 兔
                case "兔子":
                case "兔兔":
                case "兔":
                case "bunny":
                case "rabbit":
                    NPC.boughtBunny = !NPC.boughtBunny;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.boughtBunny)
                        args.Player.SendSuccessMessage("兔兔许可证 已生效");
                    else
                        args.Player.SendSuccessMessage("兔兔许可证 已标记为 未使用");
                    break;
            }
        }


         /// <summary>
        /// 清理NPC
        /// </summary>
        private void ClearNPC(CommandArgs args)
        {
            if (args.Parameters.Count ==1)
            {
                args.Player.SendInfoMessage("语法不正确");
                args.Player.SendInfoMessage("/npc clear <NPC名>, 清除指定NPC");
                args.Player.SendInfoMessage("/npc clear, 清理敌怪但保留友善NPC");
                return;
            }

            // 清除指定NPC/敌怪
            switch ( args.Parameters[1].ToLowerInvariant() )
            {
                case "help":
                    args.Player.SendInfoMessage("语法不正确");
                    args.Player.SendInfoMessage("/npc clear <NPC名>, 清除指定NPC");
                    args.Player.SendInfoMessage("/npc clear, 清理敌怪但保留友善NPC");
                    break;

                default:
                    var npcs = TShock.Utils.GetNPCByIdOrName(args.Parameters[1]);
                    if (npcs.Count == 0)
                    {
                        args.Player.SendErrorMessage("找不到对应的 NPC");
                    }
                    else if (npcs.Count > 1)
                    {
                        args.Player.SendMultipleMatchError(npcs.Select(n => $"{n.FullName}({n.type})"));
                    }
                    else
                    {
                        var npc = npcs[0];
                        args.Player.SendSuccessMessage("清理了 {0} 个 {1}", ClearNPCByID(npc.netID), npc.FullName);
                    }
                    break;
            }
        }

         /// <summary>
        /// 清理指定id的NPC
        /// </summary>
        private int ClearNPCByID(int npcID)
        {
            int cleared = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].netID==npcID )
                {
                    Main.npc[i].active = false;
                    Main.npc[i].type = 0;
                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
                    cleared++;
                }
            }
            return cleared;
        }

        static Dictionary<string, int> _NPCTypes = new Dictionary<string, int>
        {
            { "商人", 17 },
            { "护士", 18 },
            { "军火商", 19 },
            { "树妖", 20 },
            { "向导", 22 },
            { "老人", 37 },
            { "爆破专家", 38 },
            { "服装商", 54 },
            { "受缚哥布林", 105 },
            { "受缚巫师", 106 },
            { "哥布林工匠", 107 },
            { "巫师", 108 },
            { "受缚机械师", 123 },
            { "机械师", 124 },
            { "圣诞老人", 142 },
            { "松露人", 160 },
            { "蒸汽朋克人", 178 },
            { "染料商", 207 },
            { "派对女孩", 208 },
            { "机器侠", 209 },
            { "油漆工", 227 },
            { "巫医", 228 },
            { "海盗", 229 },
            { "发型师", 353 },
            { "受缚发型师", 354 },
            { "旅商", 368 },
            { "渔夫", 369 },
            { "税收官", 441 },
            { "骷髅商人", 453 },
            { "酒馆老板", 550 },
            { "高尔夫球手", 588 },
            { "高尔夫球手待拯救", 589 },
            { "动物学家", 633 },
            { "公主", 663 },
        };

        # endregion


        // 获取秘密世界种子状态描述
        private string GetSecretWorldDescription()
        {
            string s = "";
            if(Main.getGoodWorld){
                s += "for the worthy";
            }
            if(Main.drunkWorld){
                if ( s != "")
                    s += ", ";
                s += "05162020";
            }
            if(Main.tenthAnniversaryWorld){
                if ( s != "")
                    s += ", ";
                s += "05162021";
            }
            if(Main.dontStarveWorld){
                if ( s != "")
                    s += ", ";
                s += "the constant";
            }

            if ( s != "")
                s = "秘密世界:  " + s;

            return s;
        }


        // 获取腐化类型描述
        private string GetCorruptionDescription()
        {
            if(Main.ActiveWorldFileData.HasCrimson && Main.ActiveWorldFileData.HasCorruption){
                return "腐化类型: 腐化和猩红";
            }
            if(Main.ActiveWorldFileData.HasCrimson){
                return "腐化类型: 猩红";
            }
            if(Main.ActiveWorldFileData.HasCorruption){
                return "腐化类型: 腐化";
            }
            return "";
        }

        static Dictionary<string, int> _worldModes = new Dictionary<string, int>
        {
            { "经典", 1 },
            { "专家", 2 },
            { "大师", 3 },
            { "旅行", 4 }
        };

        static Dictionary<string, int> _moonPhases = new Dictionary<string, int>
        {
            { "满月", 1 },
            { "亏凸月", 2 },
            { "下弦月", 3 },
            { "残月", 4 },
            { "新月", 5 },
            { "娥眉月", 6 },
            { "上弦月", 7 },
            { "盈凸月", 8 }
        };

        // https://terraria.fandom.com/wiki/Moon_phase
        static Dictionary<string, int> _moonTypes = new Dictionary<string, int>
        {
            { "正常", 1 },
            { "火星样式", 2 },
            { "土星样式", 3 },
            { "秘银风格", 4 },
            { "明亮的偏蓝白色", 5 },
            { "绿色", 6 },
            { "糖果", 7 },
            { "金星样式", 8 },
            { "紫色的三重月亮", 9 }
        };


        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			base.Dispose(disposing);
		}
	}
}
