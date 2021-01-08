using SPR.Languages;
using System.Windows;
using System.Windows.Controls;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour C_Game.xaml
    /// </summary>
    public partial class CC_Game : UserControl//, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        /*public delegate void PanelOpenEvent(object objet, bool value);

        public PanelOpenEvent OnclickEvent;*/

        #region panel event
        public static readonly RoutedEvent OnClickPanelEvent =
            EventManager.RegisterRoutedEvent("OnClickPanel",
                 RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                 typeof(CC_Game));

        public event RoutedEventHandler OnClickPanel
        {
            add { AddHandler(OnClickPanelEvent, value); }
            remove { RemoveHandler(OnClickPanelEvent, value); }
        }
        #endregion


        #region Title
        public static readonly DependencyProperty TitleProperty =
                DependencyProperty.Register("Title", typeof(string), typeof(CC_Game), new
                    PropertyMetadata("", new PropertyChangedCallback(OnTitleChanged)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_Game cGameControl = d as CC_Game;
            cGameControl.OnTitleChanged(e);
        }

        private void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region OldHardLink
        public static readonly DependencyProperty OHardLinkProperty =
            DependencyProperty.Register("OHardLink", typeof(string), typeof(CC_Game), new
                PropertyMetadata("Hardlink", new PropertyChangedCallback(OnHardLinkChanged)));

        public string OHardLink
        {
            get { return (string)GetValue(OHardLinkProperty); }
            set { SetValue(OHardLinkProperty, value); }
        }


        private static void OnHardLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_Game UserControl1Control = d as CC_Game;
            UserControl1Control.OnHardLinkChanged(e);
        }

        private void OnHardLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region OldRelatLink
        public static readonly DependencyProperty ORelatLinkProperty =
            DependencyProperty.Register("ORelatLink", typeof(string), typeof(CC_Game), new
                PropertyMetadata("RelatLink", new PropertyChangedCallback(OnORelatLinkChanged)));

        public string ORelatLink
        {
            get { return (string)GetValue(ORelatLinkProperty); }
            set { SetValue(ORelatLinkProperty, value); }
        }


        private static void OnORelatLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_Game UserControl1Control = d as CC_Game;
            UserControl1Control.OnORelatLinkChanged(e);
        }

        private void OnORelatLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region NewHardLink
        public static readonly DependencyProperty NHardLinkProperty =
            DependencyProperty.Register("NHardLink", typeof(string), typeof(CC_Game), new
                PropertyMetadata(SPRLang.Waiting, new PropertyChangedCallback(OnNHardLinkChanged)));

        public string NHardLink
        {
            get { return (string)GetValue(NHardLinkProperty); }
            set { SetValue(NHardLinkProperty, value); }
        }


        private static void OnNHardLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_Game UserControl1Control = d as CC_Game;
            UserControl1Control.OnNHardLinkChanged(e);
        }

        private void OnNHardLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region NewRelatLink
        public static readonly DependencyProperty NRelatLinkProperty =
            DependencyProperty.Register("NRelatLink", typeof(string), typeof(CC_Game), new
                PropertyMetadata(SPRLang.Waiting, new PropertyChangedCallback(OnNRelatLinkChanged)));



        public string NRelatLink
        {
            get { return (string)GetValue(NRelatLinkProperty); }
            set { SetValue(NRelatLinkProperty, value); }
        }

        private static void OnNRelatLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_Game UserControl1Control = d as CC_Game;
            UserControl1Control.OnNRelatLinkChanged(e);
        }

        private void OnNRelatLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region
        public static readonly DependencyProperty ValidityProperty =
            DependencyProperty.Register("Validity", typeof(bool), typeof(CC_Game),
                new PropertyMetadata(false, new PropertyChangedCallback(OnValidityChanged)));

        public bool Validity
        {
            get { return (bool)GetValue(ValidityProperty); }
            set { SetValue(ValidityProperty, value); }
        }

        private static void OnValidityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // ---
        }
        #endregion

        #region panel open
        public static readonly DependencyProperty PanelOpenProperty =
    DependencyProperty.Register("PanelOpen", typeof(bool), typeof(CC_Game), new
        PropertyMetadata(false, new PropertyChangedCallback(OnPanelOpenChanged)));

        public bool PanelOpen
        {
            get { return (bool)GetValue(PanelOpenProperty); }
            set { SetValue(PanelOpenProperty, value); }
        }

        private static void OnPanelOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_Game UserControl1Control = d as CC_Game;
            UserControl1Control.OnPanelOpenChanged(e);
        }

        private void OnPanelOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion panel open

        #region NotifyProperty
        /*private bool _PanelOpen;
        public bool PanelOpen
        {
            get { return _PanelOpen; }
            set
            {
                _PanelOpen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PanelOpen"));

            }
        }*/
        #endregion


        public CC_Game()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PanelOpen == false)
                PanelOpen = true;
            else
                PanelOpen = false;


            // OnclickEvent?.Invoke(this, PanelOpen) ;
            RoutedEventArgs newEventArgs =
               new RoutedEventArgs(CC_Game.OnClickPanelEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
