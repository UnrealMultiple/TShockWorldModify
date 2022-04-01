using System;

namespace WorldModify
{
    class utils
    {
        public static string CFlag(bool foo, string fstr)
        {
            if (foo)
                return $"[c/96FF96:✔{fstr}]";
            else
                return $"-{fstr}";
        }


        public static bool ToGuid(string str)
        {
            Guid gv = new Guid();
            try
            {
                gv = new Guid(str);
            }
            catch (Exception)
            {

            }
            if (gv != Guid.Empty)
                return true;
            else
                return false;
        }

        public static int GetUnixTimestamp
        {
            get { return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; }
        }
    }
}