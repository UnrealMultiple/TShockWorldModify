using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Bestiary;
using TShockAPI;


namespace WorldModify
{
    class NPCHelper
    {
        /// <summary>
        /// npc 管理
        /// </summary>
        public static void NpcManage(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if(args.Parameters.Count<string>()==0 || args.Parameters[0].ToLowerInvariant()=="help")
            {
                op.SendInfoMessage("/npc info, 查看npc解救情况");
                op.SendInfoMessage("/npc <解救npc名 或 猫/狗/兔 >, 切换NPC解救状态");
                op.SendInfoMessage("/npc list, 查看支持切换解救状态的NPC名");
                op.SendInfoMessage("/npc clear <NPC名>, 移除一个NPC");
                op.SendInfoMessage("/npc unique, NPC去重");
                op.SendInfoMessage("/npc relive, 复活NPC（根据怪物图鉴记录）");
                op.SendInfoMessage("/npc sm, sm召唤指令备注（SpawnMob npc召唤指令）");
                return;
            }

            List<string> li = new List<string>();
            List<string> li1 = new List<string>();
            List<string> li2 = new List<string>();
            string param = args.Parameters[0].ToLowerInvariant();
            switch (param)
            {
                default:
                    // 标记进度
                    bool isPass = ToggleNPC(op, param);
                    if( !isPass )
                        op.SendErrorMessage("语法不正确！");
                    break;

                case "sm":
                case "spawn":
                    List<string> names = _NPCTypes.Keys.ToList<string>();
                    List<string> newStrs = new List<string>();
                    for (int i = 0; i < names.Count; i++)
                    {
                        newStrs.Add( string.Format("/sm {0} ({1})", _NPCTypes[names[i]], names[i]) );
                    }
                    op.SendInfoMessage("以下是 npc 生成指令, sm = spawnmob：");
                    op.SendInfoMessage(String.Join(", ", newStrs));
                    break;


                // 查看npc解救情况
                case "info":

                    // CFlag(NPC.combatBookWasUsed, "[i:4382]先进战斗技术"),
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
                        op.SendInfoMessage("已解救：{0}", String.Join(", ", li1));
                    if (li2.Count > 0 )
                        op.SendInfoMessage("待解救：{0}", String.Join(", ", li2));


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
                        op.SendInfoMessage("已使用：{0}许可证", String.Join(", ", li1));
                    if (li2.Count > 0 )
                        op.SendInfoMessage("待使用：{0}许可证", String.Join(", ", li2));

                    break;


                // 移除一个npc
                case "clear":
                    ClearNPC(args);
                    break;

                // NPC去重
                case "unique":
                    UniqueNPC(args);
                    break;

                // NPC重生
                case "relive":
                    Relive(args);
                    break;
            }
        }


         /// <summary>
         /// 切换npc解救状态
         /// </summary>
        private static bool ToggleNPC(TSPlayer op, string param)
        {
            switch ( param )
            {
                case "list":
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
                    op.SendInfoMessage("支持切换的NPC拯救/购买状态的有: ");
                    op.SendInfoMessage("{0}", String.Join(", ", li));
                    break;


                // 渔夫
                case "渔夫":
                case "沉睡渔夫":
                case "angler":
                    NPC.savedAngler = !NPC.savedAngler;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedAngler)
                        op.SendSuccessMessage("沉睡渔夫 已解救");
                    else
                        op.SendSuccessMessage("渔夫 已标记为 未解救");
                    break;


                // 哥布林工匠
                case "哥布林工匠":
                case "受缚哥布林":
                case "goblin":
                    NPC.savedGoblin = !NPC.savedGoblin;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedGoblin)
                        op.SendSuccessMessage("受缚哥布林 已解救");
                    else
                        op.SendSuccessMessage("哥布林工匠 已标记为 未解救");
                    break;


                // 机械师
                case "机械师":
                case "受缚机械师":
                case "mech":
                case "mechanic":
                    NPC.savedMech = !NPC.savedMech;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedMech)
                        op.SendSuccessMessage("受缚机械师 已解救");
                    else
                        op.SendSuccessMessage("机械师 已标记为 未解救");
                    break;


                // 发型师
                case "发型师":
                case "受缚发型师":
                case "stylist":
                    NPC.savedStylist = !NPC.savedStylist;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedStylist)
                        op.SendSuccessMessage("被网住的发型师 已解救");
                    else
                        op.SendSuccessMessage("发型师 已标记为 未解救");
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
                        op.SendSuccessMessage("昏迷男子 已解救");
                    else
                        op.SendSuccessMessage("酒馆老板 已标记为 未解救");
                    break;


                // 高尔夫球手
                case "高尔夫球手":
                case "高尔夫":
                case "golfer":
                    NPC.savedGolfer = !NPC.savedGolfer;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedGolfer)
                        op.SendSuccessMessage("高尔夫球手 已解救");
                    else
                        op.SendSuccessMessage("高尔夫球手 已标记为 未解救");
                    break;


                // 巫师
                case "巫师":
                case "受缚巫师":
                case "wizard":
                    NPC.savedWizard = !NPC.savedWizard;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedWizard)
                        op.SendSuccessMessage("受缚巫师 已解救");
                    else
                        op.SendSuccessMessage("巫师 已标记为 未解救");
                    break;


                // 税收官
                case "税收官":
                case "痛苦亡魂":
                case "tax":
                case "tax collector":
                    NPC.savedTaxCollector = !NPC.savedTaxCollector;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.savedTaxCollector)
                        op.SendSuccessMessage("痛苦亡魂 已净化");
                    else
                        op.SendSuccessMessage("税收官 已标记为 未解救");
                    break;


                // 猫
                case "猫":
                case "cat":
                    NPC.boughtCat = !NPC.boughtCat;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.boughtCat)
                        op.SendSuccessMessage("猫咪许可证 已生效");
                    else
                        op.SendSuccessMessage("猫咪许可证 已标记为 未使用");
                    break;


                // 狗
                case "狗":
                case "dog":
                    NPC.boughtDog = !NPC.boughtDog;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                    if (NPC.boughtDog)
                        op.SendSuccessMessage("狗狗许可证 已生效");
                    else
                        op.SendSuccessMessage("狗狗许可证 已标记为 未使用");
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
                        op.SendSuccessMessage("兔兔许可证 已生效");
                    else
                        op.SendSuccessMessage("兔兔许可证 已标记为 未使用");
                    break;

                default:
                    // op.SendErrorMessage("语法不正确！，请使用 /npc toggle help, 进行查询");
                    return false;
            }
            return true;
        }


         /// <summary>
        /// 清理NPC
        /// </summary>
        private static void ClearNPC(CommandArgs args)
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
        // NPC去重
        /// <summary>
        private static void UniqueNPC(CommandArgs args)
        {
            // List<int> ids = new List<int>() {22,33};
            List<int> founds = new List<int>();
            int num = 0;
            int cleared = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if( !Main.npc[i].active )
                    continue;

                // 453骷髅商人
                if( !Main.npc[i].townNPC && Main.npc[i].type!=453 )
                    continue;

                num = Main.npc[i].type;
                if( founds.Contains(num) )
                {
                    Main.npc[i].active = false;
                    Main.npc[i].type = 0;
                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
                    cleared++;
                } else {
                    founds.Add( num );
                }
            }

            if( cleared>0 ){
                args.Player.SendSuccessMessage($"已清除 {cleared} 个重复的NPC");
            } else {
                args.Player.SendInfoMessage("没有可清除的 重复的NPC");
            }
        }

        /// <summary>
        // NPC重生
        /// <summary>
        public static void Relive(CommandArgs args)
        {
            List<int> found = new List<int>();
            TSPlayer op = args.Player;

            // 解救状态
            // 渔夫
            if( NPC.savedAngler )
                found.Add(369);

            // 哥布林
            if( NPC.savedGoblin )
                found.Add(107);

            // 机械师
            if( NPC.savedMech )
                found.Add(124);

            // 发型师
            if( NPC.savedStylist )
                found.Add(353);

            // 酒馆老板
            if( NPC.savedBartender )
                found.Add(550);

            // 高尔夫球手
            if( NPC.savedGolfer )
                found.Add(588);

            // 巫师
            if( NPC.savedWizard )
                found.Add(108);

            // 税收管
            if( NPC.savedTaxCollector )
                found.Add(441);

            // 猫
            if( NPC.boughtCat )
                found.Add(637);

            // 狗
            if( NPC.boughtDog )
                found.Add(638);

            // 兔
            if( NPC.boughtBunny )
                found.Add(656);

            // 怪物图鉴解锁情况
            List<int> remains = new List<int>() {
                22, //向导
                19, //军火商
                54, //服装商
                38, //爆破专家
                20, //树妖
                207, //染料商
                17, //商人
                18, //护士
                227, //油漆工
                208, //派对女孩
                228, //巫医
                633, //动物学家
                209, //机器侠
                229, //海盗
                178, //蒸汽朋克人
                160, //松露人
                663 //公主

                // 453, //骷髅商人
                // 368, //旅商
                // 37, // 老人
            };
            // 142, //圣诞老人
            if( Main.xMas )
                remains.Add(142);

            foreach (int npcID1 in remains)
            {
                if( DidDiscoverBestiaryEntry( npcID1 ) )
                    found.Add(npcID1);
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if( !Main.npc[i].active || !Main.npc[i].townNPC )
                    continue;

                found.Remove(Main.npc[i].type);
            }

            // 生成npc
            List<string> names = new List<string>();
            foreach (int npcID in found)
            {
				NPC npc = new NPC();
				npc.SetDefaults(npcID);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, 1, op.TileX, op.TileY,5,2);

                if( names.Count!=0 && names.Count%10==0 ){
                    names.Add("\n"+npc.FullName);
                } else {
                    names.Add(npc.FullName);
                }
            }

            // 找家
            // for (int i = 0; i < Main.maxNPCs; i++)
            // {
            //     if( !Main.npc[i].active || !Main.npc[i].townNPC )
            //         continue;

            //     if( found.Contains(Main.npc[i].type) )
            //         WorldGen.QuickFindHome(i);
            // }

            if( found.Count>0 ){
				TSPlayer.All.SendInfoMessage($"{op.Name} 复活了 {found.Count}个 NPC:");
				TSPlayer.All.SendInfoMessage($"{string.Join("、", names)}");
                if( !op.RealPlayer ){
                    op.SendInfoMessage($"复活了 {found.Count}个 NPC:");
                    op.SendInfoMessage($"{string.Join("、", names)}");
                }
            } else {
                op.SendInfoMessage("入住过的NPC都活着");
            }
        }
        public static bool DidDiscoverBestiaryEntry(int npcId)
		{
			return Main.BestiaryDB.FindEntryByNPCID(npcId).UIInfoProvider.GetEntryUICollectionInfo().UnlockState > BestiaryEntryUnlockState.NotKnownAtAll_0;
		}

         /// <summary>
        /// 清理指定id的NPC
        /// </summary>
        private static int ClearNPCByID(int npcID)
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
        public static void Clear(){
            _NPCTypes.Clear();
        }

    }
}