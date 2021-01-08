using Hermes;
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
                /*  if(_Platforms != value)
                  {*/
                _Platforms = value;
                OnPropertyChanged();
                /*}*/
            }
        }

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
    }
}
