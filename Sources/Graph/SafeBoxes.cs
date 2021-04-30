using DxLocalTransf;
using DxLocalTransf.Cont;
using DxLocalTransf.Tools;
using DxTBoxCore.Box_Decisions;
using DxTBoxCore.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace SPR.Graph
{
    class SafeBoxes
    {
        internal static E_Decision? HashCopy_AskToUser(object sender, EFileResult state, FileArgs fileSrc, FileArgs fileDest)
        {
            return Application.Current.Dispatcher?.Invoke
                (
                    () =>
                    {

                        DxTBoxCore.Box_Decisions.MBDecision boxDeciz = new DxTBoxCore.Box_Decisions.MBDecision()
                        {
                            Model = new DxTBoxCore.Box_Decisions.M_Decision()
                            {
                                Message = $"Overwrite ? ({state})",
                                Source = fileSrc.Path,
                                Destination = fileDest.Path,

                                SourceInfo = FileSizeFormatter.Convert(fileSrc.Length),
                                DestInfo = FileSizeFormatter.Convert(fileDest.Length),
                            },
                            Buttons = E_DxConfB.OverWrite | E_DxConfB.Pass | E_DxConfB.Trash,
                        };

                        if (boxDeciz.ShowDialog() == true)
                        {
                            return boxDeciz.Model.Decision;
                        }

                        return E_Decision.None;

                    }
                );


        }
    }
}
