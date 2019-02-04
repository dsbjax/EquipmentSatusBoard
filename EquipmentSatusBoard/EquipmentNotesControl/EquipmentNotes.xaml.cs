using EquipmentSatusBoard.AppModeControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EquipmentSatusBoard.EquipmentNotesControl
{
    /// <summary>
    /// Interaction logic for EquipmentNotes.xaml
    /// </summary>
    public partial class EquipmentNotes : UserControl, IAppMode
    {
        private const string EQUIPMENT_NOTES_FILENAME = "esb.notes";
        public EquipmentNotes()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();

            LoadEquipmentNotes();
            AppModeNotifications.Subscribe(this);
        }

        public void SetMode(AppMode newMode)
        {
            equipmentNotes.IsReadOnly = newMode == AppMode.Slide;
        }

        private void LoadEquipmentNotes()
        {
            if (File.Exists(EQUIPMENT_NOTES_FILENAME))
                using (StreamReader reader = new StreamReader(EQUIPMENT_NOTES_FILENAME))
                    equipmentNotes.Text = reader.ReadToEnd();
        }

        internal void SaveEquipmentNotes()
        {
            using (StreamWriter writer = new StreamWriter(EQUIPMENT_NOTES_FILENAME))
                writer.Write(equipmentNotes.Text);
        }
    }
}
