using SPR.Containers;
using SPR.Languages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unbroken.LaunchBox.Plugins.Data;
using SPR.Enums;
using DxTBoxCore.Box_Progress;
#if DEBUG
using System.Diagnostics;
#endif

namespace SPR.Models
{
    class MigrateModel : INotifyPropertyChanged, IDisposable, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Avec notifications
        private string _Source;
        public string Source
        {
            get { return _Source; }
            set
            {
                _Source = value;
                Simulate_SrcPaths();
                //  OnPropertyChanged();
            }
        }

        /// <summary>
        /// dd
        /// </summary>
        private string _Destination;
        /// <summary>
        /// Dossier de destination
        /// </summary>
        public string Destination
        {
            get { return _Destination; }
            set
            {
                _Destination = value;

                Simulate_DestPaths();

                //    OnPropertyChanged();
            }
        }

        /// <summary>
        /// Dossiers principaux de la source
        /// </summary>
        public MainDatas SrcMPaths { get; } = new MainDatas();

        /// <summary>
        /// Dossiers principaux de la destination
        /// </summary>
        public MainDatas DestMPaths { get; } = new MainDatas();

        #endregion Avec Notifications

        #region Sans Notif
        SubFolderModel _srcSub;
        /// <summary>
        /// Modele contenant les infos des subfolders de la source
        /// </summary>
        public SubFolderModel SrcSub
        {
            get { return _srcSub; }
            set
            {
                if (value == null)
                    return;

                _srcSub = value;
                _srcSub.ValueChanged += SrcFoldersChanged;
            }
        }

        /// <summary>
        /// Modele contenant les infos des subfolders de la destination
        /// </summary>
        SubFolderModel _destSub;
        public SubFolderModel DestSub
        {
            get { return _destSub; }
            set
            {
                if (value == null)
                    return;

                _destSub = value;
                _destSub.ValueChanged += DestFoldersChanged;
            }
        }
        #endregion

        /// <summary>
        /// Représente les chemins principaux
        /// </summary>
        /*public HashSet<ModelSD> MainPaths 
        {
            get;
            set;
        } = new HashSet<ModelSD>();*/

        public ModelSD GamesPaths { get; set; } = new ModelSD(SPRLang.Games);

        public ModelSD ManualsPaths { get; set; } = new ModelSD(SPRLang.Manuals);

        public ModelSD ImagesPaths { get; set; } = new ModelSD(SPRLang.Images);

        public ModelSD MusicsPaths { get; set; } = new ModelSD(SPRLang.Musics);

        public ModelSD VideosPaths { get; set; } = new ModelSD(SPRLang.Videos);

        internal void Initialize()
        {
            /*
            if (selectedPlatform == null)
                throw new Exception("Selected Platform is null");*/

            //   ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SrcMPaths.Musics"));
            /*
            MainPaths.Add(new ModelSD("Games"));
            MainPaths.Add(new ModelSD("Manuals"));
            MainPaths.Add(new ModelSD("Ima"));
            MainPaths.Add(new ModelSD("Videos"));*/

        }

        #region SubFolders

        // Todo vérifier la longueur du destination path et idem pour le source doit faire au moins 3
        // Todo vérifier les fichiers sources
        internal void SrcFoldersChanged(string value, string property)
        {

            HandleFoldersChange(value, property, SrcMPaths);

            Simulate_SrcPaths();
        }

        private void DestFoldersChanged(string value, string property)
        {
            HandleFoldersChange(value, property, DestMPaths);
            Simulate_DestPaths();
        }

        private void HandleFoldersChange(string value, string property, MainDatas model)
        {
            switch (property)
            {
                case "GamesFName":
                    model.Games = value;
                    break;

                case "ManualsFName":
                    model.Manuals = value;
                    break;

                case "ImagesFName":
                    model.Images = value;
                    break;

                case "MusicsFNames":
                    model.Musics = value;
                    break;

                case "VideosFName":
                    model.Videos = value;
                    break;
            }
        }

        #endregion SubFolders


        #region Simulation
        private void Simulate_SrcPaths()
        {
            if (string.IsNullOrEmpty(Source))
                return;

            string[] arrSrcPath = Source.Split("\\");
            string rootSrc = String.Join('\\', arrSrcPath, 0, arrSrcPath.Length - 2);
            string sysName = arrSrcPath.Last();

            // SrcMPaths.Games = Path.Combine(rootSrc, SrcSub.GamesFName, sysName);
            GamesPaths.Source = Path.Combine(rootSrc, SrcSub.GamesFName, sysName);
            ManualsPaths.Source = Path.Combine(rootSrc, SrcSub.ManualsFName, sysName);
            ImagesPaths.Source = Path.Combine(rootSrc, SrcSub.ImagesFName, sysName);
            MusicsPaths.Source = Path.Combine(rootSrc, SrcSub.MusicsFName, sysName);
            VideosPaths.Source = Path.Combine(rootSrc, SrcSub.VideosFName, sysName);

            /*            foreach (var elem in MainPaths)
                            switch (elem.PathType)
                            {
                                case PathType.ImagePath:
                                    elem.Source = Path.Combine(rootSrc, SrcSub.MusicsFName, sysName);
                                    break;
                            }*/

        }


        private void Simulate_DestPaths()
        {
            string[] arrDestPath = Destination.Split("\\");
            string rootDest = String.Join('\\', arrDestPath, 0, arrDestPath.Length - 2);
            string sysName = arrDestPath.Last();

            GamesPaths.Destination = Path.Combine(rootDest, DestSub.GamesFName, sysName);
            ManualsPaths.Destination = Path.Combine(rootDest, DestSub.ManualsFName, sysName);
            //DestMPaths.Images = Path.Combine(rootDest, DestSub.ImagesFName, sysName);
            ImagesPaths.Destination = Path.Combine(rootDest, DestSub.ImagesFName, sysName);
            MusicsPaths.Destination = Path.Combine(rootDest, DestSub.MusicsFName, sysName);
            VideosPaths.Destination = Path.Combine(rootDest, DestSub.VideosFName, sysName);

            /* foreach (var elem in MainPaths)
                 switch (elem.PathType)
                 {
                     case PathType.ImagePath:
                         elem.Source = Path.Combine(rootDest, DestSub.MusicsFName, sysName);
                         break;
                 }*/
        }

        #endregion Simulation


        #region Verification
        /// <summary>
        /// Vérification avant la copie
        /// </summary>
        internal bool Verifications()
        {
            // Vérifie que les chemins de la source existent
            /* foreach (KeyValuePair<string, string> chemin in SrcMPaths.GetPairs())
             {
                 if (!Directory.Exists(chemin.Value))
                 {
                     SrcMPaths.AddError(SPRLang.DirDontExist, chemin.Key);


                     return false;
                 }
             }*/

            // Vérification de l'existence des dossiers source
            bool v = true;

            v &= Verif_Source(GamesPaths);
            v &= Verif_Source(ManualsPaths);
            v &= Verif_Source(ImagesPaths);
            v &= Verif_Source(MusicsPaths);
            v &= Verif_Source(VideosPaths);

            // Faut il vérifier

            return v;
        }

        internal bool Verif_Source(ModelSD modelPaths)
        {
            if (Directory.Exists(modelPaths.Source))
                return true;

            modelPaths.Error_Source = SPRLang.DirDontExist;
            return false;
        }
        #endregion Verifications

        #region Application
        internal bool Apply()
        {
            List<string> files = new List<string>();

            // Liste des fichiers
            string[] fsGames = Directory.GetFiles(GamesPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsManuals = Directory.GetFiles(ManualsPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsImages = Directory.GetFiles(ImagesPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsMusics = Directory.GetFiles(MusicsPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsVideos = Directory.GetFiles(VideosPaths.Source, "*.*", SearchOption.AllDirectories);

            // Nombre total de fichiers
            int TotalFiles = fsGames.Length + fsManuals.Length + fsImages.Length + fsMusics.Length + fsVideos.Length;

            DxAsCollecProgress dxApply = new DxAsCollecProgress(SPRLang.Files_Migration)
            {
                
            };
            return true;

            /*
            try
            {
                // -- Games 
                Categ_Apply(GamesPaths);

                return true;
            }
            catch (Exception exc)
            {

                return false;
            }*/
        }

        private void Task_Apply()
        {

        }

        private void Categ_Apply(ModelSD dPaths)
        {
            /* 
            * On décide de conserver par défaut toutes les structures filles des dossiers
            * Quand il y a collision on fait un test de somme:
                - Si le fichier est <= à 100 Mo
                    + Si c'est similaire on passe, si ça ne l'est pas on demande
                - Si le fichier est supérieur à 100Mo on demande si test de somme/écraser/passer
            */
            if (dPaths.Source == null )
                return;
                        


            /*
            string dest;
            foreach (string f in files)
            {
                dest = f.Replace(dPaths.Source, dPaths.Destination);

                // Test si le fichier existe
                bool replace;
                if (File.Exists(dest))
                {
                    Check_File(f, dest);
                }
            }
            */
        }

        private void Check_File(string source, string destination)
        {
            // --- 
            


        }
        #endregion

        #region Application
        internal void Application()
        {
            // Todo voir pour laisser un choix 
        }

        #endregion

        public void Dispose()
        {
            _srcSub.ValueChanged -= SrcFoldersChanged;
            _destSub.ValueChanged -= DestFoldersChanged;
        }


        #region errors (copie)

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        /// <summary>
        /// All Errors
        /// </summary>
        private readonly Dictionary<string, List<string>> _erreurs = new Dictionary<string, List<string>>();

        public bool HasErrors => _erreurs.Any() || SrcSub.HasErrors || DestSub.HasErrors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {

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
