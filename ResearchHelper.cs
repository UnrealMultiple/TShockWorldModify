using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria.GameContent.Creative;


namespace WorldModify
{
    class ResearchHelper
    {
        public static void unlockAll(TSPlayer op)
        {
            List<int> ids = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys.ToList<int>();
            if( ids.Count>0 )
                op.SendInfoMessage("正在解锁，需要一点时间，请稍等……");

            int amountNeeded;
            for (int i = 0; i < ids.Count; i++)
            {
                CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(ids[i], out amountNeeded);
                TShock.ResearchDatastore.SacrificeItem(ids[i], amountNeeded, op);
            }
            op.SendSuccessMessage("已解锁 {0} 个物品研究，重新开服可生效", ids.Count);
            op.SendSuccessMessage("研究数据仅保存在服务器上，每张地图的研究数据是分开的");
            TSPlayer.All.SendData(PacketTypes.PlayerInfo);
        }

        public static void reset()
        {
        }

        public static int GetSacrificeTotal()
        {
            return CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys.Count;
        }

        public static int GetSacrificeCount()
        {
            Dictionary<int, int> datas = TShock.ResearchDatastore.GetSacrificedItems();
            int count = 0;
            foreach (int key in datas.Keys)
            {
                int amount = datas[key];
                CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(key, out int amountNeeded);
                if( amount>=amountNeeded ){
                    count++;
                }
            }
            return count;
            // op.SendSuccessMessage("研究数据仅保存在服务器上，每张地图的研究数据是分开的");
        }

        public static void Clear(){}
    }
}