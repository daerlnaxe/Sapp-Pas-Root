using System;
using System.Collections.Generic;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Containers
{
    internal class MvPlatform : IPlatform
    {

        /// <summary>
        /// Platform Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Platform folder path
        /// </summary>
        public string Folder { get; set; }



        public string BannerImagePath { get; }

        public string DeviceImagePath { get; }

        public string ClearLogoImagePath { get; }

        public string BackgroundImagePath { get; }

        public string DefaultBoxImagePath { get; }

        public string Default3DBoxImagePath { get; }

        public string DefaultCartImagePath { get; }

        public string Default3DCartImagePath { get; }

        public string SortTitleOrTitle { get; }

        public string PlatformCategoryClearLogoImagePath { get; }

        public bool IsEmulated { get; }


        public string NestedName { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Developer { get; set; }
        public string Manufacturer { get; set; }
        public string Cpu { get; set; }
        public string Memory { get; set; }
        public string Graphics { get; set; }
        public string Sound { get; set; }
        public string Display { get; set; }
        public string Media { get; set; }
        public string MaxControllers { get; set; }
        public string Notes { get; set; }
        public string VideosFolder { get; set; }
        public string FrontImagesFolder { get; set; }
        public string BackImagesFolder { get; set; }
        public string ClearLogoImagesFolder { get; set; }
        public string FanartImagesFolder { get; set; }
        public string ScreenshotImagesFolder { get; set; }
        public string BannerImagesFolder { get; set; }
        public string SteamBannerImagesFolder { get; set; }
        public string ManualsFolder { get; set; }
        public string MusicFolder { get; set; }
        public string ScrapeAs { get; set; }
        public string VideoPath { get; set; }
        public string ImageType { get; set; }
        public string SortTitle { get; set; }
        public string Category { get; set; }
        public string LastGameId { get; set; }
        public string BigBoxView { get; set; }
        public string BigBoxTheme { get; set; }


        public IGame[] GetAllGames(bool includeHidden, bool includeBroken)
        {
            throw new NotImplementedException();
        }

        public IGame[] GetAllGames(bool includeHidden, bool includeBroken, bool excludeGamesMissingVideos, bool excludeGamesMissingBoxFrontImage, bool excludeGamesMissingScreenshotImage, bool excludeGamesMissingClearLogoImage, bool excludeGamesMissingBackgroundImage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IPlatformFolder[] GetAllPlatformFolders()
        {
            string folderCont = string.Empty;
            // Cherche les plateformes dans le debug
            switch (Name)
            {
                case "Sega Master System":
                    return DebugResources.Get_PlatformPaths(Name);
                case "Metzo":
                    folderCont = Folder.Substring(Folder.LastIndexOf(@"\") + 1);
                    return DebugResources.Get_PlatformPaths(folderCont, "G:", @"Frontend\Plateformes");

                default:

                    folderCont = Folder.Substring(Folder.LastIndexOf(@"\") + 1);
                    return DebugResources.Get_PlatformPaths(folderCont, @"G:", "Plateformes");

            }

            
        }

        public IList<IPlatform> GetChildren()
        {
            throw new NotImplementedException();
        }

        public int GetGameCount(bool includeHidden, bool includeBroken)
        {
            throw new NotImplementedException();
        }

        public int GetGameCount(bool includeHidden, bool includeBroken, bool excludeGamesMissingVideos, bool excludeGamesMissingBoxFrontImage, bool excludeGamesMissingScreenshotImage, bool excludeGamesMissingClearLogoImage, bool excludeGamesMissingBackgroundImage)
        {
            throw new NotImplementedException();
        }

        public string GetNewPlatformLogoPath(string url)
        {
            throw new NotImplementedException();
        }

        public string GetNewPlatformVideoPath(string url)
        {
            throw new NotImplementedException();
        }

        public IPlatformFolder GetPlatformFolderByImageType(string imageType)
        {
            throw new NotImplementedException();
        }

        public string GetPlatformVideoPath(bool fallBackToGameVideos = true, bool allowThemePath = true)
        {
            throw new NotImplementedException();
        }

        public bool HasGames(bool includeHidden, bool includeBroken)
        {
            throw new NotImplementedException();
        }

        public bool HasGames(bool includeHidden, bool includeBroken, bool excludeGamesMissingVideos, bool excludeGamesMissingBoxFrontImage, bool excludeGamesMissingScreenshotImage, bool excludeGamesMissingClearLogoImage, bool excludeGamesMissingBackgroundImage)
        {
            throw new NotImplementedException();
        }
    }
}
