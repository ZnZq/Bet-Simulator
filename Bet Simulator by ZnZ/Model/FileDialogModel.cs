using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Bet_Simulator_by_ZnZ.Model
{
    public static class FileDialogModel
    {
        public static string OpenFile(OpenFileDialog dialog)
        {
            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            return string.Empty;
        }

        public static string[] OpenFiles(OpenFileDialog dialog)
        {
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
                return dialog.FileNames;
            return new string[0];
        }

        public static string SaveFile(SaveFileDialog dialog)
        {
            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            return string.Empty;
        }
    }
}
