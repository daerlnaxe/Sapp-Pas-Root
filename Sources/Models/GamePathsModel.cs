using AsyncProgress.Cont;
using AsyncProgress.Tools;
using DxPaths.Windows;
using DxTBoxCore.Box_MBox;
using DxTBoxCore.Box_Progress;
using Hermes;
using Hermes.Messengers;
using SPR.Containers;
using SPR.Cores;
using SPR.Enums;
using SPR.Languages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Models
{
    /// <summary>
    /// Model for Game Path window
    /// </summary>
    /// <remarks>
    /// On a un retest quand:
    ///  - On change le mode
    ///  - On check ou uncheck les additionnal apps.
    ///  
    /// C'est géré par la fenêtre, pas par le modele
    /// </remarks>
    partial class GamePathsModel : IDisposable
    {
        public MeVerbose Mev { get; private set; }
        #region Avec Notifications

        //bool ToReplaceGiven = false;
        /*     private string _ToReplace;
             /// <summary>
             /// Permet d'affiner les modifications
             /// </summary>
             public string ToReplace
             {
                 get { return _ToReplace; }
                 set
                 {
                     if (value.Equals(_ToReplace))
                         return;

                     _ToReplace = value;
                     ActiveApply = false;
                     ActiveSimulate = true;

                     RemoveError();


                     if (value.EndsWith('\\'))
                         AddError(SPRLang.Err_FinishSlash);

                     OnPropertyChanged();

                 }
             }*/

        #endregion

        #region Sans notification
        /// <summary>
        /// Apply mode activated
        /// </summary>
        internal bool ActiveApply { get; set; }

        internal bool ActiveSimulate { get; set; }

        public ChangeGamePaths Core { get; } = new ChangeGamePaths();

        /// <summary>
        /// Object plateforme passé
        /// </summary>
        public IPlatform SelectedPlatform { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="previousPlatformState">Etat précédent, si disponible</param>
        public GamePathsModel(IPlatform selectedPlatform, string previousPlatformName = null)
        {
            if (selectedPlatform == null)
                throw new NullReferenceException("Object Plateform is null !");

            Core.UpdateStatus += (x, y) => HeTrace.WriteLine(y.Message);
            Core.SelectedPlatform = selectedPlatform;

            SelectedPlatform = selectedPlatform;
            Core.PlatformName = SelectedPlatform.Name;
            Core.ChosenMode = GamePathMode.None;

            if (Global.DebugMode)
                DebugResources.Set_GamesPaths(selectedPlatform.Name);

            //
            Core.AddAppPaths = Properties.Settings.Default.cgpAddApp;

            // Messenger
            Mev = new MeVerbose()
            {
                ByPass = true,
                LogLevel = Global.Config.VerbLvl,
            };
            HeTrace.AddMessenger("Verbose", Mev);
            HeTrace.WriteLine("Initialisation"); // NewLine

            HeTrace.WriteLine($"[CGamePaths] [InitializeEdition] Initialisation");

            HeTrace.WriteLine($@"LaunchBox main path: {Global.LaunchBoxPath}"); // NewLine
            HeTrace.WriteLine($@"Platform '{Core.PlatformName}' selected"); // NewLine


            // --- Initialisation de la plateforme actuelle
            Core.InitializePlatform();

            // --- Récupération des jeux (Async)
            PrepareInitializeGames();

            // --- Initialisation des champs grâce à l'ancienne plateforme            
            if (!string.IsNullOrEmpty(previousPlatformName))
                Core.PlatformToReplace = previousPlatformName;
            else
                Core.PlatformToReplace = FindBadPlatform();


            //    ToReplace = Platform_Tools.GetRoot_ByFolder(PreviousPlatform.Folder);
        }


        #endregion

        // ----

        /// <summary>
        /// Récupère une éventuelle mauvaise plateforme sinon renvoie la plateforme actuelle
        /// </summary>
        internal string FindBadPlatform()
        {
            string tmp = string.Empty;
            string previous = string.Empty;
            foreach (var game in Core.ExtPlatformGames)
            {

                // Gestion d'éventuelle erreur
                if (game.ApplicationPath == null)
                    continue;

                // On passe les jeux déjà ok
                if (game.ApplicationPath.RelatPath.Contains(Core.PlatformRelatPath))
                    continue;

                tmp = Path.GetFileName(Path.GetDirectoryName(game.ApplicationPath.OldPath));

                //
                if (tmp.Equals(previous, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else
                {
                    previous = tmp;
                    tmp = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(tmp))
                tmp = Core.PlatformName;

            return tmp;
        }


        #region
        /*
                internal void NewFindPivot()
                {
                    for (int i = 0; i < ExtPlatformGames.Count; i++)
                    {
                        C_Game game = ExtPlatformGames[i];

                        // Gestion d'éventuelle erreur
                        if (game.ApplicationPath == null)
                            continue;

                        // On passe les jeux déjà ok
                        if (game.ApplicationPath.RelatPath.Contains(PlatformRelatPath))
                            continue;

                        string[] arr = game.ApplicationPath.RelatPath.Split(@"\");

                        // Test avec le nom de plateforme
                        if (arr.Contains(PlatformToReplace))
                        {
                            HeTrace.WriteLine($"[Pivot] OldObject choice");
                            var posPlatName = Array.LastIndexOf(arr, SelectedPlatform.Name);
                            ToReplace = String.Join(@"\", arr, 0, posPlatName + 1);
                            return;
                        }

                        // Test pour essayer via le nom du système
                        if (arr.Contains(SelectedPlatform.Name))
                        {
                            HeTrace.WriteLine($"[Pivot] ObjectPlatfotme choice");
                            var posPlatName = Array.LastIndexOf(arr, SelectedPlatform.Name);
                            // Détermination de la position du nom de la plateforme

                            ToReplace = String.Join(@"\", arr, 0, posPlatName + 1);

                            return;
                        }

                        // Voir si dossier null 
                        // On récupère le nom du dernier dossier de la plateforme
                        string lastDir = Path.GetFileName(SelectedPlatform.Folder);
                        if (!string.IsNullOrEmpty(SelectedPlatform.Folder) && game.ApplicationPath.RelatPath.Contains(lastDir))
                        {
                            HeTrace.WriteLine($"[Pivot] Last folder from platform choice");
                            var posPlatName = Array.LastIndexOf(arr, lastDir);
                            ToReplace = String.Join(@"\", arr, 0, posPlatName - 1); // +1 Pour prendre aussi le dernier dossier

                            return;

                        }

                        ToReplace = SelectedPlatform.Folder;
                        HeTrace.WriteLine($"[Pivot] No predict");
                    }
                }

                /// <summary>
                /// 
                /// <summary>
                /// <remarks>
                /// La prédiction est trop difficile, car il y a trop d'incertitudes.
                /// Remplacé par une vérification si le premier jeu à modifier contiendrait le nom du système pour déterminer la chaine à remplacer
                /// Si ce n'est pas le cas on ne mettra rien, il y a une boite pour choisir à la main le dossier
                /// </remarks>
                [Obsolete]
                internal void FindPivot()
                {
                    // Mise en place du système pivot/tail, on donne un mot, on conservera ce qu'il y a après

                    // Récupération du premier qui n'a pas le bon chemin (pour travailler dessus)
                    //C_Game chosenGame = null;
                    for (int i = 0; i < ExtPlatformGames.Count; i++)
                    {
                        C_Game game = ExtPlatformGames[i];

                        // Gestion d'éventuelle erreur
                        if (game.ApplicationPath == null)
                            continue;

                        // On passe les jeux déjà ok
                        if (game.ApplicationPath.RelatPath.Contains(PlatformRelatPath))
                            continue;

                        string[] arr = game.ApplicationPath.RelatPath.Split(@"\");

                        // Test pour essayer via le nom du système
                        if (arr.Contains(SelectedPlatform.Name))
                        {
                            HeTrace.WriteLine($"[Pivot] ObjectPlatfotme choice");
                            var posPlatName = Array.IndexOf(arr, SelectedPlatform.Name);
                            // Détermination de la position du nom de la plateforme

                            ToReplace = String.Join(@"\", arr, 0, posPlatName + 1);

                            return;
                        }

                        // Voir si dossier null 
                        // On récupère le nom du dernier dossier de la plateforme
                        string lastDir = Path.GetFileName(SelectedPlatform.Folder);
                        if (!string.IsNullOrEmpty(SelectedPlatform.Folder) && game.ApplicationPath.RelatPath.Contains(lastDir))
                        {
                            HeTrace.WriteLine($"[Pivot] Last folder from platform choice");
                            var posPlatName = Array.IndexOf(arr, lastDir);
                            ToReplace = String.Join(@"\", arr, 0, posPlatName + 1); // +1 Pour prendre aussi le dernier dossier
                            return;

                        }

                        // Dernier cas on prend le premier application path qui ne correspond pas
                        ToReplace = Path.GetDirectoryName(game.ApplicationPath.RelatPath);
                        HeTrace.WriteLine("[Pivot] Last choice - Edit is strongly recommended");
                        return;
                    }

                    ToReplace = SelectedPlatform.Folder;
                    HeTrace.WriteLine($"[Pivot] No predict");
                }*/

        #endregion

        private void PrepareInitializeGames()
        {
            TaskLauncher tl = new TaskLauncher()
            {
                AutoCloseWindow = true,
                ProgressIHM = new Splash()
                {
                    Model = new EphemProgress(Core),
                },
                MethodToRun = () => Core.InitializeGames()
            };
            tl.Launch(Core);

            HeTrace.RemoveMessenger("mee");
        }

        /// <summary>
        /// Vérifie l'intégrité des jeux (pas de renouvellement des jeux)
        /// </summary>
        public void PrepareCheckAllGames([CallerMemberName] string propertyName = null)
        {
            if (Core.ChosenMode == GamePathMode.None)
                return;

            HeTrace.WriteLine($"--- Check Validity of Games ({propertyName}) ---");

            TaskLauncher tl = new TaskLauncher()
            {
                AutoCloseWindow = true,
                ProgressIHM = new Splash()
                {
                    Model = new EphemProgress(Core),
                },
                MethodToRun = () => Core.CheckAllGames(),
            };
            tl.Launch(Core);

        }

        internal void PrepareSimulation()
        {
            HeTrace.WriteLine($"--- Prepare Simulation ---");

            TaskLauncher tl = new TaskLauncher()
            {
                AutoCloseWindow = true,
                ProgressIHM = new Splash()
                {
                    Model = new EphemProgress(Core),
                },
                MethodToRun = () => Core.Simulation(),
            };
            if (tl.Launch(Core) == true)
            {
                ActiveSimulate = false;
                ActiveApply = true;
            }
        }

        internal void PrepareApply()
        {
            HeTrace.WriteLine($"--- Prepare Apply ---");
            TaskLauncher tl = new TaskLauncher()
            {
                AutoCloseWindow = true,
                ProgressIHM = new Splash()
                {
                    Model = new EphemProgress(Core),
                },
                MethodToRun = () => Core.ApplyChanges(),
            };
            if (tl.Launch(Core) == true)
            {
                DxMBox.ShowDial(SPRLang.Paths_Modified);
            }
            else
            {
                DxMBox.ShowDial("Error");
            }

            ActiveSimulate = true;
            ActiveApply = false;

        }



        /*


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvGame"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        private bool CheckGame(C_Game mvGame, Maw<EphemProgress> maw)
        {
            if (mvGame.ApplicationPath == null)
                return false;


            bool isValide = false;

            mvGame.States.Clear();

            // Check des paths principaux (ne gère pas les images)
            foreach (C_PathsDouble pathO in mvGame.EnumGetPaths)
            {
                //  if (String.IsNullOrEmpty(pathO.OldRelatPath))
                if (pathO == null)
                    continue;

                HeTrace.WriteLine($"\t[CheckGame] {pathO.Type}: {pathO.RelatPath}");
                switch (pathO.Type)
                {
                    // Le premier initialise 'valide'
                    case nameof(PathType.ApplicationPath):
                        //isValide = pathO.OldRelatPath.Contains(DicSystemPaths["Application"]);
                        bool resPath = Test_Path(DicSystemPaths[MediaType.Application].RelatPath, pathO.RelatPath);
                        mvGame.States.Add(new CState("Main Application", resPath));
                        isValide = resPath;
                        break;

                    // Les autres font une opération de bit
                    case nameof(PathType.ManualPath):
                        //isValide &= pathO.OldRelatPath.Contains(DicSystemPaths["Manual"]);
                        bool resMan = Test_Path(DicSystemPaths[MediaType.Manual].RelatPath, pathO.RelatPath);
                        mvGame.States.Add(new CState("Manual", resMan));
                        isValide &= resMan;
                        //HeTrace.EndLine($"{isValide} (Manual): {DicSystemPaths["Manual"]}", 10);
                        break;

                    case nameof(PathType.MusicPath):
                        //isValide &= pathO.OldRelatPath.Contains(DicSystemPaths["Music"]);
                        bool resMusic = Test_Path(DicSystemPaths[MediaType.Music].RelatPath, pathO.RelatPath);
                        mvGame.States.Add(new CState("Music", resMusic));
                        isValide &= resMusic;
                        //HeTrace.EndLine($"{isValide} (Music): {DicSystemPaths["Music"]}", 10);
                        break;

                    case nameof(PathType.VideoPath):
                        bool resVideo = Test_Path(DicSystemPaths[MediaType.Video].RelatPath, pathO.RelatPath);
                        mvGame.States.Add(new CState("Video", resVideo));
                        isValide &= resVideo;
                        break;
                }


                //                valide &= pathO.Original_RLink.Contains(tboxROldPath.Text);
            }

            // Check des chemins additionnels, si les chemins additionnels sont pris en compte
            if (AddAppPaths)
                foreach (AAppPath pathO in mvGame.AddiRomPaths)
                {
                    bool test = Test_Path(DicSystemPaths[MediaType.Application].RelatPath, pathO.RelatPath);
                    mvGame.States.Add(new CState($"Clone id: {pathO.Id}", test));
                    isValide &= test;
                }

            HeTrace.WriteLine($"\tValidity of the game: {isValide}");
            return isValide;
        }*/

        /*
        /// <summary>
        /// Teste la validité d'un chemin en fonction d'un référent
        /// </summary>
        /// <param name="referent">La racine</param>
        /// <param name="toTest">Le chemin vers le fichier</param>
        /// <returns></returns>
        bool Test_Path(string referent, string toTest)
        {
            HeTrace.WriteLine($"\t\treferent: '{referent}'", 10);
            HeTrace.WriteLine($"\t\tto test: '{toTest}'", 10);

            if (!string.IsNullOrEmpty(toTest) && string.IsNullOrEmpty(referent))
            {
                HeTrace.WriteLine($"\t\tResult: false - referent is null string", 10);
                return false;
            }

            #region 05/03/2021
            // dans le cas forcé on n'accepte pas les sous dossiers
            if (ChosenMode == GamePathMode.Forced)
            {
                string predictedP = $@"{referent}\{Path.GetFileName(toTest)}";
                if (toTest.Equals(predictedP))
                {
                    HeTrace.WriteLine($"\t\tResult: true", 10);
                    return true;
                }

            }
            // Dans le cas des keepsubfolders on accepte les mix
            else if (ChosenMode == GamePathMode.KeepSubFolders)
            {
                if (toTest.Contains(referent))
                {
                    HeTrace.WriteLine($"\t\tResult: true", 10);
                    return true;
                }

            }
            else
            {
                HeTrace.WriteLine($"\t\tResult: false - ChosenMode is set on None", 10);
                return false;

            }
            #endregion 05/03/2021

            HeTrace.WriteLine($"\t\tResult: false", 10);
            return false;
        }
        */



        /// <summary>
        /// Compare vieux paths avec nouveau et détermine s'il y aura une modification à faire.
        /// </summary>
        /// <param name="pathO"></param>
        [Obsolete]
        private void Check_IfModifyRequired(C_PathsDouble pathO)
        {
            HeTrace.WriteLine($"\tHPath: {pathO.HardPath}", 10);
            HeTrace.WriteLine($"\tNewHPath: {pathO.NewHardPath}", 10);
            HeTrace.WriteLine($"\tRPath: {pathO.RelatPath}", 10);
            HeTrace.WriteLine($"\tNewRPath: {pathO.NewRelatPath}", 10);

            // Si les paths sont égaux pas besoin de modifier
            pathO.ToModify = !pathO.Test_Validity();
            HeTrace.WriteLine($"\tTestIMN: {pathO.ToModify}", 10);

            // On montre que les données n'ont pas besoin d'être modifiées
            if (!pathO.ToModify)
            {
                pathO.NewHardPath = SPRLang.No_Modif;
                pathO.NewRelatPath = SPRLang.No_Modif;
            }
        }





        public void Dispose()
        {
            HeTrace.RemoveMessenger("Verbose");
            Properties.Settings.Default.cgpAddApp = Core.AddAppPaths;
            Properties.Settings.Default.Save();
        }



    }
}

