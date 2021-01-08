using Hermes;
using Hermes.Messengers;
using SPR.Containers;
using SPR.Graph;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Unbroken.LaunchBox.Plugins.Data;
using prop = SPR.Properties.Settings;

namespace SPR
{
    /*
     * Point d'entrée pour faire tourner le plugin comme un programme afin de débugger
     */
    [HermesAttr]
    public class DebugPoint
    {

        /// <summary>
        /// Pseudo PlatformsList
        /// </summary>
        public static ObservableCollection<IPlatform> FakePlatforms { get; private set; }

        [STAThread]
        public static void Main()
        {
            Global.DebugMode = true;

            Global.SetPluginLocation();

            Global.InitializeConfig();

            // Initialisation du messager
            MeDebug meD = new MeDebug()
            {
                //Prefix = "",
                LogLevel = 10,
                ByPass = true,

            };
            //meD.AddCaller(prefix: "Debug >>>");

            MeSimpleLog meSL = new MeSimpleLog()
            {
                LogLevel = Global.Config.LogLvl,
                ByPass = true,
                FuncPrefix = Hermes.Cont.EPrefix.Horodating | Hermes.Cont.EPrefix.Prefixing

            };
            meSL.ActivateFileLog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, prop.Default.LogFolder, prop.Default.LogFile));
            meSL.AddCaller(prefix: "DeDebug >>>");

            HeTrace.AddMessenger("Debug", meD);
            HeTrace.AddLogger("Logger", meSL);

            Hermes.HeTrace.WriteLine("Test");

            // On indique manuellement en debug
            Global.LaunchBoxPath = @"G:\Frontend\LaunchBox\Core\";

            string[] tmp = Global.LaunchBoxPath.Split(@"\");

            Global.SetLaunchBoxRoot();


            // Création de fausses plateformes
            MvPlatform plat1 = new MvPlatform() { Name = "Don't exists/proche reel", Folder = @"G:\Ailleurs\Games\Sega Genesis" };
            MvPlatform plat2 = new MvPlatform() { Name = "Nintendo Entertainment System", Folder = @"i:\Games\NES" };
            MvPlatform plat3 = new MvPlatform() { Name = "Sega Master System", Folder = @"" };
            MvPlatform plat4 = new MvPlatform() { Name = "Sega Master System", Folder = @"../../Plateformes/Games/Sega Master System" };
            MvPlatform plat5 = new MvPlatform() { Name = "different letter/proche reel", Folder = @"m:\Ailleurs\Games\Sega Genesis" };
            MvPlatform plat6 = new MvPlatform() { Name = "Racine de Disque", Folder = @"..\..\Games\Sega Genesis" };
            MvPlatform plat7 = new MvPlatform() { Name = "Normal", Folder = @"..\..\Plateformes\Games\Sega Mega Drive" };
            MvPlatform plat8 = new MvPlatform() { Name = "Metzo", Folder = @"..\Plateformes\Games\Sega Genesis" };
            MvPlatform plat9 = new MvPlatform() { Name = "racine", Folder = @"..\..\Games\Sega Mega Drive" };


            FakePlatforms = new ObservableCollection<IPlatform>();
            FakePlatforms.Add(plat1);
            FakePlatforms.Add(plat2);
            FakePlatforms.Add(plat3);
            FakePlatforms.Add(plat4);
            FakePlatforms.Add(plat5);
            FakePlatforms.Add(plat6);
            FakePlatforms.Add(plat7);
            FakePlatforms.Add(plat8);
            FakePlatforms.Add(plat9);

            //
            W_PlateformsList wpl = new W_PlateformsList();
            wpl.ShowDialog();
        }






    }
}
