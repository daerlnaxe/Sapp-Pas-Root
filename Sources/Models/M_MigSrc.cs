using SPR.Languages;
using System;
using System.Collections.Generic;
using System.Text;
using static SPR.Properties.Settings;

namespace SPR.Models
{
    class M_MigSrc : AChooseFolder
    {
        public override string Info => SPRLang.Choose_Source_Folder;

        public override string StartingFolder { get; set; }

        public M_MigSrc()
        {
            StartingFolder = Default.MigSrcPath;

            if (string.IsNullOrEmpty(StartingFolder))
                StartingFolder = System.IO.Path.Combine(Global.LaunchBoxRoot, Default.AppsFolder);
        }

        public override void Browse_Executed(string linkResult)
        {
            ResultFolder = linkResult;
            Default.MigSrcPath = System.IO.Path.GetFileName( linkResult); // on redescend d'un cran pour obliger l'utilisateur à choisir)
            Default.Save();

        }
    }
}
