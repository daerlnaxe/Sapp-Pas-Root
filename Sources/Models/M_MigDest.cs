using SPR.Languages;
using System;
using System.Collections.Generic;
using System.Text;
using static SPR.Properties.Settings;

namespace SPR.Models
{
    class M_MigDest : AChooseFolder
    {
        public override string Info => SPRLang.Choose_Dest_Folder;
        public override string StartingFolder { get; set; }


        public M_MigDest()
        {            
            StartingFolder = Default.MigDestPath;

            if (string.IsNullOrEmpty(StartingFolder))
                StartingFolder = System.IO.Path.Combine(Global.LaunchBoxRoot, Default.AppsFolder);

        }

        public override void Browse_Executed(string linkResult)
        {
            ResultFolder = linkResult;
            Default.MigDestPath = linkResult;
            Default.Save();
        }
    }
}
