using EquipmentSatusBoard.AppModeControls;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using EquipmentSatusBoard.Forms;
using System;
using System.ComponentModel;
using System.Windows.Input;
using EquipmentSatusBoard.CommonControls;

namespace EquipmentSatusBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IAppMode
    {
        private static string SAVED_PASSWORDS_FOLDER =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
            Properties.Settings.Default.AppDataFolder;

        private static string SAVED_PASSWORDS_FILE = 
            SAVED_PASSWORDS_FOLDER + Properties.Settings.Default.PasswordFilename;

        private const int PASSWORD_BYTE_SIZE = 32;

        private AppModeNotifications modeChangeNotifications = new AppModeNotifications();
        private AppModePassword[] savedPasswords;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                savedPasswords = LoadPasswords();
                AppModeNotifications.Subscribe(this);

                Events();
                modeChangeNotifications.Broadcast(AppMode.Slide);

            }catch(Exception e)
            {
                ErrorLogger.LogError("Error Initializing Main Window, MainWindow()", e);
                Close();
            }
        }

        private AppModePassword[] LoadPasswords()
        {
            if (!File.Exists(SAVED_PASSWORDS_FILE))
                return CreatePasswords();

            AppModePassword[] passwordsFromFile = new AppModePassword[2];

            try
            {
                using (FileStream passwordsReader = new FileStream(SAVED_PASSWORDS_FILE, FileMode.Open, FileAccess.Read))
                {
                    byte[] adminPassword = new byte[32];
                    byte[] techPassword = new byte[32];

                    passwordsReader.Read(adminPassword, 0, PASSWORD_BYTE_SIZE);
                    passwordsReader.Read(techPassword, 0, PASSWORD_BYTE_SIZE);
                    passwordsReader.Close();

                    passwordsFromFile[0] = new AppModePassword() { Mode = AppMode.Admin, Password = adminPassword };
                    passwordsFromFile[1] = new AppModePassword() { Mode = AppMode.Tech, Password = techPassword };
                }
            }catch(Exception e)
            {
                ErrorLogger.LogError("Error Loading Passwords, MainWindow.LoadPasswords()", e);
                throw e;
            }

            return passwordsFromFile;
        }

        private void Events()
        {
            mainStatusBar.AppModeChangeRequest += AppModeChangeRequest;
        }

        private void AppModeChangeRequest(object sender, System.EventArgs e)
        {
            var dialog = new AppModeChangeDialog();

            if (ValidatePassword(dialog.GetModeRequest()))
                modeChangeNotifications.Broadcast(dialog.NewMode);
        }

        private bool ValidatePassword(AppMode appMode)
        {
            if (appMode == AppMode.Slide) return true;

            var dialog = new GetModePasswordDialog();
            SHA256 hash = SHA256.Create();
            byte[] password;
            bool match = true;

            try
            {
                password = hash.ComputeHash(Encoding.UTF8.GetBytes(dialog.ShowDialog(appMode)));

                if (password.Length != PASSWORD_BYTE_SIZE || savedPasswords[0].Password.Length != PASSWORD_BYTE_SIZE || savedPasswords[1].Password.Length != PASSWORD_BYTE_SIZE)
                    return false;

                for (int i = 0; i < PASSWORD_BYTE_SIZE; i++)
                    match &= savedPasswords[appMode == AppMode.Admin ? 0 : 1].Password[i] == password[i];

                if (!match) new IncorrectPasswordDialog().ShowDialog();
            }
            catch (Exception e)
            {
                ErrorLogger.LogError("Error Validating Password, MainWIndow.ValidatePassword(AppMode)", e);
                return false;
            }

            return match;
        }


        private AppModePassword[] CreatePasswords()
        {
            try
            {
                var dialog = new CreatePasswordsDialog();

                if ((bool)dialog.ShowDialog())
                    using (SHA256 hash = SHA256.Create())
                    {
                        byte[] adminPassword = new byte[32], techPassword = new byte[32];
                        AppModePassword[] passwords = new AppModePassword[2];

                        adminPassword = hash.ComputeHash(Encoding.UTF8.GetBytes(dialog.AdminPassword));
                        techPassword = hash.ComputeHash(Encoding.UTF8.GetBytes(dialog.TechPassword));

                        passwords[0] = new AppModePassword() { Mode = AppMode.Admin, Password = adminPassword };
                        passwords[1] = new AppModePassword() { Mode = AppMode.Tech, Password = techPassword };

                        SavePasswords(passwords);

                        return passwords;
                    }
            }catch(Exception e)
            {
                ErrorLogger.LogError("Error Creating Passwords, MainWindow.CreatePasswords", e);
            }

            return null;
        }

        private void SavePasswords(AppModePassword[] passwords)
        {
            try
            {
                if (!Directory.Exists(SAVED_PASSWORDS_FOLDER)) Directory.CreateDirectory(SAVED_PASSWORDS_FOLDER);
                using (FileStream writer = new FileStream(SAVED_PASSWORDS_FILE, FileMode.Create, FileAccess.Write))
                {
                    writer.Write(passwords[0].Password, 0, PASSWORD_BYTE_SIZE);
                    writer.Write(passwords[1].Password, 0, PASSWORD_BYTE_SIZE);
                    writer.Close();
                }
            }catch(Exception e)
            {
                ErrorLogger.LogError("Error Saving Passswords, MainWindow.SavePasswords(AppModePassword[])", e);
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
            statusBoard.Save();

            base.OnClosing(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.X && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                Close();

            if (e.Key == Key.Escape)
                modeChangeNotifications.Broadcast(AppMode.Slide);

            if (e.Key == Key.F2)
                if (ValidatePassword(AppMode.Tech))
                    modeChangeNotifications.Broadcast(AppMode.Tech);

            if (e.Key == Key.F3)
                if (ValidatePassword(AppMode.Admin))
                    modeChangeNotifications.Broadcast(AppMode.Admin);

            base.OnKeyDown(e);
        }
    }
}
