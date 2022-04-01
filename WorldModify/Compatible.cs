using TShockAPI;

namespace WorldModify
{
    class Compatible
    {
        public static string DefaultRegistrationGroupName
        {
            // 1.4.0.5
            // get { return TShock.Config.DefaultRegistrationGroupName; }

            // 1.4.2.1
            get { return TShock.Config.Settings.DefaultRegistrationGroupName; }
        }

        public static bool IsGuest(TSPlayer op)
        {
            // 1.4.0.5
            // return op.Group.Name == TShock.Config.DefaultGuestGroupName;

            // 1.4.2.1
            return op.Group.Name == TShock.Config.Settings.DefaultGuestGroupName;
        }
    }
}