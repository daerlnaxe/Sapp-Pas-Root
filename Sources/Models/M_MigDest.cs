using DxTBoxCore.BoxChoose;
using DxTBoxCore.Languages;
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

        public override void Browse_Executed()
        {
            TreeChoose cf = new TreeChoose()
            {
                SaveButtonName = DxTBLang.Select,
                CancelButtonName = DxTBLang.Cancel,
                Model = new M_ChooseFolder()
                {
                    Info = DxTBLang.Select,
                    HideWindowsFolder = true,
                    PathCompareason = System.StringComparison.CurrentCultureIgnoreCase,
                    StartingFolder = StartingFolder
                }
            };

            if (cf.ShowDialog() == true)
            {
                ResultFolder = cf.LinkResult;
                Default.MigDestPath = cf.LinkResult;
                Default.Save();
            }
        }
    }
}
