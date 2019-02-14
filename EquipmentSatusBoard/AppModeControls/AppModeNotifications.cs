using System.Collections.Generic;

namespace EquipmentSatusBoard.AppModeControls
{
    internal class AppModeNotifications
    {
        private static List<IAppMode> subscribers;

        internal AppModeNotifications()
        {
            if (subscribers == null)
                subscribers = new List<IAppMode>();
        }

        internal static void Subscribe(IAppMode subscriber)
        {
            if (subscribers == null)
                subscribers = new List<IAppMode>();

            subscribers.Add(subscriber);
        }

        internal void Broadcast(AppMode mode)
        {
            foreach (var subscriber in subscribers)
                subscriber.SetMode(mode);
        }
    }
}
