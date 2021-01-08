using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour CC_SDPaths.xaml
    /// </summary>
    public partial class CC_PairPaths : UserControl, INotifyDataErrorInfo
    {
        #region Largeur de la première colonne (utilisé pour harmoniser)


        #endregion

        #region Title
        public static readonly DependencyProperty TitleProperty =
                DependencyProperty.Register("Title", typeof(string), typeof(CC_PairPaths), new
                    PropertyMetadata("", new PropertyChangedCallback(OnTitleChanged)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_PairPaths cGameControl = d as CC_PairPaths;
            cGameControl.OnTitleChanged(e);
        }

        private void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }
        #endregion

        #region FirstPathName
        public static readonly DependencyProperty FirstPathNameProperty =
        DependencyProperty.Register("FirstPathName", typeof(string), typeof(CC_PairPaths), new
            PropertyMetadata("First Path Name", new PropertyChangedCallback(OnFirstPathNameChanged)));

        public string FirstPathName
        {
            get { return (string)GetValue(FirstPathNameProperty); }
            set { SetValue(FirstPathNameProperty, value); }
        }


        private static void OnFirstPathNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_PairPaths cGameControl = d as CC_PairPaths;
            cGameControl.OnFirstPathNameChanged(e);
        }

        private void OnFirstPathNameChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }

        #endregion FirstPathName

        #region SecondPathName
        public static readonly DependencyProperty SecondPathNameProperty =
        DependencyProperty.Register("SecondPathName", typeof(string), typeof(CC_PairPaths), new
            PropertyMetadata("Second Path Name", new PropertyChangedCallback(OnSecondPathNameChanged)));

        public string SecondPathName
        {
            get { return (string)GetValue(SecondPathNameProperty); }
            set { SetValue(SecondPathNameProperty, value); }
        }


        private static void OnSecondPathNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_PairPaths cGameControl = d as CC_PairPaths;
            cGameControl.OnSecondPathNameChanged(e);
        }

        private void OnSecondPathNameChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }

        #endregion FirstPath

        #region FirstPath
        /*public static readonly DependencyProperty FirstPathProperty =
        DependencyProperty.Register("FirstPath", typeof(string), typeof(CC_PairPaths), new
            PropertyMetadata("First Path ", new PropertyChangedCallback(OnFirstPathChanged)));*/

        public static DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(CC_PairPaths), new
                FrameworkPropertyMetadata("First Path", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null, false, UpdateSourceTrigger.PropertyChanged));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }


        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_PairPaths cGameControl = d as CC_PairPaths;
            cGameControl.OnSourceChanged(e);
        }

        private void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {

            //tbTest.Text = e.NewValue.ToString();
        }

        #endregion FirstPath



        #region SecondPath
        public static readonly DependencyProperty SecondPathProperty =
        DependencyProperty.Register("SecondPath", typeof(string), typeof(CC_PairPaths), new
            PropertyMetadata("Second Path ", new PropertyChangedCallback(OnSecondPathChanged)));


        public string SecondPath
        {
            get { return (string)GetValue(SecondPathProperty); }
            set { SetValue(SecondPathProperty, value); }
        }


        private static void OnSecondPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CC_PairPaths cGameControl = d as CC_PairPaths;
            cGameControl.OnSecondPathChanged(e);
        }

        private void OnSecondPathChanged(DependencyPropertyChangedEventArgs e)
        {
            //tbTest.Text = e.NewValue.ToString();
        }


        public CC_PairPaths()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((INotifyDataErrorInfo)DataContext).ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(go);

        }

        private void go(object sender, DataErrorsChangedEventArgs e)
        {


            ErrorsChanged?.Invoke(this, e);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get
            {
                if (DataContext == null)
                    return false;

                var test = (INotifyDataErrorInfo)DataContext;

                return test.HasErrors;
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return ((INotifyDataErrorInfo)DataContext).GetErrors(propertyName);
        }
        #endregion FirstPathName

        #region Errors
        /*
        public string Error => "FirstPath";

        public string this[string columnName]
        {
            get
            {
                var test = Validation.GetHasError(this);
                Debug.WriteLine($" -- {test}");
                // use a specific validation or ask for UserControl Validation Error 
                //return Validation.GetHasError(this) ? "UserControl has Error" : null;
                return "";
            }
        }*/

        #endregion


    }
}
