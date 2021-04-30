using Hermes;
using System;
using System.Collections.Generic;
using System.Text;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Cores
{
    public static class Platform_Tools
    {
        public static string GetRoot_ByFolder(string platformRoot)
        {
            string tmp = platformRoot.Replace('/', '\\');
            string[] arrTmp = platformRoot.Split(tmp, '\\');
            return string.Join('\\', arrTmp);
        }

        [Obsolete]
        internal static void SetFolders(in IPlatform platform)
        {
            HeTrace.WriteLine($"[SetFolders] begin for '{platform.Name}'");
            /*if (platform == null)
                throw new Exception("[SetFolders] platform is null");*/

            IEnumerable<IPlatformFolder> platformFolders = platform.GetAllPlatformFolders();
            foreach (var platformFolder in platformFolders)
            {
                if (platformFolder == null || platformFolder.MediaType ==null)
                    continue;

                HeTrace.WriteLine($"\t{platformFolder.MediaType}: '{platformFolder.MediaType}'", level:10);

                if (platformFolder.MediaType.Equals("Manual"))
                    platform.ManualsFolder = platformFolder.FolderPath;
                else if (platformFolder.MediaType.Equals("Music"))
                    platform.MusicFolder = platformFolder.FolderPath;
                else if(platformFolder.MediaType.Equals("Video"))
                    platform.VideosFolder = platformFolder.FolderPath;

            }
            HeTrace.WriteLine($"[SetFolders] finished for '{platform.Name}'");

        }
    }
}
