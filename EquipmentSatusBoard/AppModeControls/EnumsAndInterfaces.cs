namespace EquipmentSatusBoard.AppModeControls
{
    public enum AppMode
    {
        Slide,
        Tech,
        Admin
    }

    public interface IAppMode
    {
        void SetMode(AppMode newMode);
    }

    public struct AppModePassword
    {
        public AppMode Mode;
        public byte[] Password;
    }
}
