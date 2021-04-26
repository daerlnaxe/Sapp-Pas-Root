using SPR.Models;
using System.Windows;
using System.Windows.Controls;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour W_Migrate.xaml
    /// </summary>
    public partial class W_Migrate : Window
    {
        MigrateModel _Model;

        public bool ActiveBox { get; set; }

        public W_Migrate(IPlatform selectedPlatform)
        {
            _Model = new MigrateModel(new Page())
            {

            };
            InitializeComponent();

           // _Model.InitializeEdition(selectedPlatform);
            this.DataContext = _Model;
        }




    }
}
