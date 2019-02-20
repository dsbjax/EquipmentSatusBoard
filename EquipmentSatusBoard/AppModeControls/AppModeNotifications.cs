using System.Collections.Generic;

namespace EquipmentSatusBoard.AppModeControls
{
    internal class AppModeNotifications
    {
        private static List<IAppMode> modeNotificationSubscribers;

        internal AppModeNotifications()
        {
            if (modeNotificationSubscribers == null)
                modeNotificationSubscribers = new List<IAppMode>();
        }

        internal static void Subscribe(IAppMode subscriber)
        {
            if (modeNotificationSubscribers == null)
                modeNotificationSubscribers = new List<IAppMode>();

            modeNotificationSubscribers.Add(subscriber);
        }

        internal static void Unsubscribe(IAppMode subscriber)
        {
            if (modeNotificationSubscribers != null)
                modeNotificationSubscribers.Remove(subscriber);
        }

        internal void Broadcast(AppMode mode)
        {
            foreach (var modeNotificationSubscriber in modeNotificationSubscribers)
                modeNotificationSubscriber.SetMode(mode);
        }
    }
}
