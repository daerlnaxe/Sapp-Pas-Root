using DxPaths.Windows;
using DxTBoxCore.MBox;
using Hermes;
using Hermes.Messengers;
using SPR.Containers;
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
        /*
        private string _Log;
        /// <summary>
        /// 
        /// </summary>
        public string Log
        {
            get { return _Log; }
            set
            {
                _Log = value;
                OnPropertyChanged();
            }
        }*/

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
                /*  if (value == _ChosenMode)
                      return;*/

                RemoveError();

                switch (value)
                {
                    case GamePathMode.None:
                        AddError(SPRLang.Err_SelectMode);
                        break;

                    case GamePathMode.KeepSubFolders:
                        FindPivot();
                        break;

                }

                _ChosenMode = value;

                OnPropertyChanged();
            }
        }


        //bool ToReplaceGiven = false;
        private string _ToReplace;
        /// <summary>
        /// Permet d'affiner les modifications
        /// </summary>
        public string ToReplace
        {
            get { return _ToReplace; }
            set
            {
                _ToReplace = value;

                RemoveError();

                //if(ChosenMode)

                if (!value.StartsWith(".\\") && !value.StartsWith("..\\"))
                    AddError(SPRLang.Err_MustRelat);

                /*if (string.IsNullOrEmpty(value))
                    AddError("is null");*/

                if (value.EndsWith('\\'))
                    AddError(SPRLang.Err_FinishSlash);

                OnPropertyChanged();

            }
        }

        #endregion

        #region Sans notification

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
        public Dictionary<MediaType, C_Paths> DicSystemPaths { get; private set; } = new Dictionary<MediaType, C_Paths>();


        public GamesModel()
        {


        }


        #endregion

        /// <summary>
        /// Initialization with a platform (edition)
        /// </summary>
        /// <param name="selectedPlatform"></param>
        internal void InitializeEdition(IPlatform selectedPlatform)
        {
            if (selectedPlatform == null)
                throw new NullReferenceException("Object Plateform is null !");

            SelectedPlatform = selectedPlatform;
            PlatformName = selectedPlatform.Name;

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

            // --- Récupération des jeux
            InitializeGames();
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


            /*DicSystemPaths.Add("Application", new C_Paths(ObjectPlatform.Folder, nameof(PathType.ApplicationPath)));
            //
            foreach (var plat in platformFolders)
                DicSystemPaths.Add(plat.MediaType,new C_Paths(plat.FolderPath, nameof());*/


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

                // --- Check de la validité du jeu
                tmpGame.IsValide = CheckGame(tmpGame);

                ExtPlatformGames.Add(tmpGame);

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
        internal void FindPivot()
        {
            // Mise en place du système pivot/tail, on donne un mot, on conservera ce qu'il y a après
            //string[] arr = _IPlatformGames[0].ApplicationPath.Split(@"\");

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



                // Travail sur la chaine à remplacer
                // Ne convient pas, trop de problèmes.
                /*
                string[] arr = _IPlatformGames[i].ApplicationPath.Split(@"\");
                ToReplace = String.Join(@"\", arr, 0, arr.Length - 2);
                break;*/

                string[] arr = game.ApplicationPath.RelatPath.Split(@"\");

                // on peut éventuellement trouver via le nom du systeme.
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

                // On peut eventuellement trouver via le nom du dernier dossier utilisé dans le path actuel


                /*
                // En dernier choix on prend l'avant dernier dossier
                string[] arr = _IPlatformGames[i].ApplicationPath.Split(@"\");
                if (arr.Length >= 3)
                {
                    ToReplace = arr[arr.Length - 3];

                    break;
                }
                */

                // Dernier cas on prend le premier application path qui ne correspond pas
                ToReplace = Path.GetDirectoryName(game.ApplicationPath.RelatPath);
                HeTrace.WriteLine("[Pivot] Last choice - Edit is strongly recommended");
                return;
            }

            ToReplace = SelectedPlatform.Folder;
            HeTrace.WriteLine($"[Pivot] No predict");
        }


        /// <summary>
        /// Vérifie l'intégrité des jeux (pas de renouvellement des jeux)
        /// </summary>
        public void CheckAllGames()
        {
            HeTrace.WriteLine("--- Check Validity of Games ---");

            foreach (C_Game game in ExtPlatformGames)
                game.IsValide = CheckGame(game);

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
                        isValide = Test_Path(DicSystemPaths[MediaType.Application].RelatPath, pathO.RelatPath);
                        break;

                    // Les autres font une opération de bit
                    case nameof(PathType.ManualPath):
                        //isValide &= pathO.OldRelatPath.Contains(DicSystemPaths["Manual"]);
                        isValide &= Test_Path(DicSystemPaths[MediaType.Manual].RelatPath, pathO.RelatPath);
                        //HeTrace.EndLine($"{isValide} (Manual): {DicSystemPaths["Manual"]}", 10);
                        break;

                    case nameof(PathType.MusicPath):
                        //isValide &= pathO.OldRelatPath.Contains(DicSystemPaths["Music"]);
                        isValide &= Test_Path(DicSystemPaths[MediaType.Music].RelatPath, pathO.RelatPath);
                        //HeTrace.EndLine($"{isValide} (Music): {DicSystemPaths["Music"]}", 10);
                        break;

                    case nameof(PathType.VideoPath):
                        isValide &= Test_Path(DicSystemPaths[MediaType.Video].RelatPath, pathO.RelatPath);
                        break;
                }


                //                valide &= pathO.Original_RLink.Contains(tboxROldPath.Text);
            }

            // Check des chemins additionnels, si les chemins additionnels sont pris en compte
            if (AddAppPaths)
                foreach (AAppPath pathO in mvGame.AddiRomPaths)
                {
                    isValide &= Test_Path(DicSystemPaths[MediaType.Application].RelatPath, pathO.RelatPath);
                }

            HeTrace.WriteLine($"\tValidity of the game: {isValide}");
            return isValide;
        }

        /// <summary>
        /// Teste la validité d'un chemin en fonction d'un référent
        /// </summary>
        /// <param name="referent"></param>
        /// <param name="toTest"></param>
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

            if (toTest.Contains(referent))
            {
                HeTrace.WriteLine($"\t\tResult: true", 10);
                return true;
            }

            HeTrace.WriteLine($"\t\tResult: false - path doesn't contain referent", 10);
            return false;
        }


        /// <summary>
        /// Initialize with parameters needed entries
        /// </summary>
        [Obsolete]
        private void NormalInit()
        {
            HeTrace.WriteLine("Plugin Mode Activé"); // NewLine

            // Récupération des jeux ;avec tri        
            _IPlatformGames = SelectedPlatform.GetAllGames(IncludeHidden, true)//(false, false)
                                                 ;//         .OrderBy(x => x.Title).ToArray();
        }

        /// <summary>
        /// Simulation
        /// </summary>
        internal void Simulation(/*GamePathMode mode*/)
        {
            HeTrace.WriteLine("Simulation"); // NewLine

            // On verifie que ToReplace n'est pas null (peut arriver)
            if (ChosenMode == GamePathMode.KeepSubFolders && string.IsNullOrEmpty(ToReplace))
            {
                AddError("Is Null", nameof(ToReplace));
                return;
            }



            #region remplace alterpath
            // say mode
            HeTrace.WriteLine($"Mode: {ChosenMode}");

            // Refresh les chemins de la plateforme
            HeTrace.WriteLine("Refresh for platform Paths"); // NewLine
            this.InitializePlateformFolders();


            // Refresh des jeux pour avoir la dernière version
            HeTrace.WriteLine("Refresh for game Paths"); // NewLine
            this.InitializeGames();

            // Modification des chemins
            foreach (C_Game game in ExtPlatformGames)
            {
                // Trace.WriteLine($@"{game.Title}:");
                HeTrace.WriteLine($"-- {game.Title}"); // NewLine

                // Vérification que le jeu n'est pas déjà valide.
                #region 24/12/2020
                if (game.IsValide)
                {
                    HeTrace.WriteLine($"\tGame already valid -> Simulation avoided: {game.Title}");
                    continue;
                }
                #endregion

                //   Trace.Indent();

                // On examine tous les chemins, et en fonction du type on modifie les destinations                
                foreach (C_PathsDouble pathO in game.EnumGetPaths)
                {
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
                    //HeTrace.WriteLine($"\tModification for {pathO.Type}: {pathO.OldRelatPath}"); // NewLine


                    #region 24/12/2020
                    #endregion



                    // Assignation du root path pour reconstruire
                    string rootPath = "";

                    // On va commencer par chercher le chemin auquel rataché les informations


                    switch (pathO.Type)
                    {
                        // On peut utiliser le keep
                        case nameof(PathType.ApplicationPath):
                            rootPath = DicSystemPaths[MediaType.Application].RelatPath;
                            AppPathsBuilder(pathO, rootPath, ChosenMode);
                            break;

                        #region Forced mode
                        case nameof(PathType.ManualPath):
                            rootPath = DicSystemPaths[MediaType.Manual].RelatPath;
                            ForcedMode(pathO, rootPath);
                            break;

                        case nameof(PathType.MusicPath):
                            rootPath = DicSystemPaths[MediaType.Music].RelatPath;
                            ForcedMode(pathO, rootPath);
                            break;

                        case nameof(PathType.VideoPath):
                            rootPath = DicSystemPaths[MediaType.Video].RelatPath;
                            ForcedMode(pathO, rootPath);
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
                            HeTrace.WriteLine("\t!!! Untreated");
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
                }

                #region 2020

                // Si la fonction pour modifier aussi les romx mixed n'est pas activée on passe
                if (AddAppPaths != true || game.AddiRomPaths.Count == 0)
                    continue;

                HeTrace.WriteLine($"2020: Additionnal App Path Managing  -----"); // NewLine
                // Modifications sur les chemins des roms mixed
                foreach (AAppPath addiApp in game.AddiRomPaths)
                {
                    HeTrace.WriteLine($"\t-- AdditionnalApplication: {addiApp.RelatPath}");

                    // Conversions des / en \
                    //addiApp.Paths.OldRelatPath = addiApp.Paths.OldRelatPath.Replace('/', '\\');
                    addiApp.RelatPath = addiApp.RelatPath.Replace('/', '\\');
                    //addiApp.Paths.OldHardPath = addiApp.Paths.OldHardPath.Replace('/', '\\');
                    addiApp.HardPath = addiApp.HardPath.Replace('/', '\\');

                    // Assignation du root
                    //PathsBuilder(addiApp.Paths, DicSystemPaths["Application"], mode);
                    AppPathsBuilder(addiApp, DicSystemPaths[MediaType.Application].RelatPath, ChosenMode);
                    //rootPath = Path.Combine(dicSystemPaths["Application"], fichier);

                    /*
                    // modes
                    string fichier = "";    // Récupération du nom du fichier ?
                    if (rbForced.Checked)       // mode forcé
                    {
                        int pos = addiAppPath.Original_RLink.LastIndexOf('\\');
                        fichier = addiAppPath.Original_RLink.Substring(pos + 1); //+1 pour lever le \
                    }
                    else if (rbKeepSub.Checked) // Mode conservant les sous-dossiers 
                    {
                        int pos = addiAppPath.Original_RLink.IndexOf($@"\{PlatformName}\");
                        fichier = addiAppPath.Original_RLink.Substring(pos + PlatformName.Length + 2);
                    }*/

                    //HeTrace.WriteLine($"$2020 : {fichier}";
                    //  addiApp.Destination_RLink = fileDest;
                    //addiApp.Destination_HLink = Path.GetFullPath(Path.Combine(AppPath, fileDest));

                    //HeTrace.WriteLine($"2020 Additionnal App: {addiApp.Paths.OldRelatPath} => {addiApp.Paths.NewRelatPath}"); // NewLine

                }
                #endregion --- 2020

                //   Trace.Unindent();
            }

            HeTrace.WriteLine("End of Simulation"); // NewLine
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
        private void AppPathsBuilder(C_PathsDouble pathO, string rootPath, GamePathMode mode)
        {
            HeTrace.WriteLine($"\tUtilisation du rootpath {rootPath}", 10); // NewLine

            // modes
            if (mode == GamePathMode.Forced)
            {
                ForcedMode(pathO, rootPath);
            }
            else if (mode == GamePathMode.KeepSubFolders)
            // Mode conservant les sous-dossiers - Doit fonctionner normalement avec les fichiers à la racine aussi
            {
                #region 24/12/2020
                // On vérifie que la chaine contient le dossier enfant
                if (pathO.RelatPath.Contains(ToReplace))
                {
                    KeepSubFolderMode(pathO, rootPath);
                }
                /*
                else
                {
                    ForcedMode(pathO, ToReplace);
                }*/

                #endregion
            }


            if (!pathO.ToModify)
                HeTrace.WriteLine($"\tAppPathsBuilder no Simulation: {pathO.RelatPath} ");

        }



        /// <summary>
        /// Mode forcé on ne va récupérer que le nom du fichier
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="rootPath"></param>
        private void ForcedMode(C_PathsDouble pathO, string rootPath)
        {
            string tmp = Path.Combine(rootPath, Path.GetFileName(pathO.RelatPath));

            // Recherche du lien réel + remise en forme en passant
            pathO.NewHardPath = Path.GetFullPath(tmp, Global.LaunchBoxRoot);
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);

            #region 24/12/2020
            pathO.ToModify = true;
            #endregion

            HeTrace.WriteLine($"\tForcedMode: {pathO.NewRelatPath}");
        }

        /// <summary>
        /// Mode Keep, on conserve la structure
        /// </summary>
        /// <param name="pathO"></param>
        /// <param name="rootPath"></param>
        private void KeepSubFolderMode(C_PathsDouble pathO, string rootPath)
        {
            string mee = pathO.RelatPath.Replace($"{ToReplace}", "");
            //int pos = pathO.OldRelatPath.IndexOf($@"\{PlatformName}\");

            //string tmp = pathO.OldRelatPath.Substring(pos + PlatformName.Length + 2);

            pathO.NewHardPath = Path.Combine(rootPath, mee.Substring(1));
            pathO.NewRelatPath = DxPath.To_Relative(Global.LaunchBoxRoot, pathO.NewHardPath);

            #region 24/12/2020
            pathO.ToModify = true;
            #endregion 

            HeTrace.WriteLine($"\tKeepSubFolderMode: {pathO.NewRelatPath}");
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
                if (game.IsValide)
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

            //  A totalement modifier car ne gère pas le "tomodify"
            if (!game.ApplicationPath.ToModify)
            {
                HeTrace.WriteLine("\t\tGame not modified (because of simulation method)");
                return;
            }

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
                foreach (IAdditionalApplication oMixedRoms in originalGame.GetAllAdditionalApplications())
                {
                    // Récupération des informations des chemins pour la mixes rom
                    AAppPath appPaths = game.AddiRomPaths.FirstOrDefault(x => x.Id.Equals(oMixedRoms.Id));

                    if (appPaths == null)
                    {
                        HeTrace.WriteLine("\t\tAdditionnal application not found");
                        return;
                    }


                    //MessageBox.Show($"{paths.ID} {paths.Destination_HLink} - {paths.Destination_RLink}");

                    //
                    // oMixedRoms.ApplicationPath = paths.Destination_RLink;
                    //
                    appPaths.RelatPath = appPaths.NewRelatPath;
                    appPaths.HardPath = appPaths.NewHardPath;
                    appPaths.Raz_NewPaths();
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

