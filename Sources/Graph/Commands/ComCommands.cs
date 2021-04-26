using DxTBoxCore.Box_MBox;
using SPR.Languages;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SPR.Graph.Commands
{
    class ComCommands
    {
        public static readonly RoutedCommand Browse = new RoutedCommand("Browse", typeof(ComCommands));
        public static readonly RoutedCommand Simulate = new RoutedCommand("Simulate", typeof(ComCommands));
        public static readonly RoutedCommand Apply = new RoutedCommand("Apply", typeof(ComCommands));
        public static readonly RoutedCommand Stop = new RoutedCommand("Stop", typeof(ComCommands));
        public static readonly RoutedUICommand OpenInExplorer = new RoutedUICommand(SPRLang.Open_In_Explorer, "OpenInExplorer_Command", typeof(ComCommands));

        /// <summary>
        /// Open path in explorer
        /// </summary>
        /// <param name="targetPath"></param>
        internal static void OpenInExplorer_Command(string targetPath)
        {
            try
            {
                if (System.IO.Directory.Exists(targetPath))
                    Process.Start("explorer.exe", targetPath);
                else
                {
                    DxMBox.ShowDial($"{SPRLang.Path_D_Exist}: '{targetPath}'");

                }

            }
            catch (Exception exc)
            {

            }

        }
    }

}
