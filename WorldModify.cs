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
            Commands.ChatCommands.Add(new Command(new List<string>() {"moonphase"}, ChangeMoonPhase, "moonphase", "mp", "moon") { HelpText = "修改月相"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"moonstyle"}, ChangeMoonStyle, "moonstyle", "ms") { HelpText = "修改月亮样式"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"helper"}, helper, "helper") { HelpText = "额外的指令帮助"});
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
                args.Player.SendInfoMessage("/wm ftw，开启/关闭 for the worthy 秘密世界");
                args.Player.SendInfoMessage("/wm boss，查看boss进度");
                args.Player.SendInfoMessage("/wm npc，查看NPC解救情况");
                args.Player.SendInfoMessage("/wm search, 解锁全物品研究（有待测试）");
            }
            string CFlag(bool foo){
                if (foo)
                    return "✔";
                else
                    return "";
            }

            if (args.Parameters.Count<string>() == 0)
            {
                ShowHelpText();
                return;
            }

            List<string> li1 = new List<string>();
            List<string> li2 = new List<string>();
            List<string> li3 = new List<string>();
            List<string> li = new List<string>();
            switch (args.Parameters[0].ToLowerInvariant())
            {
                // 帮助
                default:
                case "help":
                    ShowHelpText();
                    return;

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
                

                // 查看boss进度
                case "boss":
                    li1.Add(CFlag(NPC.downedSlimeKing) + "史莱姆王");
                    li1.Add(CFlag(NPC.downedBoss1) + "克苏鲁之眼");

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
                    break;

                // 查看npc解救情况
                case "npc":
                    li = NPC.savedAngler ? li1 : li2;
                    li.Add("渔夫");

                    li = NPC.savedGoblin ? li1 : li2;
                    li.Add("哥布林");

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
                    break;


                // 全物品研究（有待测试）
                case "research":
                    List<int> ids = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys.ToList<int>();
                    int amountNeeded;
                    for (int i = 0; i < ids.Count; i++)
                    {
                        CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(ids[i], out amountNeeded);
                        TShock.ResearchDatastore.SacrificeItem(ids[i], amountNeeded, args.Player);
                    }
                    args.Player.SendInfoMessage("已解锁 {0} 个物品研究", ids.Count);
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


        /// <summary> 额外的指令帮助 </summary>
        private void helper(CommandArgs args)
        {
            if(args.Parameters.Count<string>()==0 || args.Parameters[0].ToLowerInvariant()=="help")
            {
                args.Player.SendInfoMessage("/helper boss, spawnboss指令备注");
                args.Player.SendInfoMessage("/helper npc, spawnmob指令备注,NPC部分");
                return;
            }

            switch (args.Parameters[0].ToLowerInvariant())
            {
                // npc
                case "npc":
                    List<string> names = _NPCTypes.Keys.ToList<string>();
                    List<string> newStrs = new List<string>();
                    for (int i = 0; i < names.Count; i++)
                    {
                        newStrs.Add( string.Format("/sm {0} ({1})", _NPCTypes[names[i]], names[i]) );
                    }
                    args.Player.SendInfoMessage(String.Join(", ", newStrs));
                    break;

                // boss
                case "sb":
                case "boss":
                    string [] part1 = {
                        "\"king slime\" (史莱姆王)",
                        "\"eye of cthulhu\" (克苏鲁之眼)",
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
                    args.Player.SendInfoMessage("/sb " + String.Join(", /sb ", part1));
                    args.Player.SendInfoMessage("/sb " + String.Join(", /sb ", part2));
                    args.Player.SendInfoMessage("/sb " + String.Join(", /sb ", part3));
                    break;
            }
        }


        // 获取秘密世界种子状态描述
        private string GetSecretWorldDescription()
        {
            if(Main.drunkWorld && Main.getGoodWorld){
                return "秘密世界：for the worthy 和 05162020";
            }
            if(Main.getGoodWorld){
                return "秘密世界：for the worthy";
            }
            if(Main.drunkWorld){
                return "秘密世界：05162020";
            }
            return "";
        }


        // 获取腐化类型描述
        private string GetCorruptionDescription()
        {
            if(Main.ActiveWorldFileData.HasCrimson && Main.ActiveWorldFileData.HasCorruption){
                return "腐化类型: 腐化和猩红共存";
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


        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			base.Dispose(disposing);
		}
	}
}
