using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using TerrariaApi.Server;
using TShockAPI;


namespace WorldModify
{
    [ApiVersion(2, 1)]
    public class WorldModify : TerrariaPlugin
    {
        public override string Author => "hufang360";

        public override string Description => "简易的世界修改器";

        public override string Name => "WorldModify";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public WorldModify(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.use"}, WMCommand, "worldmodify", "wm") { HelpText = "简易的世界修改器"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.moon"}, ChangeMoonPhase, "moonphase", "moon") { HelpText = "月相管理"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.moonstyle"}, ChangeMoonStyle, "moonstyle", "ms") { HelpText = "月亮样式管理"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.boss"}, BossHelper.BossManage, "bossmanage", "boss" ) { HelpText = "boss管理"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.npc"}, NpcHelper.NpcManage, "npcmanage", "npc") { HelpText = "npc管理"});

            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.relive"}, NpcHelper.Relive, "relive") { HelpText = "复活NPC"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.bossinfo"}, BossHelper.BossInfo, "bossinfo", "bi") { HelpText = "boss进度信息"});
            Commands.ChatCommands.Add(new Command(new List<string>() {"wm.worldinfo"}, WorldInfo, "worldinfo", "wi") { HelpText = "世界信息"});
        }

        #region command wm
        private void WMCommand(CommandArgs args)
        {
            TSPlayer op = args.Player;
            void ShowHelpText()
            {
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out int pageNumber))
                    return;

                List<string> lines = new List<string> {
                    "/wm info，查看 世界信息",
                    "/wm name [世界名]，查看/修改 世界名字",
                    "/wm id [id]，查看/修改 世界ID",
                    "/wm uuid [uuid]，查看/修改 世界uuid",

                    "/wm seed [种子]，查看/修改 世界种子",
                    "/wm 0516，开启/关闭 05162020 秘密世界",
                    "/wm 05162021，开启/关闭 05162021 秘密世界",
                    "/wm ftw，开启/关闭 for the worthy 秘密世界",
                    "/wm ntb，开启/关闭 not the bees 秘密世界",
                    "/wm dst，开启/关闭 永恒领域 秘密世界（饥荒联动）",

                    "/wm spawn，查看 出生点",
                    "/wm dungeon，查看 地牢点",
                    "/wm surface [深度]，查看/修改 地表深度",
                    "/wm cave [深度]，查看/修改 洞穴深度",

                    "/wm wind，查看 风速",
                    "/wm research,  解锁 全物品研究",
                    "/wm bestiary,  解锁 怪物图鉴全收集",

                    "/moon help,  月相管理",
                    "/moonstyle help,  月亮样式管理",
                    "/boss help, boss管理",
                    "/npc help, npc管理"
                };

                PaginationTools.SendPage(
                    op, pageNumber, lines,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = "帮助 ({0}/{1})：",
                        FooterFormat = "输入 {0}wm help {{0}} 查看更多".SFormat(Commands.Specifier)
                    }
                );
            }

            if (args.Parameters.Count<string>() == 0)
            {
                op.SendErrorMessage("语法错误，输入 /wm help 查询用法");
                return;
            }

            string text = "";
            switch (args.Parameters[0].ToLowerInvariant())
            {
                // 帮助
                case "help":
                    ShowHelpText();
                    return;

                default:
                    op.SendErrorMessage("语法不正确！输入 /wm help 查询用法");
                    break;

                // 世界信息
                case "info":
                    ShowWorldInfo(args, true);
                    break;


                // 名字
                case "name":
                    if( args.Parameters.Count == 1 ){
                        op.SendInfoMessage("世界名称: {0}", Main.worldName);
                        op.SendInfoMessage("更改世界名称，请输入 /wm seed <名称>");
                        break;
                    }

                    Main.worldName = args.Parameters[1];
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    op.SendSuccessMessage("世界的名称已改成 {0}", args.Parameters[1]);
                    break;


                // 种子
                case "seed":
                    if( args.Parameters.Count == 1 ){
                        op.SendInfoMessage("世界种子: {0}（{1}）", WorldGen.currentWorldSeed, Main.ActiveWorldFileData.GetFullSeedText());
                        op.SendInfoMessage("更改世界种子，请输入 /wm seed <种子>");
                        break;
                    }

                    Main.ActiveWorldFileData.SetSeed(args.Parameters[1]);
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    op.SendSuccessMessage("世界的种子已改成 {0}", args.Parameters[1]);
                    break;


                // worldId 800875906
                case "id":
                    if( args.Parameters.Count == 1 ){
                        op.SendInfoMessage("世界ID: {0}", Main.worldID);
                        op.SendInfoMessage("更改世界ID，请输入 /wm id <id>");
                        break;
                    }

                    int worldId;
                    if ( int.TryParse(args.Parameters[1],out worldId) )
                    {
                        Main.worldID = worldId;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("世界的ID已改成 {0}", args.Parameters[1]);
                    } else {
                        op.SendErrorMessage("世界ID只能由数字组成");
                    }
                    break;


                // uuid ee700694-ab04-434e-b872-8a800a527cd7
                case "uuid":
                    if( args.Parameters.Count == 1 ){
                        op.SendInfoMessage("uuid: {0}", Main.ActiveWorldFileData.UniqueId);
                        op.SendInfoMessage("更改世界的uuid，请输入 /wm uuid <uuid>");
                        break;
                    }

                    string uuid = args.Parameters[1].ToLower();
                    if( utils.ToGuid(uuid) )
                    {
                        Main.ActiveWorldFileData.UniqueId = new Guid(uuid);
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("世界的UUID已改成 {0}", uuid);
                    } else {
                        op.SendErrorMessage("uuid格式不正确！");
                    }
                    break;

                // 地表深度
                case "surface":
                case "sur":
                    if( args.Parameters.Count == 1 ){
                        op.SendInfoMessage($"表层深度: {Main.worldSurface}");
                        op.SendInfoMessage("修改深度，请输入 /wm surface <深度>");
                        break;
                    }
                    if ( int.TryParse(args.Parameters[1],out int surface) )
                    {
                        Main.worldSurface = surface;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("表层深度 已改成 {0}", surface);
                    } else {
                        op.SendErrorMessage("深度输入错误！");
                    }
                    break;

                // 洞穴深度
                case "cave":
                    if( args.Parameters.Count == 1 ){
                        op.SendInfoMessage($"洞穴深度: {Main.rockLayer}");
                        op.SendInfoMessage("修改深度，请输入 /wm cave <深度>");
                        break;
                    }
                    if ( int.TryParse(args.Parameters[1],out int cave) )
                    {
                        Main.rockLayer = cave;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("洞穴深度 已改成 {0}", cave);
                    } else {
                        op.SendErrorMessage("深度输入错误！");
                    }
                    break;

                // 出生点
                case "spawn":
                    op.SendInfoMessage($"出生点：{Main.spawnTileX}, {Main.spawnTileY}");
                    op.SendInfoMessage("进入游戏后，输入 /setspawn 设置出生点");
                    op.SendInfoMessage("进入游戏后，输入 /spawn 传送至出生点");
                    break;

                // 地牢点
                case "dungeon":
                case "dun":
                    op.SendInfoMessage($"地牢点：{Main.dungeonX }, {Main.dungeonY}");
                    op.SendInfoMessage("进入游戏后，输入 /setdungeon 设置地牢点");
                    op.SendInfoMessage("进入游戏后，输入 /tpnpc \"Old Man\" 传送至地牢点");
                    break;

                // 风速
                case "wind":
                    op.SendInfoMessage($"风速：{Main.windSpeedCurrent}");
                    op.SendInfoMessage("调节风速，请输入 /wind <速度>");
                    break;

                #region egg
                // 醉酒世界
                case "516":
                case "0516":
                case "5162020":
                case "05162020":
                case "2020":
                case "drunk":
                    if (Main.drunkWorld) {
                        Main.drunkWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已关闭 05162020 秘密世界（醉酒世界 / DrunkWorld）");
                    } else {
                        Main.drunkWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已开启 05162020 秘密世界（醉酒世界 / DrunkWorld）");
                    }
                    break;


                // 10周年庆典,tenthAnniversaryWorld
                case "2011":
                case "2021":
                case "5162011":
                case "5162021":
                case "05162011":
                case "05162021":
                case "celebrationmk10":
                    if (Main.tenthAnniversaryWorld) {
                        Main.tenthAnniversaryWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已关闭 05162021 秘密世界（10周年庆典）");
                    } else {
                        Main.tenthAnniversaryWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已开启 05162021 秘密世界（10周年庆典）");
                    }
                    break;


                // ftw
                case "ftw":
                case "for the worthy":
                    if (Main.getGoodWorld) {
                        Main.getGoodWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已关闭 for the worthy 秘密世界");
                    } else  {
                        Main.getGoodWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已开启 for the worthy 秘密世界");
                    }
                    break;

                // not the bees
                case "ntb":
                    if (Main.notTheBeesWorld) {
                        Main.notTheBeesWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已关闭 not the bees 秘密世界");
                    } else  {
                        Main.notTheBeesWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已开启 not the bees 秘密世界");
                    }
                    break;

                //  饥荒联动
                case "eye":
                case "dst":
                case "constant":
                    if (Main.dontStarveWorld) {
                        Main.dontStarveWorld = false;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已关闭 永恒领域 秘密世界（饥荒联动）");
                    } else {
                        Main.dontStarveWorld = true;
                        TSPlayer.All.SendData(PacketTypes.WorldInfo);
                        op.SendSuccessMessage("已开启 永恒领域 秘密世界（饥荒联动）");
                    }
                    break;
                #endregion

                // 全物品研究
                case "research":
                case "res":
                    if( args.Parameters.Count>1 && args.Parameters[1].ToLower()=="false")
                    {
                        ResearchHelper.reset();
                    } else {
                        ResearchHelper.unlockAll(op);
                    }
                    break;

                // 怪物图鉴
                case "bestiary":
                case "best":
                    if( args.Parameters.Count>1 && args.Parameters[1].ToLower()=="false")
                    {
                        BestiaryHelper.ResetBestiary();

                        text = "怪物图鉴 已重置，重进游戏后生效";
                        TSPlayer.All.SendInfoMessage(text);
                        if( !op.RealPlayer )
                            op.SendInfoMessage(text);
                    }else{
                        BestiaryHelper.UnlockBestiary();

                        BestiaryUnlockProgressReport result = Main.GetBestiaryProgressReport();
                        text = $"怪物图鉴 已全部解锁 ;-) {result.CompletionAmountTotal}/{result.EntriesTotal}";
                        TSPlayer.All.SendSuccessMessage(text);
                        if( !op.RealPlayer )
                            op.SendInfoMessage(text);
                    }
                    break;

            }
        }
        #endregion

        # region worldinfo
        static Dictionary<string, int> _worldModes = new Dictionary<string, int>
        {
            { "经典", 1 },
            { "专家", 2 },
            { "大师", 3 },
            { "旅行", 4 }
        };
        private void WorldInfo(CommandArgs args)
        {
            ShowWorldInfo(args);
        }

        private void ShowWorldInfo(CommandArgs args, bool isSuperAdmin=false)
        {
            TSPlayer op = args.Player;

            List<string> lines = new List<string> {
                $"名称: {Main.worldName}",
                $"大小: {Main.ActiveWorldFileData.WorldSizeName}（{Main.maxTilesX}x{Main.maxTilesY}）",
                $"难度: {_worldModes.Keys.ElementAt(Main.GameMode)}",
                $"种子: {WorldGen.currentWorldSeed}"
            };
            // if( isSuperAdmin )
            // {
            //     // lines.Add($"ID: {Main.worldID}");
            //     // lines.Add($"UUID: {Main.ActiveWorldFileData.UniqueId}");
            //     // lines.Add($"版本: {Main.curRelease}  {Main.versionNumber}");
            // }

            string text = "";
            string text2 = "";
            // 腐化 秘密世界
            text = GetSecretWorldDescription();
            if( !string.IsNullOrEmpty(text) )
                lines.Add( text );
            lines.Add( GetCorruptionDescription(isSuperAdmin) );

            // 时间
            if( isSuperAdmin )
            {
                double time = Main.time / 3600.0;
				time += 4.5;
				if (!Main.dayTime)
					time += 15.0;
				time = time % 24.0;
				lines.Add( string.Format("时间：{0}:{1:D2}", (int)Math.Floor(time), (int)Math.Floor((time % 1.0) * 60.0)) );
            }

            // 附魔日晷
            text = Main.fastForwardTime ? "生效中":"";
            text2 = Main.sundialCooldown>0 ? $"{Main.sundialCooldown}天后可再次使用":"";
            if( string.IsNullOrEmpty(text) )
                text = text2;
            else{
                if( !string.IsNullOrEmpty(text2) )
                    text = $"{text} {text2}";
            }
            if( !string.IsNullOrEmpty(text) )
                lines.Add($"日晷：{text}2");

            if( isSuperAdmin )
            {
                lines.Add($"月亮: {_moonPhases.Keys.ElementAt(Main.moonPhase)}");
                lines.Add($"月亮样式: {_moonTypes.Keys.ElementAt(Main.moonType)}");

                string percent = "";
                int num1 = 0;
                int num2 = 0;
                if( TShock.ServerSideCharacterConfig.Settings.Enabled )
                {
                    num1 = ResearchHelper.GetSacrificeCount();
                    num2 = ResearchHelper.GetSacrificeTotal();
                    percent = Terraria.Utils.PrettifyPercentDisplay( (float)(num1/num2), "P2");
                    lines.Add($"物品研究：{percent}（{num1}/{num2}）");
                } else {
                    lines.Add($"物品研究：需开启 SSC，暂不支持查看单个玩家的物品研究记录");
                }

                BestiaryUnlockProgressReport result = Main.GetBestiaryProgressReport();
                percent = Terraria.Utils.PrettifyPercentDisplay(result.CompletionPercent, "P2");
                lines.Add($"怪物图鉴：{percent}（{result.CompletionAmountTotal}/{result.EntriesTotal}）");


                lines.Add($"出生点：{Main.spawnTileX}, {Main.spawnTileY}");
                lines.Add($"地牢点：{Main.dungeonX }, {Main.dungeonY}");
                lines.Add($"表层深度: {Main.worldSurface}");
                lines.Add($"洞穴深度: {Main.rockLayer}");


                if( DD2Event.DownedInvasionT1 )
                    text ="已通过 T1难度";
                else if( DD2Event.DownedInvasionT2 )
                    text ="已通过 T2难度";
                else if( DD2Event.DownedInvasionT3 )
                    text ="已通过 T3难度";
                else
                    text ="";
                if( !string.IsNullOrEmpty(text) )
                    lines.Add($"撒旦军队：{text}");

                // 日食、血月
                // 哥布林军队、海盗入侵
                // 撒旦军队
                // 派对、雨、沙尘暴
                // 史莱姆雨
                // 大风天
                // 南瓜月、雪人军团、霜月、火星暴乱
                // 月亮事件
                string textSize = $"（已清理{Main.invasionProgress}%波）（规模：{Main.invasionSize} ）";
                string textSize2 = $"（第{Main.invasionProgressWave}波：{Main.invasionProgress}%）";
                if( Main.invasionType==1 )
                    text = $"哥布林入侵（{textSize}）";
                else if( Main.invasionType==2 )
                    text = $"霜月（{textSize2}）";
                else if( Main.invasionType==3 )
                    text = $"海盗入侵（{textSize}）";
                else if( Main.invasionType==4 )
                    text = $"火星暴乱（{textSize}）";
                else{
                    // NPC.BusyWithAnyInvasionOfSorts
                    if( Main.pumpkinMoon )
                        text = $"南瓜月（{textSize2}）";
                    else if( Main.snowMoon )
                        text = $"雪人军团（{textSize}）";
                    else if( DD2Event.Ongoing )
                        text = $"撒旦军队（{textSize2}）";
                    else
                        text = "";
                }
                if( !string.IsNullOrEmpty(text) )
                    lines.Add($"入侵：{string.Join(", ", text)}");

                // lines.Add($"云量：{Main.numClouds}");
                // lines.Add($"风速：{Main.windSpeedCurrent}");

                // 杂项
                List<string> texts = new List<string>();
                if( BirthdayParty._wasCelebrating ) texts.Add("派对");
                if( LanternNight.LanternsUp ) texts.Add("灯笼夜");
                if( Star.starfallBoost>3f ) texts.Add("流星雨");
                if( Main.bloodMoon ) texts.Add("血月");
                if( Main.eclipse ) texts.Add("日食");
                if( Main.raining ) texts.Add("雨");
                if( Main.IsItStorming ) texts.Add("雷雨");
                if( Main.IsItAHappyWindyDay ) texts.Add("大风天");
                if( Sandstorm.Happening ) texts.Add("沙尘暴");
                if( Main.slimeRain ) texts.Add("史莱姆雨");
                if( texts.Count>0 )
                    lines.Add($"事件：{string.Join(", ", texts)}");

                texts = new List<string>();
                if( WorldGen.spawnMeteor ) texts.Add("陨石");
                if( Main.xMas ) texts.Add("圣诞节");
                if( Main.halloween ) texts.Add("万圣节");
                if( texts.Count>0 )
                    lines.Add($"杂项：{string.Join(", ", texts)}");
            }
            op.SendInfoMessage( string.Join("\n", lines) );
        }
        #endregion


        #region moon
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
            void helpText (){
                args.Player.SendInfoMessage("用法：/moonstyle <月亮样式>");
                args.Player.SendInfoMessage("月亮样式：{0} （可用数字 1~9 代替）", String.Join(", ", _moonTypes.Keys));
            }

            if(args.Parameters.Count<string>()==0){
                args.Player.SendInfoMessage("当前月亮样式: {0}", _moonTypes.Keys.ElementAt(Main.moonType));
                helpText();
                return;
            }

            if( args.Parameters[0].ToLowerInvariant() ==  "help" ){
                helpText();
                return;
            }

            int moontype;
            if (int.TryParse(args.Parameters[0], out moontype))
            {
                if (moontype < 1 || moontype > 9)
                {
                    helpText();
                    return;
                }
            }
            else if (_moonTypes.ContainsKey(args.Parameters[0]))
            {
                moontype = _moonTypes[args.Parameters[0]];
            }
            else
            {
                helpText();
                return;
            }
            Main.dayTime = false;
            Main.moonType = moontype-1;
            Main.time = 0.0;
            TSPlayer.All.SendData(PacketTypes.WorldInfo);
            args.Player.SendSuccessMessage("月亮样式已改为 {0}", _moonTypes.Keys.ElementAt(moontype-1));
        }
        #endregion


        #region get description
        // 获取秘密世界种子状态描述
        private string GetSecretWorldDescription()
        {
            List<string> ss = new List<string>();

            if(Main.getGoodWorld)
                ss.Add("for the worthy");

            if(Main.drunkWorld)
                ss.Add("05162020");

            if(Main.tenthAnniversaryWorld)
                ss.Add("05162021");

            if(Main.dontStarveWorld)
                ss.Add("the constant");

            if(Main.notTheBeesWorld)
                ss.Add("not the bees");

            if ( ss.Count>0 )
                return $"彩蛋: { String.Join(", ", ss) }";
            else
                return "";
        }


        // 获取腐化类型描述
        private string GetCorruptionDescription(bool isSuperAdmin=false)
        {
            // Main.ActiveWorldFileData.HasCrimson
            // Main.ActiveWorldFileData.HasCorruption
            string more(int type)
            {
                if( isSuperAdmin )
                    type = 3;

                string text = "";

                if( type==1 )
                    text = $"已摧毁 {WorldGen.shadowOrbCount}个[i:115]";
                else if( type==2 )
                    text = $"已摧毁 {WorldGen.heartCount}个[i:3062]";
                else
                    text = $"已摧毁 {WorldGen.heartCount}个[i:3062] {WorldGen.shadowOrbCount}个[i:115]";

                if( Main.hardMode )
                    text += $" {WorldGen.altarCount}个祭坛";

                string s2 = GetWorldStatusDialog();
                if( !string.IsNullOrEmpty(s2) )
                    text += $", {s2}";

                return text;
            }

            if(Main.drunkWorld){
                return $"腐化: 腐化和猩红, {more(3)}";
            } else {
                if(WorldGen.crimson)
                    return $"腐化: 猩红, {more(2)}";
                else
                    return $"腐化: 腐化, {more(1)}";
            }
        }


        public string GetWorldStatusDialog()
		{
			int tGood = WorldGen.tGood;
			int tEvil = WorldGen.tEvil;
			int tBlood = WorldGen.tBlood;

			if (tGood > 0 && tEvil > 0 && tBlood > 0)
				return $"{tGood}%神圣 {tEvil}%腐化 {tBlood}%猩红";

			else if (tGood > 0 && tEvil > 0)
				return $"{tGood}%神圣 {tEvil}%腐化";

            else if (tGood > 0 && tBlood > 0)
				return $"{tGood}%神圣 {tBlood}%猩红";

            else if (tEvil > 0 && tBlood > 0)
				return $"{tEvil}%腐化 {tBlood}%猩红";

            else if (tEvil > 0)
				return $"{tEvil}%腐化";

            else if (tBlood > 0)
				return $"{tBlood}%猩红";

            else
                return "";
		}
        #endregion

        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                _worldModes.Clear();
                _moonPhases.Clear();
                _moonTypes.Clear();

                BossHelper.Clear();
                NpcHelper.Clear();
                ResearchHelper.Clear();
			}
			base.Dispose(disposing);
		}
	}
}
