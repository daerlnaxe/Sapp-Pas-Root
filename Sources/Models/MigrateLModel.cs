using AsyncProgress.Tools;
using DxLocalTransf.Copy;
using DxPaths.Windows;
using DxTBoxCore.Box_Progress;
using HashCalc;
using Hermes;
using SPR.Containers;
using SPR.Cores;
using SPR.Graph;
using SPR.Languages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Models
{
    public class MigrateLModel
    {
        public IPlatform OldPlatform { get; private set; }
        public IPlatform CurrentPlatform { get; private set; }

        /*public ModelSD GamesPaths { get; set; } = new ModelSD(SPRLang.Games);

        public ModelSD ManualsPaths { get; set; } = new ModelSD(SPRLang.Manuals);

        public ModelSD ImagesPaths { get; set; } = new ModelSD(SPRLang.Images);

        public ModelSD MusicsPaths { get; set; } = new ModelSD(SPRLang.Musics);

        public ModelSD VideosPaths { get; set; } = new ModelSD(SPRLang.Videos);


        public string OldImageRootPath { get; set; }
        public string CurrentImageRootPath { get; set; }*/



        public C_Platform VisPlatform { get; set; }

        public MigrateLModel(C_Platform backupPlatform, IPlatform currentPlatform)
        {
            CurrentPlatform = currentPlatform;
            ///OldPlatform = oldPlatform;
            VisPlatform = backupPlatform;


            //HeTrace.WriteLine($"Old Platform path '{oldPlatform.app}'");
            HeTrace.WriteLine($"Current Platform path '{CurrentPlatform.Folder}'");


            //VisPlatform = C_Platform.Platform_Maker(oldPlatform, oldPlatform.GetAllPlatformFolders());

            VisPlatform.ApplicationPath.NewRelatPath = currentPlatform.Folder;
            VisPlatform.ApplicationPath.NewHardPath = Path.GetFullPath(currentPlatform.Folder, Global.LaunchBoxRoot);

            foreach (var elem in VisPlatform)
            {
                foreach (var elem2 in currentPlatform.GetAllPlatformFolders())
                    if (elem2.MediaType.Equals(elem.Type))
                    {
                        elem.NewHardPath = Path.GetFullPath(elem2.FolderPath, Global.LaunchBoxRoot);
                        elem.NewRelatPath = DxPath.To_RelativeOrNull(Global.LaunchBoxRoot, elem.NewHardPath);
                    }
            }


            /*Platform_Tools.SetFolders(in oldPlatform);

            Platform_Tools.SetFolders(in currentPlatform);*/
            //oldPlatform.

            //HeTrace.WriteLine($"default boximagepath '{oldPlatform.DefaultBoxImagePath}'");


            // Initialization des dossiers
            // Initialization according to the mode (debug/plugin)
            /*if (Global.DebugMode)
                // Utilisation de pseudos dossiers
                _PlatformFolders = ((MvPlatform)PlatformObject).GetAllPlatformFolders();
            else
                // Récupération de tous les dossiers + tri
                _PlatformFolders = PlatformObject.GetAllPlatformFolders()
                                        .OrderBy(x => x.MediaType).ToArray();*/

            // Games
            /*            GamesPaths.Source = oldPlatform.Folder;
                        GamesPaths.Destination = currentPlatform.Folder;
                        HeTrace.WriteLine(oldPlatform.Folder, 5);
                        HeTrace.WriteLine(currentPlatform.Folder, 5);

                        /*  // Images
                          ImagesPaths.Source = Path.GetDirectoryName(OldPlatform.DefaultBoxImagePath);
                          ImagesPaths.Destination = Path.GetDirectoryName(CurrentPlatform.DefaultBoxImagePath);*/
            // Manuals
            /*     ManualsPaths.Source = oldPlatform.ManualsFolder;
                 ManualsPaths.Destination = currentPlatform.ManualsFolder;
                 // Musics
                 MusicsPaths.Source = oldPlatform.MusicFolder;
                 MusicsPaths.Destination = currentPlatform.MusicFolder;
                 // Videos 
                 VideosPaths.Source = oldPlatform.VideosFolder;
                 VideosPaths.Destination = currentPlatform.VideosFolder;*/
        }

        internal void Exec()
        {
            IEnumerable<DataTrans> datas = GetFiles();


            HeTrace.WriteLine("Copy");
            HashCopy hashCopy = new HashCopy();
            hashCopy.AskToUser += SafeBoxes.HashCopy_AskToUser;
            hashCopy.UpdateStatus += (x, y) => HeTrace.WriteLine(y.Message);
            PersistProgressD mee = new PersistProgressD(hashCopy);

            TaskLauncher launcher = new TaskLauncher()
            {
                ProgressIHM = new DxDoubleProgress(mee),
                AutoCloseWindow = true,
                MethodToRun = () => hashCopy.VerifSevNCopy(datas),


            };
            launcher.Launch(hashCopy);

        }


        /// <summary>
        /// Récupération des fichiers (vérification si le dossier source existe à chaque fois)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DataTrans> GetFiles()
        {
            List<DataTrans> datas = new List<DataTrans>();

            foreach (var platFolder in VisPlatform)
            {
                if (!Directory.Exists(platFolder.HardPath))
                {
                    HeTrace.WriteLine($"Folder doesn't exist '{platFolder.HardPath}'", 5);
                    continue;
                }                


                foreach (string f in Directory.EnumerateFiles(platFolder.HardPath, "*.*", SearchOption.AllDirectories))
                {
                    // Cas particulier du theme video
                    if (platFolder.Type.Equals("Video", StringComparison.OrdinalIgnoreCase) &&
                        f.Contains("Theme")
                        )
                        continue;


                    DataTrans dt = new DataTrans()
                    {
                        Name = Path.GetFileName(f),
                        CurrentPath = f,
                        DestPath = f.Replace(platFolder.HardPath, platFolder.NewHardPath),                    
                    };

                    HeTrace.WriteLine($"Copy from: {dt.CurrentPath}",10);
                    HeTrace.WriteLine($"To: {dt.DestPath}",10);

                    datas.Add(dt);
                }


            }
            return datas;
        }

    }
}
