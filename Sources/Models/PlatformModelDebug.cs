using SPR.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Models
{
    partial class PlatformModel
    {
        /// <summary>
        /// Initialize parameters with fake needed entries
        /// </summary>
        private void DebugInit()
        {
            Log = "Debug Mode Activé" + Environment.NewLine;

            //_AppPath = @"i:\Frontend\LaunchBox\";


            // Fill a fake array for debug mode with a sample of paths
      

            _PlatformFolders = raoul;


        }
    }
}
