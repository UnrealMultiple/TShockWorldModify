using System;
using System.IO;
using System.Threading;
using Terraria;
using TShockAPI;


namespace WorldModify
{
    public class BackupHelper
    {
        public static string BackupPath { get; set; }


        public static void Backup(TSPlayer op)
        {
            Thread t = new Thread(() =>
            {
                DoBackup(op);
            });
            t.Name = "[wm]Backup Thread";
            t.Start();
        }

        private static void DoBackup(TSPlayer op)
        {
            try
            {
                string worldname = Main.worldPathName;
                string name = Path.GetFileName(worldname);

                Main.ActiveWorldFileData._path = Path.Combine(BackupPath, string.Format("{0}.{1:yyyy-MM-dd_HH.mm.ss}.bak", name, DateTime.Now));

                string worldpath = Path.GetDirectoryName(Main.worldPathName);
                if (worldpath != null && !Directory.Exists(worldpath))
                    Directory.CreateDirectory(worldpath);

                TShock.Log.Info("[wm]正在备份地图...");
                Console.WriteLine("[wm]正在备份地图...");
                op.SendInfoMessage("正在备份地图...");

                TShock.Utils.SaveWorld();

                TShock.Log.Info($"[wm]世界已备份 ({Main.worldPathName})");
                Console.WriteLine($"[wm]世界已备份 ({Main.worldPathName})");
                op.SendInfoMessage($"世界已备份");

                Main.ActiveWorldFileData._path = worldname;
            }
            catch (Exception ex)
            {
                TShock.Log.Error("[wm]备份失败!");
                Console.WriteLine("[wm]备份地图失败!");
                op.SendInfoMessage("备份地图失败！请手动查看日志文件");
                TShock.Log.Error(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
