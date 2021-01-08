using SPR.Languages;
using System.Windows;
using System.Windows.Controls;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour C_Platform.xaml
    /// </summary>
    public partial class PlatformControl : UserControl
    {
        #region MediaType
        public static readonly DependencyProperty CategorieProperty =
           DependencyProperty.Register("Categorie", typeof(string), typeof(PlatformControl), new
            PropertyMetadata("", new PropertyChangedCallback(OnCategorieChanged)));

        public string Categorie
        {
            get { return (string)GetValue(CategorieProperty); }
            set { SetValue(CategorieProperty, value); }
        }


        private static void OnCategorieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlatformControl UserControl1Control = d as PlatformControl;
            UserControl1Control.OnCategorieChanged(e);
        }

        private void OnCategorieChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion MediaType


        #region OldHardLink
        public static readonly DependencyProperty OHardLinkProperty =
            DependencyProperty.Register("OHardLink", typeof(string), typeof(PlatformControl), new
                PropertyMetadata("Hardlink", new PropertyChangedCallback(OnHardLinkChanged)));

        public string OHardLink
        {
            get { return (string)GetValue(OHardLinkProperty); }
            set { SetValue(OHardLinkProperty, value); }
        }


        private static void OnHardLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlatformControl UserControl1Control = d as PlatformControl;
            UserControl1Control.OnHardLinkChanged(e);
        }

        private void OnHardLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region OldRelatLink
        public static readonly DependencyProperty ORelatLinkProperty =
            DependencyProperty.Register("ORelatLink", typeof(string), typeof(PlatformControl), new
                PropertyMetadata("RelatLink", new PropertyChangedCallback(OnORelatLinkChanged)));

        public string ORelatLink
        {
            get { return (string)GetValue(ORelatLinkProperty); }
            set { SetValue(ORelatLinkProperty, value); }
        }


        private static void OnORelatLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlatformControl UserControl1Control = d as PlatformControl;
            UserControl1Control.OnORelatLinkChanged(e);
        }

        private void OnORelatLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region NewHardLink
        public static readonly DependencyProperty NHardLinkProperty =
            DependencyProperty.Register("NHardLink", typeof(string), typeof(PlatformControl), new
                PropertyMetadata(SPRLang.Waiting, new PropertyChangedCallback(OnNHardLinkChanged)));

        public string NHardLink
        {
            get { return (string)GetValue(NHardLinkProperty); }
            set { SetValue(NHardLinkProperty, value); }
        }


        private static void OnNHardLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlatformControl UserControl1Control = d as PlatformControl;
            UserControl1Control.OnNHardLinkChanged(e);
        }

        private void OnNHardLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region NewRelatLink
        public static readonly DependencyProperty NRelatLinkProperty =
            DependencyProperty.Register("NRelatLink", typeof(string), typeof(PlatformControl), new
                PropertyMetadata(SPRLang.Waiting, new PropertyChangedCallback(OnNRelatLinkChanged)));

        public string NRelatLink
        {
            get { return (string)GetValue(NRelatLinkProperty); }
            set { SetValue(NRelatLinkProperty, value); }
        }

        private static void OnNRelatLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlatformControl UserControl1Control = d as PlatformControl;
            UserControl1Control.OnNRelatLinkChanged(e);
        }

        private void OnNRelatLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region Size Category
        public static readonly DependencyProperty SizeCategProperty =
            DependencyProperty.Register("SizeCateg", typeof(int), typeof(PlatformControl), new
                PropertyMetadata(500, new PropertyChangedCallback(OnSizeCategChanged)));

        public int SizeCateg
        {
            get { return (int)GetValue(SizeCategProperty); }
            set { SetValue(SizeCategProperty, value); }
        }

        private static void OnSizeCategChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlatformControl UserControl1Control = d as PlatformControl;
            UserControl1Control.OnSizeCategChanged(e);
        }

        private void OnSizeCategChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion
        /*
        #region OldHardLink
        public static readonly DependencyProperty OldHardLink
            .Property =
   DependencyProperty.Register("MediaType", typeof(string), typeof(C_Platform), new
    PropertyMetadata("", new PropertyChangedCallback(OnMediatypeChanged)));

        public string MediaType
        {
            get { return (string)GetValue(MediaTypeProperty); }
            set { SetValue(MediaTypeProperty, value); }
        }
        #endregion
        */

        public PlatformControl()
        {
            InitializeComponent();
            //LayoutRoot.DataContext = this;
        }
    }
}
