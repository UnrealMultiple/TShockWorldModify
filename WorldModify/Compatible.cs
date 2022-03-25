using TShockAPI;
using System;
using System.Reflection;
using TShockAPI.Configuration;

namespace WorldModify
{
    class Compatible
    {
        // public static Boolean RequireLogin
        // {
        //     // 1.4.0.5
        //     // https://github.com/Pryaxis/TShock/blob/f538ceb79371776afa386e9bc7648366f16b897c/TShockAPI/DB/GroupManager.cs

        //     // 1.4.0.5
        //     // get { return TShock.Config.RequireLogin; }
        //     // get { return TShock.Config.Settings.RequireLogin; }

        //     // TShockSettings
        //     get
        //     {
        //         try
        //         {
        //             bool result = (bool) typeof(ConfigFile).InvokeMember("RequireLogin", BindingFlags.Public | BindingFlags.GetProperty, null, null, null);
        //             return result;
        //         }
        //         #pragma warning disable 0168
        //         catch (MissingMethodException e){}
        //         #pragma warning restore 0168

        //         return false;
        //     }


        // }

        public static String DefaultRegistrationGroupName
        {
            // 1.4.0.5
            // get { return TShock.Config.DefaultRegistrationGroupName; }

            get { return TShock.Config.Settings.DefaultRegistrationGroupName; }
        }

        public static Boolean isGuest(TSPlayer op){
            return op.Group.Name == TShock.Config.Settings.DefaultGuestGroupName;
        }
    }
}