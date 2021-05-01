using Hermes;
using SPR.Containers;
using SPR.Languages;
using SPR.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour C_Game.xaml
    /// </summary>
    public partial class CC_GamePlus : UserControl//, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        /*public delegate void PanelOpenEvent(object objet, bool value);

        public PanelOpenEvent OnclickEvent;*/

        #region panel event
        public static readonly RoutedEvent OnClickPanelEvent =
            EventManager.RegisterRoutedEvent("OnClickPanel",
                 RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                 typeof(CC_GamePlus));

        public event RoutedEventHandler OnClickPanel
        {
            add { AddHandler(OnClickPanelEvent, value); }
            remove { RemoveHandler(OnClickPanelEvent, value); }
        }
        #endregion


        #region Title
        public static readonly DependencyProperty TitleProperty =
                DependencyProperty.Register("Title", typeof(string), typeof(CC_GamePlus), new
                    PropertyMetadata("", new PropertyChangedCallback(OnTitleChanged)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_GamePlus cGameControl = d as CC_GamePlus;
            cGameControl.OnTitleChanged(e);
        }

        private void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region OldHardLink
        public static readonly DependencyProperty OHardLinkProperty =
            DependencyProperty.Register("OHardLink", typeof(string), typeof(CC_GamePlus), new
                PropertyMetadata("Hardlink", new PropertyChangedCallback(OnHardLinkChanged)));

        public string OHardLink
        {
            get { return (string)GetValue(OHardLinkProperty); }
            set { SetValue(OHardLinkProperty, value); }
        }


        private static void OnHardLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_GamePlus UserControl1Control = d as CC_GamePlus;
            UserControl1Control.OnHardLinkChanged(e);
        }

        private void OnHardLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region OldRelatLink
        public static readonly DependencyProperty ORelatLinkProperty =
            DependencyProperty.Register(nameof(ORelatLink), typeof(string), typeof(CC_GamePlus)/*, new
                PropertyMetadata("RelatLink", new PropertyChangedCallback(OnORelatLinkChanged))*/);

        public string ORelatLink
        {
            get { return (string)GetValue(ORelatLinkProperty); }
            set 
            {
                Debug.WriteLine("Set ORelatLink");
                SetValue(ORelatLinkProperty, value); 
            }
        }

        
        private static void OnORelatLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_GamePlus UserControl1Control = d as CC_GamePlus;
            UserControl1Control.OnORelatLinkChanged(e);
        }

        private void OnORelatLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region NewHardLink
        public static readonly DependencyProperty NHardLinkProperty =
            DependencyProperty.Register("NHardLink", typeof(string), typeof(CC_GamePlus), new
                PropertyMetadata(SPRLang.Waiting, new PropertyChangedCallback(OnNHardLinkChanged)));

        public string NHardLink
        {
            get { return (string)GetValue(NHardLinkProperty); }
            set { SetValue(NHardLinkProperty, value); }
        }


        private static void OnNHardLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_GamePlus UserControl1Control = d as CC_GamePlus;
            UserControl1Control.OnNHardLinkChanged(e);
        }

        private void OnNHardLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region NewRelatLink
        public static readonly DependencyProperty NRelatLinkProperty =
            DependencyProperty.Register("NRelatLink", typeof(string), typeof(CC_GamePlus), new
                PropertyMetadata(SPRLang.Waiting, new PropertyChangedCallback(OnNRelatLinkChanged)));

        public string NRelatLink
        {
            get { return (string)GetValue(NRelatLinkProperty); }
            set { SetValue(NRelatLinkProperty, value); }
        }

        private static void OnNRelatLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_GamePlus UserControl1Control = d as CC_GamePlus;
            UserControl1Control.OnNRelatLinkChanged(e);
        }

        private void OnNRelatLinkChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion


        #region

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source),
                                                        typeof(IEnumerable<CState>), typeof(CC_GamePlus));

        public IEnumerable<CState> Source
        {
            get => (IEnumerable<CState>)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);

        }
        
        /*
        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_GamePlus ccGP = (CC_GamePlus)d;
            ccGP.SourceChange(e);
        }

        private void SourceChange(DependencyPropertyChangedEventArgs e)
        {
            Dictionary<string, bool> src = (Dictionary<string, bool>)e.NewValue;
            Source = src;
            // throw new NotImplementedException();
        }*/

        public static readonly DependencyProperty TrueColorProperty = DependencyProperty.Register("TrueColor",
                                                        typeof(Color), typeof(CC_GamePlus),
                                                        new PropertyMetadata(Colors.Green));

        public Color TrueColor
        {
            get => (Color)GetValue(TrueColorProperty);
            set => SetValue(TrueColorProperty, value);
        }

        public static readonly DependencyProperty FalseColorProperty = DependencyProperty.Register("FalseColor",
                                                        typeof(Color), typeof(CC_GamePlus),
                                                        new PropertyMetadata(Colors.Red));

        public Color FalseColor
        {
            get => (Color)GetValue(FalseColorProperty);
            set => SetValue(FalseColorProperty, value);
        }
        #endregion


        #region
        public static readonly DependencyProperty ValidityProperty =
            DependencyProperty.Register("Validity", typeof(bool?), typeof(CC_GamePlus),
                new PropertyMetadata(null, new PropertyChangedCallback(OnValidityChanged)));

        public bool? Validity
        {
            get { return (bool?)GetValue(ValidityProperty); }
            set { SetValue(ValidityProperty, value); }
        }

        private static void OnValidityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // ---
        }
        #endregion

        #region panel open
        public static readonly DependencyProperty PanelOpenProperty =
    DependencyProperty.Register("PanelOpen", typeof(bool), typeof(CC_GamePlus), new
        PropertyMetadata(false, new PropertyChangedCallback(OnPanelOpenChanged)));

        public bool PanelOpen
        {
            get { return (bool)GetValue(PanelOpenProperty); }
            set { SetValue(PanelOpenProperty, value); }
        }

        private static void OnPanelOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_GamePlus UserControl1Control = d as CC_GamePlus;
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


        public CC_GamePlus()
        {
            InitializeComponent();
            // --- JAMAIS datacontext
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
