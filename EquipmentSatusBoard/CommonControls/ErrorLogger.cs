using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentSatusBoard.CommonControls
{
    internal class ErrorLogger
    {
        internal static void LogError(string errorMessage, Exception e)
        {
            string errorFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                Properties.Settings.Default.AppDataFolder + Properties.Settings.Default.ErrorLogFilename;

            using (StreamWriter writer = new StreamWriter(errorFile, true))
            {
                writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HHmm"));
                writer.WriteLine(errorMessage);
                writer.WriteLine(e.Source);
                writer.WriteLine(e.StackTrace);
                writer.WriteLine();
            }
        }
    }
}
