using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace SPR.Graph.Rules
{



    public class DirectoryRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string p = (string)value;

            if (string.IsNullOrEmpty(p))
                return new ValidationResult(false, "Path can't be null or empty");

            if (!Directory.Exists(p))
                return new ValidationResult(false, "Path doesn't exist");

            return ValidationResult.ValidResult;

        }
    }
}
