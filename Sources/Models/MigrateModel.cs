using AsyncProgress;
using AsyncProgress.Tools;
using DxLocalTransf;
using DxLocalTransf.Cont;
using DxLocalTransf.Copy;
using DxTBoxCore.Box_Decisions;
using DxTBoxCore.Box_MBox;
using DxTBoxCore.Box_Progress;
using DxTBoxCore.Common;
using HashCalc;
using Hermes;
using SPR.Containers;
using SPR.Languages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
#if DEBUG
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
        private string _Platform;
        public string Platform
        {
            get => _Platform;
            set
            {
                _Platform = value;
                OnPropertyChanged();
                Simulate_DestPaths();
            }
        }



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
        /// <summary>
        /// Blocks copying and summing of destination files
        /// </summary>
       // private bool _NotSimul;


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



        public ModelSD GamesPaths { get; set; } = new ModelSD(SPRLang.Games);

        public ModelSD ManualsPaths { get; set; } = new ModelSD(SPRLang.Manuals);

        public ModelSD ImagesPaths { get; set; } = new ModelSD(SPRLang.Images);

        public ModelSD MusicsPaths { get; set; } = new ModelSD(SPRLang.Musics);

        public ModelSD VideosPaths { get; set; } = new ModelSD(SPRLang.Videos);


        private Page IHM;

        public MigrateModel(Page w)
        {
            IHM = w;
        }


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

            Platform = sysName;

            // SrcMPaths.Games = Path.Combine(rootSrc, SrcSub.GamesFName, sysName);
            GamesPaths.Source = Path.Combine(rootSrc, SrcSub.GamesFName, sysName);
            ManualsPaths.Source = Path.Combine(rootSrc, SrcSub.ManualsFName, sysName);
            ImagesPaths.Source = Path.Combine(rootSrc, SrcSub.ImagesFName, sysName);
            MusicsPaths.Source = Path.Combine(rootSrc, SrcSub.MusicsFName, sysName);
            VideosPaths.Source = Path.Combine(rootSrc, SrcSub.VideosFName, sysName);

        }


        private void Simulate_DestPaths()
        {
            if (string.IsNullOrEmpty(Destination))
                return;

            //string[] arrDestPath = Destination.Split("\\");
            // string rootDest = String.Join('\\', arrDestPath, 0, arrDestPath.Length - 2);
            string rootDest = Destination;
            string platform = Platform.Trim();
            //string sysName = arrDestPath.Last();

            GamesPaths.Destination = Path.Combine(rootDest, DestSub.GamesFName, platform);
            ManualsPaths.Destination = Path.Combine(rootDest, DestSub.ManualsFName, platform);
            //DestMPaths.Images = Path.Combine(rootDest, DestSub.ImagesFName, sysName);
            ImagesPaths.Destination = Path.Combine(rootDest, DestSub.ImagesFName, platform);
            MusicsPaths.Destination = Path.Combine(rootDest, DestSub.MusicsFName, platform);
            VideosPaths.Destination = Path.Combine(rootDest, DestSub.VideosFName, platform);
        }


        #endregion Simulation


        #region Verification
        /// <summary>
        /// Vérification avant la copie
        /// </summary>
        internal bool Verifications()
        {
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

        /// <summary>
        /// Vérification que les dossiers existent
        /// </summary>
        /// <param name="modelPaths"></param>
        /// <returns></returns>
        internal bool Verif_Source(ModelSD modelPaths)
        {
            if (Directory.Exists(modelPaths.Source))
                return true;

            modelPaths.Error_Source = SPRLang.DirDontExist;
            return false;
        }
        #endregion Verifications

        #region Application
        internal bool? Apply()
        {
            HeTrace.WriteLine($"\nMigrating files");
            /*
            List<ModelSD> files = new List<ModelSD>();
            
            files.Add(GamesPaths);
            files.Add(ManualsPaths);
            files.Add(ImagesPaths);
            files.Add(MusicsPaths);
            files.Add(VideosPaths);*/


            HeTrace.WriteLine("Apply - Copy begin");
            List<DataTrans> files = new List<DataTrans>();
            files.AddRange(PrepareFiles(GamesPaths));
            files.AddRange(PrepareFiles(ImagesPaths));
            files.AddRange(PrepareFiles(ManualsPaths));
            files.AddRange(PrepareFiles(MusicsPaths));
            files.AddRange(PrepareFiles(VideosPaths));

            
            // Liste des fichiers
           /* string[] fsGames = Directory.GetFiles(GamesPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsManuals = Directory.GetFiles(ManualsPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsImages = Directory.GetFiles(ImagesPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsMusics = Directory.GetFiles(MusicsPaths.Source, "*.*", SearchOption.AllDirectories);
            string[] fsVideos = Directory.GetFiles(VideosPaths.Source, "*.*", SearchOption.AllDirectories);

            // Nombre total de fichiers
            int TotalFiles = fsGames.Length + fsManuals.Length + fsImages.Length + fsMusics.Length + fsVideos.Length;
           */
            
            HeTrace.WriteLine("Apply - Copy begin");
            HashCopy hashCopy = new HashCopy();
            hashCopy.UpdateStatus += (x,y ) => HeTrace.WriteLine(y.Message);
            PersistProgressD mee = new PersistProgressD(hashCopy);
            
            TaskLauncher launcher = new TaskLauncher()
            {
                ProgressIHM = new DxDoubleProgress(mee),
                AutoCloseWindow = false,
                MethodToRun = () => hashCopy.VerifSevNCopy(files),


            };
            launcher.Launch(hashCopy);
            /*

            MawEvo<PersisProgressD> mawmaw = new MawEvo<PersisProgressD>();

            TaskLauncher launcher = new TaskLauncher()
            {
                AutoCloseWindow = false,
                ProgressIHM = new DxDoubleProgress(mawmaw.Parler),
                MethodToRun = () => Categ_Apply(mawmaw, files),
            };



            return launcher.Launch(mawmaw);*/

            //            dxApply.TaskToRun.UpdateStatus += (x) => HeTrace.WriteLine(x);


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

        private IEnumerable<DataTrans> PrepareFiles(ModelSD modelSD)
        {
            List<DataTrans> files = new List<DataTrans>();
            foreach (string f in Directory.EnumerateFiles(modelSD.Source, "*.*", SearchOption.AllDirectories))
            {
                DataTrans dt = new DataTrans()
                {
                    Name = Path.GetFileName(f),
                    CurrentPath = f,
                    DestPath = f.Replace(modelSD.Source, modelSD.Destination)
                };
                files.Add(dt);

                HeTrace.WriteLine($"Prepare { dt.CurrentPath}\r\nto {dt.DestPath}");
            }
            return files;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tP"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        private object Categ_Apply(I_ASBase tP, List<ModelSD> models)
        {
         //   MawEvo<PersistProgressD> encaps = (MawEvo<PersistProgressD>)tP;
            /*
                - Je veux dissocier les tâches une List String ne me le permet pas.
                - Ce que je veux faire apparaitre c'est le nombre de fichier pour chaque catégorie
                - Il faudrait considérer la copie comme un tiers du travail, et la comparaison comme deux tiers.    

            */

           /* Dictionary<ModelSD, string[]> tasksOnFiles = new Dictionary<ModelSD, string[]>();
            int TotalFiles = 0;

            // Récupération des fichiers
            encaps.Parler.SetStatus(this, new StateArg("Initialize the list of files"));
            foreach (ModelSD mSD in models)
            {
                string[] tmp = Directory.GetFiles(mSD.CurrentPath, "*.*", SearchOption.AllDirectories);
                tasksOnFiles.Add(mSD, tmp);
                TotalFiles += tmp.Count();
            }

            */



            // Nombre total de fichiers
            //  int TotalFiles = fsGames.Length + fsManuals.Length + fsImages.Length + fsMusics.Length + fsVideos.Length;

            // Signaler le maximum pour la barre du Total

            // Préparation du système de vérification
            /* OpDFilesExt opfSys = new OpDFilesExt();
             opfSys.NotSimul = true;
             opfSys.AskToUser += this.AskWhatToDo;
             opfSys.SumError += this.InformErrorSum;
             opfSys.UpdateProgress += encaps.Parler.SetProgress;
             opfSys.IWriteLine += (x, y) => encaps.Parler.SetStatus(x, new StateArg(y, false));*/

            /*int i = 0; // Là selon on pourrait changer un peu la progression
            int max = models.Count;
            */
            // On va lire tous les fichiers donc de 0 au nombre total
            //for (int i = 0; i < tasksOnFiles.Count; i++)
            /* foreach (var tof in tasksOnFiles)
             {
                 //ModelSD Element = tasksOnFiles[i];
                 ModelSD Element = tof.Key;
                 string[] files = tof.Value;


                 // Signal position 0 & statuts pour la progression totale
                 encaps.Parler.SetTotalProgress(this, new ProgressArg(i, max, false));
                 encaps.Parler.SetStatus(this, new StateArg(Element.Title));

                 // Signal paramétrage de progress part
                 encaps.Parler.SetProgress(this, new ProgressArg(0, 100, false));

                 // Pour chaque catégorie on va lire chaque fichier
                 for (int j = 0; j < files.Length; j++)
                 {
                     // Raz
                     opfSys.RazSums();

                     string sourceFile = files[j];

                     // Transformation du fichier
                     string destFile = sourceFile.Replace(Element.Source, Element.Destination);

                     encaps.Parler.SetStatus(this, new StateArg(sourceFile));
                     encaps.Parler.SetProgress(this, new ProgressArg(j, 100, false));

                     // On met en pause (pourquoi ?)
                     tP.Pause(10);

                     // Gestion de l'état stop
                 /*    if (opfSys.DecisionByDefault == E_Decision.Stop || opfSys.DecisionByDefault == E_Decision.PassAll)
                         tP.TokenSource.Cancel();

                     // On arrête
                     if (tP.CancelToken.IsCancellationRequested)
                         return null;

                     // --- Travail
                     opfSys.CalculateSum();

                     bool ff = opfSys.ManageCopy(sourceFile, destFile, j * 3);*/

            /* }

         }*/

            /*encaps.Parler.SetTotalProgress(this, new ProgressArg(max, max, false));
            encaps.Parler.SetTotalStatus(this, new StateArg("Finished"));
            /* 
            * On décide de conserver par défaut toutes les structures filles des dossiers
            */


            return null;
        }



        /// <summary>
        /// Affiche une fenêtre pour demander quoi faire en cas de conflit
        /// </summary>
        /// <param name="opSender"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private E_Decision? AskWhatToDo(object opSender, EFileResult state, FileArgs srcFA, FileArgs destFA)
        {
            string m = string.Empty;


            if (state == EFileResult.DifferentSize)
            {
                m = "Files have different size";
            }
            else if (state == EFileResult.DifferentHash)
            {
                m = "Files have different hash sum but same size";
            }

            E_Decision decis = E_Decision.None;
            Func<E_Decision> box = delegate ()
            {

                MBDecision window = new MBDecision()
                {
                    Model = new M_Decision()
                    {
                        Message = m,
                        SourceInfo = $"Size: {srcFA.Length}",
                        DestInfo = $"Size: {destFA.Length}"
                    }
                };
                if (window.ShowDialog() == true)
                    return window.Model.Decision;
                else
                    return E_Decision.Stop;
            };

            return decis = IHM.Dispatcher.Invoke(box);

        }



        private void InformErrorSum(object opSender, AsyncProgress.Cont.MessageArg arg)
        {
            IHM.Dispatcher.Invoke(
                  () => DxMBox.ShowDial("Copy problem", "Error", E_DxButtons.Ok, optMessage: arg.Message));

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
