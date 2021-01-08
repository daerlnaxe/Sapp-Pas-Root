using SPR.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SPR.Graph.Converters
{
    class GameModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //            Enum.TryParse<GamePathMode>(value.ToString(), out GamePathMode mode);
            //switch (mode)
            switch ((GamePathMode)value)
            {
                case GamePathMode.Forced:
                    return 1;

                case GamePathMode.KeepSubFolders:
                    return 2;

                default:
                    return 0;

            }
            /*
            GamePathMode mode = (GamePathMode)value;
            int param = Int32.Parse(parameter.ToString());
            if (mode == GamePathMode.Forced && param == 1)
                return true;
            else if (mode == GamePathMode.KeepSubFolders && param == 2)
                return true;

            return false;
            */

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum.TryParse<GamePathMode>(value.ToString(), out GamePathMode mode);
            return mode;
        }
    }
}
