using DxPaths.Windows;
using DxTBoxCore.Box_MBox;
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
    /// L'altération des paths ne gère pas pour le moment les répertoires images ni le thème video
    /// </remarks>
    partial class GamesModel : INotifyPropertyChanged, IDisposable, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region Avec Notifications

        public MeVerbose Mev { get; private set; }


        private string _PlatformName;
        /// <summary>
        /// Current Name of the plateform
        /// </summary>
        public string PlatformName
        {
            get { return _PlatformName; }
            set
            {
                _PlatformName = value;
                OnPropertyChanged();
            }
        }

        private string _PlatformToReplace;
        public string PlatformToReplace
        {
            get => _PlatformToReplace;
            set
            {


                _PlatformToReplace = value;

                OnPropertyChanged();
                // .... Enclencher un retest
            }
        }

        public string SubStringToRemove { get; set; }

        /// <summary>
        /// Tables des jeux de la plateforme
        /// </summary>
        public ObservableCollection<C_Game> ExtPlatformGames { get; set; } = new ObservableCollection<C_Game>();

        private GamePathMode _ChosenMode;
        public GamePathMode ChosenMode
        {
            get { return _ChosenMode; }
            set
            {
                RemoveError();

                switch (value)
                {
                    case GamePathMode.None:
                        AddError(SPRLang.Err_SelectMode);
                        break;

                        /*   case GamePathMode.KeepSubFolders:
                               //NewFindPivot();
                               break;
                        */

                }

                _ChosenMode = value;
                //CheckAllGames();

                OnPropertyChanged();
            }
        }


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
        internal bool ActiveSimulate { get; set; } = true;

        //public IPlatform PreviousPlatform { get; private set; }

        /// <summary>
        /// Object plateforme passé
        /// </summary>
        public IPlatform SelectedPlatform { get; private set; }



        /// <summary>
        /// Boolean to include hidden apps;
        /// </summary>
        /// <remarks>
        /// Pas de notif
        /// </remarks>
        public bool IncludeHidden { get; set; }

        /// <summary>
        /// Boolean to include Additionnal App Paths
        /// </summary>
        /// <remarks>
        /// example for a game with several roms
        /// </remarks>
        public bool AddAppPaths { get; set; }

        /// <summary>
        /// Platform hard path
        /// </summary>
        public string PlatformHardPath { get; set; }

        /// <summary>
        /// Platform relative path
        /// </summary>
        public string PlatformRelatPath { get; set; }


        /// <summary>
        /// Liste originale des jeux
        /// </summary>
        IGame[] _IPlatformGames;

        /// <summary>
        /// Liste des chemins de la plateforme
        /// </summary>
        public Dictionary<MediaType, C_Paths> DicSystemPaths { get; private set; } =
                                                            new Dictionary<MediaType, C_Paths>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="previousPlatformState">Etat précédent, si disponible</param>
        public GamesModel(IPlatform selectedPlatform, string previousPlatformName = null)
        {
            if (selectedPlatform == null)
                throw new NullReferenceException("Object Plateform is null !");

            // PreviousPlatform = previousPlatformState;
            SelectedPlatform = selectedPlatform;
            PlatformName = SelectedPlatform.Name;
            ChosenMode = GamePathMode.None;

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
            HeTrace.WriteLine($@"Platform '{PlatformName}' selected"); // NewLine


            // --- Initialisation de la plateforme actuelle
            InitializePlatform(selectedPlatform);


            // --- Récupération des jeux
            InitializeGames();

            // --- Initialisation des champs grâce à l'ancienne plateforme            
            if (!string.IsNullOrEmpty(previousPlatformName))
                PlatformToReplace = previousPlatformName;
            else
                PlatformToReplace = FindBadPlatform();


            //    ToReplace = Platform_Tools.GetRoot_ByFolder(PreviousPlatform.Folder);
        }


        #endregion

        /// <summary>
        /// Initialization with a platform (edition)
        /// </summary>
        /// <param name="selectedPlatform"></param>
        /// <remarks>
        /// #1
        /// </remarks>
        internal void InitializePlatform(IPlatform selectedPlatform)
        {
            // --- Remplissage des informations de la plateforme
            HeTrace.WriteLine(@"Filling information fields"); // NewLine

            // Dans le cas où l'on est à la racine
            string folder = SelectedPlatform.Folder;
            if (string.IsNullOrEmpty(folder))
            {
                folder = Path.Combine(Global.LaunchBoxRoot, "Games", SelectedPlatform.Name);
            }

            if (folder.StartsWith('.'))
                PlatformRelatPath = folder;
            else
                PlatformRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, folder);

            HeTrace.WriteLine(@"Transformation RelatLink To HardLink "); // NewLine
            PlatformHardPath = Path.GetFullPath(PlatformRelatPath, Global.LaunchBoxRoot);

            // --- Récupération des dossiers de la plateforme
            InitializePlateformFolders();
        }

        /// <summary>
        /// Construit le dictionnaire pour avoir tous les paths de la plateforme
        /// </summary>
        private void InitializePlateformFolders()
        {
            // --- Init pathfolders
            IPlatformFolder[] platformFolders;
            DicSystemPaths.Clear();

            //
            if (Global.DebugMode)
            {
                // Utilisation de pseudos dossiers
                //  platformFolders = DebugResources.Get_PlatformPaths();
                platformFolders = ((MvPlatform)SelectedPlatform).GetAllPlatformFolders();
            }
            else
            {
                platformFolders = SelectedPlatform.GetAllPlatformFolders();
            }

            DicSystemPaths = Global.Make_DicPlatformPaths(SelectedPlatform.Folder, platformFolders);
        }


        /// <summary>
        /// Initialize games
        /// </summary>
        private void InitializeGames()
        {
            HeTrace.WriteLine("--- Paths of the games Initialization ---"); // NewLine

            if (Global.DebugMode == true)
                _IPlatformGames = DebugResources.Get_GamesPaths();
            else
                _IPlatformGames = SelectedPlatform.GetAllGames(IncludeHidden, true);

            // --- Transformation des jeux
            ExtPlatformGames.Clear();
            foreach (IGame game in _IPlatformGames)
            {
                C_Game tmpGame = new C_Game(game);
                tmpGame.BuildPaths(game);

                // --- Check de la validité du jeu (29/04/2021)
                tmpGame.IsValide = CheckGame(tmpGame);

                ExtPlatformGames.Add(tmpGame);

            }
        }

        // ----

        /// <summary>
        /// Récupère une éventuelle mauvaise plateforme sinon renvoie la plateforme actuelle
        /// </summary>
        internal string FindBadPlatform()
        {
            string tmp = string.Empty;
            string previous = string.Empty;
            foreach (var game in ExtPlatformGames)
            {

                // Gestion d'éventuelle erreur
                if (game.ApplicationPath == null)
                    continue;

                // On passe les jeux déjà ok
                if (game.ApplicationPath.RelatPath.Contains(PlatformRelatPath))
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
                tmp = PlatformName;

            return tmp;
        }


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

        /// <summary>
        /// Vérifie l'intégrité des jeux (pas de renouvellement des jeux)
        /// </summary>
        public void CheckAllGames([CallerMemberName] string propertyName = null)
        {
            if (ChosenMode == GamePathMode.None)
                return;

            HeTrace.WriteLine($"--- Check Validity of Games ({propertyName}) ---");

            foreach (C_Game game in ExtPlatformGames)
            {
                HeTrace.WriteLine($"Test for: {game.Title} [{ChosenMode}]");
                game.IsValide = CheckGame(game);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvGame"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        private bool CheckGame(C_Game mvGame)
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
        }

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



        /// <summary>
        /// Simulation
        /// </summary>
        internal void Simulation(/*GamePathMode mode*/)
        {
            HeTrace.WriteLine("Simulation"); // NewLine

            // On verifie que ToReplace n'est pas null (peut arriver)
            /*         if (ChosenMode == GamePathMode.KeepSubFolders && string.IsNullOrEmpty(ToReplace))
                     {
                         AddError("Is Null", nameof(ToReplace));
                         return;
                     }*/

            #region remplace alterpath
            // say mode
            HeTrace.WriteLine($"Mode: {ChosenMode}");

            // Refresh les chemins de la plateforme
            HeTrace.WriteLine("Refresh for platform Paths"); // NewLine
            this.InitializePlateformFolders();

            // Refresh des jeux pour avoir la dernière version
            HeTrace.WriteLine("Refresh for game Paths"); // NewLine
            this.InitializeGames();

            HeTrace.WriteLine("Simulation in progress...");
            // Modification des chemins
            foreach (C_Game game in ExtPlatformGames)
            {
                // Trace.WriteLine($@"{game.Title}:");
                HeTrace.WriteLine($"-- {game.Title}"); // NewLine

                #region 04/03/2021
                // On va profiter à chaque fois pour vérifier s'il y a validité
                bool gameValidity = true;
                #endregion

                // On examine tous les chemins, et en fonction du type on modifie les destinations                
                foreach (C_PathsDouble pathO in game.EnumGetPaths)
                {
                    string rootPath = "";

                    // Cas où pathO est null
                    if (pathO == null)
                    {
                        HeTrace.WriteLine($"\tPath O == null");
                        continue;
                    }

                    HeTrace.Write($"\t-- {pathO.Type}: ");

                    // On passe si le chemin d'origine est vide (balise vide)
                    if (string.IsNullOrEmpty(pathO.RelatPath))
                    {
                        HeTrace.EndLine("null");
                        continue;
                    }

                    HeTrace.EndLine(pathO.RelatPath);

                    // Assignation du root path pour reconstruire

                    // On va commencer par chercher le chemin auquel rataché les informations
                    switch (pathO.Type)
                    {
                        // On peut utiliser le keep ou le forcer grâce à AppPathsBuilder
                        case nameof(PathType.ApplicationPath):
                            rootPath = DicSystemPaths[MediaType.Application].RelatPath;
                            PathsBuilder(pathO, rootPath);
                            break;

                        #region Forced mode
                        case nameof(PathType.ManualPath):
                            rootPath = DicSystemPaths[MediaType.Manual].RelatPath;
                            PathsBuilder(pathO, rootPath);
                            break;

                        case nameof(PathType.MusicPath):
                            rootPath = DicSystemPaths[MediaType.Music].RelatPath;
                            PathsBuilder(pathO, rootPath);
                            break;

                        case nameof(PathType.VideoPath):
                            rootPath = DicSystemPaths[MediaType.Video].RelatPath;
                            KeepSubFolderMode(pathO, rootPath);
                            break;
                        #endregion Forced mode


                        #region

                        /*
                        case "CartBackImagePath":
                            rootPath = dicSystemPaths["Cart - Back"];
                            boxLog.Text.Insert(0, $"CartBackImagePath: {rootPath}" + Environment.NewLine);
                            break;

                        case "CartFrontImagePath":
                            rootPath = dicSystemPaths["Cart - Front"];
                            boxLog.Text.Insert(0, $"CartFrontImagePath: {rootPath}" + Environment.NewLine);
                            break;

                        case "Cart3DImagePath":
                            break;
                        case "BackgroundImagePath":
                            break;
                        case "Box3DImagePath":
                            break;
                        case "BackImagePath":
                            break;


                        case "MarqueeImagePath":
                        case "FrontImagePath":
                            rootPath = dicSystemPaths["Box - Front"];
                            break;

                        case "ScreenshotImagePath":
                            rootPath = dicSystemPaths["Screenshot - Gameplay"];
                            break;
                        case "ClearLogoImagePath":
                            rootPath = dicSystemPaths["Clear Logo"];
                            break;*/

                        #endregion
                        default:
                            HeTrace.WriteLine("\t!!! Not managed");
                            continue;

                    }

                    // Création des chemins

                    #region viré pour une méthode
                    /*
                    // modes
                    if (mode == GamePathMode.Forced)
                    // mode forcé on ne va récupérer que le nom du fichier
                    {
                        string tmp = Path.Combine(rootPath, Path.GetFileName(pathO.OldRelatPath));

                        // Recherche du lien réel + remise en forme en passant
                        pathO.NewHardPath = Path.GetFullPath(tmp, Global.LaunchBoxPath);
                        pathO.NewRelatPath = Path.GetRelativePath(Global.LaunchBoxPath, pathO.NewHardPath);
                    }
                    else if (mode == GamePathMode.KeepSubFolders)
                    // Mode conservant les sous-dossiers 
                    {
                        /*
                         * Ce mode doit éliminer l'ancien lien
                         * 
                         *//*

                        string mee = pathO.OldRelatPath.Replace($"{ToReplace}", "");
                        //int pos = pathO.OldRelatPath.IndexOf($@"\{PlatformName}\");

                        //string tmp = pathO.OldRelatPath.Substring(pos + PlatformName.Length + 2);

                        pathO.NewHardPath = Path.Combine(rootPath, mee.Substring(1));
                        pathO.NewRelatPath = Path.GetRelativePath(Global.LaunchBoxPath, pathO.NewHardPath);
                    }
                    */
                    #endregion


                    #endregion remplace Alterpath

                    #region 04/03/2021
                    gameValidity &= !pathO.ToModify;

                    #endregion
                }


                #region 2020


                if (AddAppPaths == true && game.AddiRomPaths.Count > 0)
                {
                    HeTrace.WriteLine($"2020: Additionnal App Path Managing  -----"); // NewLine

                    // Modifications sur les chemins des roms mixed
                    foreach (AAppPath addiApp in game.AddiRomPaths)
                    {
                        HeTrace.WriteLine($"\t-- AdditionnalApplication: {addiApp.RelatPath}");

                        // Conversions des / en \
                        addiApp.RelatPath = addiApp.RelatPath.Replace('/', '\\');
                        addiApp.HardPath = addiApp.HardPath.Replace('/', '\\');

                        // Assignation du root
                        PathsBuilder(addiApp, DicSystemPaths[MediaType.Application].RelatPath);


                        gameValidity &= !addiApp.ToModify; //addiApp.Test_Validity();

                    }
                } // End additional app
                #endregion --- 2020


                HeTrace.WriteLine("Check validity of games");
                #region 04/03/2021
                HeTrace.WriteLine($"Validity test {game.Title}:  {gameValidity}");
                game.IsValide = gameValidity;
                #endregion
                //   Trace.Unindent();
            }       // Fin du parcours des jeux;


            HeTrace.WriteLine("End of Simulation"); // NewLine
        }

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



        /// <summary>
        /// Créer les chemins en fonction du mode
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="rootPath">Chemin de root avec lequel travailler</param>
        /// <param name="mode">Mode des chemins (normal/forcé)</param>
        /// <remarks>
        /// Pour le moment uniquement utilisé pour les applications:
        ///     - Cas forcé.
        ///     - Cas du keep, il doit avoir le pivot inclus dans son chemin
        ///     - Cas du keep qui n'a pas le pivot inclus, on passe pour laisser l'utilisateur faire d'autres modifs
        ///     
        /// </remarks>
        private void PathsBuilder(C_PathsDouble pathO, string rootPath)
        {
            HeTrace.WriteLine($"\tUtilisation du rootpath {rootPath}", 10); // NewLine
            pathO.NewRelatPath = pathO.RelatPath;
            pathO.NewHardPath = pathO.HardPath;

            // modes
            if (ChosenMode == GamePathMode.Forced)
            {
                ForcedMode(pathO, rootPath);
            }
            // Mode conservant les sous-dossiers - Doit fonctionner normalement avec les fichiers à la racine aussi
            else if (ChosenMode == GamePathMode.KeepSubFolders)
            {
                KeepSubFolderMode(pathO, rootPath);
                RemoveSubString(pathO, rootPath);
            }

            if (!pathO.ToModify)
            {
                pathO.NewHardPath = SPRLang.No_Modif;
                pathO.NewRelatPath = SPRLang.No_Modif;
                HeTrace.WriteLine($"\tAppPathsBuilder no modification: {pathO.RelatPath} ");
            }

        }


        /// <summary>
        /// Mode forcé on ne va récupérer que le nom du fichier
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="categPath"></param>
        private void ForcedMode(C_PathsDouble pathO, string categPath)
        {

            string tmp = Path.Combine(categPath, Path.GetFileName(pathO.NewRelatPath));

            #region 26/04/2021
            if (pathO.NewRelatPath.Equals(tmp))
            {
                HeTrace.WriteLine($"\tPath use already ForcedMode: '{pathO.NewRelatPath}'");
                pathO.ToModify = false;
                return;

            }
            #endregion

            pathO.ToModify = true;

            // Recherche du lien réel + remise en forme en passant
            pathO.NewHardPath = Path.GetFullPath(tmp, Global.LaunchBoxRoot);
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);

            #region 24/12/2020
            // 05/03/2021 pathO.ToModify = true;
            #endregion

            HeTrace.WriteLine($"\tForcedMode: {pathO.NewRelatPath}");
        }

        /// <summary>
        /// Mode Keep, on conserve la structure
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="categPath"></param>
        private void KeepSubFolderMode(C_PathsDouble pathO, string categPath)
        {
            //string toReplace = ToReplace.Trim();
            HeTrace.WriteLine($"\tCheck {pathO.NewRelatPath}");


            // Si le chemin contient déjà l'ancien rootpath ou qu'il ne contient pas ce qu'on doit remplacer on passe
            bool cond1 = pathO.NewRelatPath.Contains(categPath);
            bool cond2 = pathO.NewRelatPath.Contains(PlatformToReplace);


            // Le chemin a déjà la bonne racine
            if (cond1)
            {
                HeTrace.WriteLine($"\tPath is Good");

                pathO.ToModify = false;

                return;
            }

            // Le chemin ne contient pas l'ancienne plateforme
            else if (!cond2)
            {
                HeTrace.WriteLine($"\tPath is bad, but doesn't contain '{PlatformName}'");

                pathO.ToModify = false;
                return;
            }

            if (cond2)
            {
                HeTrace.WriteLine($"\tPath is bad and contains '{PlatformToReplace}'");

                // On récupère la mauvaise partie
                string badPart = pathO.NewRelatPath.Substring(0, pathO.NewRelatPath.IndexOf(PlatformToReplace) + PlatformToReplace.Length);
                HeTrace.WriteLine($"\tFound bad part: {badPart}");

                string tmp = pathO.NewRelatPath.Replace(badPart, categPath);

                pathO.NewHardPath = Path.GetFullPath(tmp, Global.LaunchBoxRoot);

            }



            pathO.ToModify = true;
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);
            HeTrace.WriteLine($"\tKeepSubFolderMode: {pathO.NewHardPath}");
            HeTrace.WriteLine($"\tKeepSubFolderMode: {pathO.NewRelatPath}");
        }

        private void RemoveSubString(C_PathsDouble pathO, string categPath)
        {
            if (string.IsNullOrEmpty(SubStringToRemove))
                return;

            string tail = pathO.NewRelatPath.Replace(categPath, string.Empty);

            string substringToRemove = SubStringToRemove.Replace('/', '\\');

            if (!tail.Contains(substringToRemove, StringComparison.OrdinalIgnoreCase))
                return;

            // Le chemin nécessite une modification sur la tail
            HeTrace.WriteLine($"\tPath need to remove '{substringToRemove}'");

            string target = tail.Replace(substringToRemove, string.Empty, StringComparison.OrdinalIgnoreCase)
                                    .Replace(@"\\", @"\");

            pathO.NewHardPath = Path.GetFullPath(Path.Combine(categPath, target.TrimStart('\\')),
                                        Global.LaunchBoxPath);
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);
            pathO.ToModify = true;

        }




        /// <summary>
        /// Apply changes
        /// </summary>
        internal void ApplyChanges()
        {
            HeTrace.WriteLine(@"[ApplyChanges] Process start ..."); // NewLine

            foreach (var game in ExtPlatformGames)
            {
                //
                if (game.IsValide == true)
                {
                    HeTrace.WriteLine($"\tGame already valid -> pass: {game.Title}");
                    continue;
                }

                DematrioChka(game);
            }

            if (!Global.DebugMode)
            {
                PluginHelper.DataManager.Save();
                DxMBox.ShowDial(SPRLang.Paths_Modified);

                //MessageBox.Show(Lang.Save_Ok, Lang.Save_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Régénération
            InitializeGames();
            HeTrace.WriteLine(@"[ApplyChanges] Process finish."); // NewLine
        }

        /// <summary>
        /// Utilisé par Apply
        /// </summary>
        /// <param name="game"></param>
        private void DematrioChka(C_Game game)
        {
            HeTrace.WriteLine($"\t[DematrioChka] {game.Title}");
            // Trace.Indent();

            // Cherchez dans tous les IGames
            /*IGame originalGame = null;
            foreach (IGame igame in _IPlatformGames)
            {
                if (igame.Id.Equals(game.Id))
                {
                    originalGame = igame;
                    break;
                }
            }*/

            IGame originalGame = _IPlatformGames.FirstOrDefault(x => game.Id.Equals(x.Id));
            if (originalGame == null)
            {
                HeTrace.WriteLine("\t\tGame not found");
                return;
            }

            if (originalGame.ApplicationPath == null)
            {
                HeTrace.WriteLine("\t\tGame's application path is null !");
                return;
            }

            #region 04/03/2021
            /*//  A totalement modifier car ne gère pas le "tomodify"
            if (!game.ApplicationPath.ToModify)
            {
                HeTrace.WriteLine("\t\tGame not modified (because of simulation method)");
                return;
            }*/
            #endregion 04/03/2021

            // Jeux                
            //originalGame.ApplicationPath = game.ApplicationPath == null ? null : game.ApplicationPath.NewRelatPath;
            // Manual
            // originalGame.ManualPath = game.ManualPath == null ? null : game.ManualPath.NewRelatPath;
            // Music
            //originalGame.MusicPath = game.MusicPath == null ? null : game.MusicPath.NewRelatPath;
            // Video
            //originalGame.VideoPath = game.VideoPath == null ? null : game.VideoPath.NewRelatPath;
            // Theme Video
            // originalGame.ThemeVideoPath = game.ThemeVideoPath == null ? null : game.ThemeVideoPath.NewRelatPath;

            // Passer tous les dossiers cibles des mvGames dans IPGames
            foreach (C_PathsDouble collecOPaths in game.EnumGetPaths)
            {
                // Si la balise est vide
                if (collecOPaths == null)
                {
                    HeTrace.WriteLine($"Path object is null");
                    continue;
                }

                // S'il n'y a pas d'utilité de cette partie modifier
                if (!collecOPaths.ToModify)
                {
                    HeTrace.WriteLine($"\t\t{collecOPaths.Type} Path not modified (Same as original)");
                    continue;
                }


                switch (collecOPaths.Type)
                {
                    // On passe sur les principaux car ils sont déjà traités
                    case nameof(PathType.ApplicationPath):
                        originalGame.ApplicationPath = game.ApplicationPath.NewRelatPath;
                        break;

                    case nameof(PathType.ManualPath):
                        originalGame.ManualPath = game.ManualPath.NewRelatPath;
                        break;

                    case nameof(PathType.MusicPath):
                        originalGame.MusicPath = game.MusicPath.NewRelatPath;
                        break;

                    case nameof(PathType.VideoPath):
                        originalGame.VideoPath = game.VideoPath.NewRelatPath;
                        break;
                }

                HeTrace.WriteLine($"\t\tModifications for {collecOPaths.Type}", 10);

                //
                collecOPaths.RelatPath = collecOPaths.NewRelatPath;
                collecOPaths.HardPath = collecOPaths.NewHardPath;
                collecOPaths.Raz_NewPaths();
            }

            #region 2020 Prise en charge des mixed roms

            // Uniquement si la box est cochée
            if (AddAppPaths == true)
            {
                HeTrace.WriteLine("\t\tModifications for the Additionnal Apps");
                foreach (IAdditionalApplication oMixedRoms in originalGame.GetAllAdditionalApplications())
                {
                    // Récupération des informations des chemins pour la mixes rom
                    AAppPath appPaths = game.AddiRomPaths.FirstOrDefault(x => x.Id.Equals(oMixedRoms.Id));

                    if (appPaths == null)
                    {
                        HeTrace.WriteLine("\t\tAdditionnal application not found");
                        continue;
                    }

                    // S'il n'y a pas d'utilité de cette partie modifier
                    if (!appPaths.ToModify)
                    {
                        HeTrace.WriteLine($"\t\t{appPaths.RelatPath} Path not modified (Same as Original)");
                        continue;
                    }


                    //MessageBox.Show($"{paths.ID} {paths.Destination_HLink} - {paths.Destination_RLink}");

                    //
                    oMixedRoms.ApplicationPath = appPaths.NewRelatPath;
                    //
                    appPaths.RelatPath = appPaths.NewRelatPath;
                    appPaths.HardPath = appPaths.NewHardPath;
                    appPaths.Raz_NewPaths();
                }
            }
            #endregion

        }


        public void Dispose()
        {
            HeTrace.RemoveMessenger("Verbose");
        }


        #region errors

        /// <summary>
        /// All Errors
        /// </summary>
        private readonly Dictionary<string, List<string>> _erreurs = new Dictionary<string, List<string>>();


        public bool HasErrors => _erreurs.Any();// throw new NotImplementedException();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            Debug.WriteLine($"[GetErrors] {propertyName}");

            if (_erreurs.ContainsKey(propertyName))
            {
                string tmp = null;

                // Récupération des erreurs de la propriété concernée
                foreach (string errMess in _erreurs[propertyName])
                    if (tmp == null)
                        tmp = errMess;
                    else
                        tmp += Environment.NewLine + errMess;

                yield return tmp;

            }
        }


        /// <summary>
        /// Ajoute une erreur au tableau des erreurs
        /// </summary>
        /// <param name="propertyName">Name of property</param>
        /// <param name="erreur">error description</param>
        private void AddError(string erreur, [CallerMemberName] string propertyName = null)
        {
            // On ajoute au cas où ça serait manquant
            if (!_erreurs.ContainsKey(propertyName))
            {
                _erreurs[propertyName] = new List<string>();
            }

            _erreurs[propertyName].Add($"{propertyName}: {erreur}");
        }

        /// <summary>
        /// Enlève une erreur en fonction du nom de la propriété
        /// </summary>
        /// <param name="propertyName"></param>

        private void RemoveError([CallerMemberName] string propertyName = null)
        {
            if (_erreurs.ContainsKey(propertyName))
                // On enlève de la file
                _erreurs.Remove(propertyName);
        }



        #endregion
    }
}

