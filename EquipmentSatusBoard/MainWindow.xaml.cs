using EquipmentSatusBoard.AppModeControls;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using EquipmentSatusBoard.Forms;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace EquipmentSatusBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IAppMode
    {
        private static string PASSWORD_FOLDER =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
            Properties.Settings.Default.AppDataFolder;

        private static string PASSWORD_FILE = PASSWORD_FOLDER + Properties.Settings.Default.PasswordFilename;

        private const int PASSWORD_BYTE_SIZE = 32;

        private AppModeNotifications appModeNotifications = new AppModeNotifications();
        private AppModePassword[] passwords;

        public MainWindow()
        {
            InitializeComponent();

            passwords = LoadPasswords();
            AppModeNotifications.Subscribe(this);

            Events();
            statusBoard.LoadStatusPages();
            appModeNotifications.Broadcast(AppMode.Slide);
        }

        private void Events()
        {
            mainStatusBar.AppModeChangeRequest += AppModeChangeRequest;
        }

        private void AppModeChangeRequest(object sender, System.EventArgs e)
        {
            var dialog = new AppModeChangeDialog();

            if (ValidatePassword(dialog.GetModeRequest()))
                appModeNotifications.Broadcast(dialog.NewMode);
        }

        private bool ValidatePassword(AppMode appMode)
        {
            if (appMode == AppMode.Slide) return true;

            var dialog = new GetModePasswordDialog();

            SHA256 hash = SHA256.Create();
            byte[] password = hash.ComputeHash(Encoding.UTF8.GetBytes(dialog.ShowDialog(appMode)));

            bool match = true;
            for (int i = 0; i < PASSWORD_BYTE_SIZE; i++)
                match &= passwords[appMode == AppMode.Admin ? 0 : 1].Password[i] == password[i];

            if (!match) new IncorrectPasswordDialog().ShowDialog();

            return match;
        }

        private AppModePassword[] LoadPasswords()
        {
            if (!File.Exists(PASSWORD_FILE))
                return CreatePasswords();

            AppModePassword[] passwords = new AppModePassword[2];

            using (FileStream reader = new FileStream(PASSWORD_FILE, FileMode.Open, FileAccess.Read))
            {
                byte[] adminPassword = new byte[32];
                byte[] techPassword = new byte[32];

                reader.Read(adminPassword, 0, PASSWORD_BYTE_SIZE);
                reader.Read(techPassword, 0, PASSWORD_BYTE_SIZE);
                reader.Close();

                passwords[0] = new AppModePassword() { Mode = AppMode.Admin, Password = adminPassword };
                passwords[1] = new AppModePassword() { Mode = AppMode.Tech, Password = techPassword };
            }

            return passwords;
        }

        private AppModePassword[] CreatePasswords()
        {
            var dialog = new CreatePasswordsDialog();

            if((bool)dialog.ShowDialog())
                using (SHA256 hash = SHA256.Create())
                {
                    byte[] adminPassword = new byte[32], techPassword = new byte[32];
                    AppModePassword[] passwords = new AppModePassword[2];

                    adminPassword = hash.ComputeHash(Encoding.UTF8.GetBytes(dialog.AdminPassword));
                    techPassword = hash.ComputeHash(Encoding.UTF8.GetBytes(dialog.TechPassword));

                    passwords[0] = new AppModePassword() { Mode = AppMode.Admin, Password = adminPassword };
                    passwords[1] = new AppModePassword() { Mode = AppMode.Tech, Password = techPassword };

                    SavePassword(passwords);

                    return passwords;
                }

            return null;
        }

        private void SavePassword(AppModePassword[] passwords)
        {
            if (!Directory.Exists(PASSWORD_FOLDER)) Directory.CreateDirectory(PASSWORD_FOLDER);
            using (FileStream writer = new FileStream(PASSWORD_FILE, FileMode.Create, FileAccess.Write))
            {
                writer.Write(passwords[0].Password, 0, PASSWORD_BYTE_SIZE);
                writer.Write(passwords[1].Password, 0, PASSWORD_BYTE_SIZE);
                writer.Close();
            }
        }

        public void SetMode(AppMode newMode)
        {
            if (newMode == AppMode.Admin)
                WindowStyle = WindowStyle.SingleBorderWindow;
            else
                WindowStyle = WindowStyle.None;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            mainStatusBar.SavePhoneNumbers();
            radarStatusControl.Save();
            //equipmentNotes.SaveEquipmentNotes();
            statusBoard.Save();

            base.OnClosing(e);
        }

        private void CheckForQuit(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.X && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                Close();
        }
    }
}
