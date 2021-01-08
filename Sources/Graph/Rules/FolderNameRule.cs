using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SPR.Graph.Rules
{
    public class FolderNameRule : ValidationRule
    {
        /*
         * value est apparemment passé en copie
         */
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string p = (string)value;

            if (string.IsNullOrEmpty(p))
                return new ValidationResult(false, "Path can't be null or empty");

            Regex r = new Regex("[*?\\\\:/|<>]");
            if (r.IsMatch(p))
            {
                //value = null; // Ne fonctionne pas
                return new ValidationResult(false, @"Folder name can't contain \?*:/|<>");
            }

            return ValidationResult.ValidResult;

        }
    }
}
