using DxTBoxCore.Box_MBox;
using DxTBoxCore.Common;
using Hermes;
using SPR.Containers;
using SPR.Graph;
using SPR.Languages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Models
{
    class PlateformsListModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //public IPlatform PreviousPlatformState { get; private set; }
        public C_Platform CBAckupPlatform { get; private set; }

        private IPlatform _SelectedPlatform;
        /// <summary>
        /// Selected Platform
        /// </summary>
        public IPlatform SelectedPlatform
        {
            get { return _SelectedPlatform; }
            set
            {
                _SelectedPlatform = value;
            }
        }


        // LaunchBox refuse two platforms with same name (to watching)
        private ObservableCollection<IPlatform> _Platforms = new ObservableCollection<IPlatform>();
        public ObservableCollection<IPlatform> Platforms
        {
            get { return _Platforms; }
            set
            {
                _Platforms = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        internal void Initialize()
        {
            //
            if (!Global.DebugMode)
            {
                Platforms = ListPlatform();
            }
            else if (Global.DebugMode)
            {
                //HeTrace.GetMessenger("Debug").AddCaller(this, "PlatformLists >>>");
                Platforms = DebugPoint.FakePlatforms;
            }

            HeTrace.WriteLine("Platforms initialized", this);
        }

        /// <summary>
        /// List Platforms, create an item for each
        /// </summary>
        /// <remarks>
        /// Raz before
        /// </remarks>
        private ObservableCollection<IPlatform> ListPlatform()
        {
            HeTrace.WriteLine("List platforms", this);
            IPlatform[] platforms = PluginHelper.DataManager.GetAllPlatforms();

            // Remise à zéro
            ObservableCollection<IPlatform> tmp = new ObservableCollection<IPlatform>();

            // ObservableCollection<IPlatform> tmp = new ObservableCollection<IPlatform>();
            foreach (IPlatform platform in platforms)
            {
                //MessageBox.Show(platform.Name);
                /*    ListViewItem lvi = new ListViewItem(platform.Name);
                    lvi.SubItems.Add(platform.Folder);
                    lvPlatforms.Items.Add(lvi);*/

                //dicPlatforms.Add(platform.Name, platform);

                tmp.Add(platform);
                HeTrace.WriteLine($"{platform.Name} ajouté", 10);
            }

            HeTrace.WriteLine("ListPlatform achieved", 10);
            return tmp;

        }

        internal void EditPlatform()
        {
            try
            {
                //PreviousPlatformState = SelectedPlatform;
                // Cliché des dossiers
                CBAckupPlatform =
                    C_Platform.Platform_Maker(SelectedPlatform, SelectedPlatform.GetAllPlatformFolders());

                // Lancement de la modification des paths
                W_PlatformPaths wPP = new W_PlatformPaths()
                {
                    Model = new PlatformModel(SelectedPlatform.Name)
                };

                if (wPP.ShowDialog() != true)
                    return;

                // Rafraichissement
                if (!Global.DebugMode)// && !wp.Model.PlatformObject.Folder.Equals(oldPath))
                {
                    Initialize();

                    //SelectedPlatform = PluginHelper.DataManager.GetPlatformByName(CBAckupPlatform.PlatformName);

                }

                if (!CBAckupPlatform.ApplicationPath.OldPath.Equals(SelectedPlatform.Folder)
                    && DxMBox.ShowDial(SPRLang.QChange_GamesPaths, "Question", E_DxButtons.No | E_DxButtons.Yes)== true)
                {
                    W_GamePaths wp = new W_GamePaths()
                    {
                        Model = new GamesModel(SelectedPlatform, CBAckupPlatform?.PlatformName),
                    };
                    wp.ShowDialog();
                }
            }

            catch (Exception exc)
            {
                DxMBox.ShowDial(exc.Message);
                HeTrace.WriteLine(exc.Message);
                HeTrace.WriteLine(exc.StackTrace);
            }
        }

        internal void EditGames()
        {
            try
            {

                W_GamePaths wp = new W_GamePaths()
                {
                    Model = new GamesModel(SelectedPlatform),
                };
                wp.ShowDialog();
            }
            catch (Exception exc)
            {
                DxMBox.ShowDial(exc.Message);
                HeTrace.WriteLine(exc.Message);
                HeTrace.WriteLine(exc.StackTrace);
            }

        }

        internal void LockedMigrate()
        {
            try
            {
                W_LockedMigrate wLM = new W_LockedMigrate()
                {
                    Model = new MigrateLModel(CBAckupPlatform, SelectedPlatform),

                };
                wLM.ShowDialog();
            }
            catch (Exception exc)
            {
                DxMBox.ShowDial(exc.Message);
                HeTrace.WriteLine(exc.Message);
                HeTrace.WriteLine(exc.StackTrace);
            }
        }
    }
}
