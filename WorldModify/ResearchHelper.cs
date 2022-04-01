using System.Collections.Generic;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using TShockAPI;


namespace WorldModify
{
    class ResearchHelper
    {
        public static void unlockAll(TSPlayer op)
        {
            op.SendInfoMessage("正在解锁，请稍等……");
            Dictionary<int, int> dic = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId;
            foreach (KeyValuePair<int, int> item in dic)
            {
                TShock.ResearchDatastore.SacrificeItem(item.Key, item.Value, op);
                var response = NetCreativeUnlocksModule.SerializeItemSacrifice(item.Key, item.Value);
                NetManager.Instance.Broadcast(response);
            }
            op.SendSuccessMessage($"已解锁 { dic.Count} 个物品研究");
        }

        public static void Reset()
        {
        }

        public static int GetSacrificeTotal()
        {
            return CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Count;
        }

        public static int GetSacrificeCompleted()
        {
            Dictionary<int, int> datas = TShock.ResearchDatastore.GetSacrificedItems();
            int count = 0;
            foreach (int key in datas.Keys)
            {
                int amount = datas[key];
                CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(key, out int amountNeeded);
                if (amount >= amountNeeded)
                {
                    count++;
                }
            }
            return count;
            // op.SendSuccessMessage("研究数据仅保存在服务器上，每张地图的研究数据是分开的");
        }

        public static void Clear() { }
    }
}