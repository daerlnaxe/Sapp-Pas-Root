using Hermes;
using Hermes.Cont;
using Hermes.Messengers;
using SPR.Graph;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using Unbroken.LaunchBox.Plugins;
using Sett = SPR.Properties.Settings;

namespace SPR
{
    public class SappPasRoot_Plugin : ISystemMenuItemPlugin
    {
        /// <summary>
        /// Titre du plugin
        /// </summary>
        public string Caption => "SappPasRoot"; // TODO Languages.Lang.Plugin_Title;

        /// <summary>
        /// Montrer en mode bigbox
        /// </summary>
        public bool ShowInBigBox => false;

        /// <summary>
        /// 
        /// </summary>
        public bool AllowInBigBoxWhenLocked => false;

        /// <summary>
        /// Montrer dans Launchbox
        /// </summary>
        public bool ShowInLaunchBox => true;

        /// <summary>
        /// Icone de la dll pour le menu
        /// </summary>
        public Image IconImage => null;

        public void OnSelected()
        {
            Directory.SetCurrentDirectory(Global.LaunchBoxPath);

            Global.InitializeConfig();

            //return;
            try
            {

                /*TextWriterTraceListener textWriter = new TextWriterTraceListener(@".././Logs/SappPasRoot.log");
                //Ajout bit à bit de deux options de sortie
                textWriter.TraceOutputOptions = TraceOptions.Callstack | TraceOptions.ProcessId | TraceOptions.Timestamp;

                Trace.Listeners.Add(textWriter);
                Trace.AutoFlush = true;
                Trace.WriteLine($"\n {new string('=', 10)} Initialization {new string('=', 10)}");*/

                MeSimpleLog meSL = new MeSimpleLog(Path.Combine(Global.LaunchBoxRoot, Sett.Default.LogFolder, Sett.Default.LogFile))
                {
                    //Prefix
                    LogLevel = Global.Config.LogLvl,
                    ByPass = true,
                    FuncPrefix = EPrefix.Prefixing | EPrefix.Horodating
                };

                HeTrace.AddLogger("Logger", meSL);
                HeTrace.WriteLine("Init ok", callerName: "Logger");
                HeTrace.WriteLine($"LaunchBox Path: {Global.LaunchBoxPath}");
                HeTrace.WriteLine($"LaunchBox Root (found): {Global.LaunchBoxRoot}");

                //PluginHelper. .LaunchBoxMainForm.FormClosing += new FormClosingEventHandler(Fermeture);

                // 2020/30/10 verif
                W_PlateformsList sp = new W_PlateformsList();
                sp.ShowDialog();
                // 2020/30/10 verif
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                // Trace.WriteLine(e);
                using (StreamWriter file = new StreamWriter(@".././Logs/#err.txt"))
                {
                    file.WriteLine(e);
                }

            }
            HeTrace.RemoveLogger("Logger");
        }

        public SappPasRoot_Plugin()
        {

            // Attribution du chemin d'accès de Launchbox (différent en mode débug)
            Global.LaunchBoxPath = AppDomain.CurrentDomain.BaseDirectory;

            Global.SetLaunchBoxRoot();

            Global.SetPluginLocation();
        }
    }
}
