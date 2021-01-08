using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Containers
{
    /*
     * Used for debugging
     */
    public class MvGame : IGame
    {
        public string Id { get; set; }
        /// <summary>
        /// Title of game
        /// </summary>
        public string Title { get; set; }
        public string Platform { get; set; }
        public string Publisher { get; set; }
        public string Developer { get; set; }
        public Image RatingImage { get; }
        public bool Favorite { get; set; }

        #region Paths
        public string ScreenshotImagePath { get; }

        public string FrontImagePath { get; }

        public string MarqueeImagePath { get; }

        public string BackImagePath { get; }



        public string Box3DImagePath { get; }

        public string BackgroundImagePath { get; }

        public string Cart3DImagePath { get; }

        public string CartFrontImagePath { get; }

        public string CartBackImagePath { get; }

        public string ClearLogoImagePath { get; }
        public string ApplicationPath { get; set; }
        #endregion Paths
        public string ManualPath { get; set; }
        public string MusicPath { get; set; }



        public string PlatformClearLogoImagePath { get; }


        public string CommandLine { get; set; }
        public bool Completed { get; set; }
        public string ConfigurationCommandLine { get; set; }
        public string ConfigurationPath { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public string DosBoxConfigurationPath { get; set; }
        public string EmulatorId { get; set; }

        public string DetailsWithPlatform { get; }

        public string DetailsWithoutPlatform { get; }
        public DateTime? LastPlayedDate { get; set; }
        public string Notes { get; set; }
        public string Rating { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? ReleaseYear { get; set; }
        public string RootFolder { get; set; }
        public bool ScummVmAspectCorrection { get; set; }
        public bool ScummVmFullscreen { get; set; }
        public string ScummVmGameDataFolderPath { get; set; }
        public string ScummVmGameType { get; set; }
        public bool ShowBack { get; set; }
        public string SortTitle { get; set; }
        public string Source { get; set; }
        public bool OverrideDefaultStartupScreenSettings { get; set; }
        public bool UseStartupScreen { get; set; }
        public bool HideAllNonExclusiveFullscreenWindows { get; set; }
        public int StartupLoadDelay { get; set; }
        public bool HideMouseCursorInGame { get; set; }
        public bool DisableShutdownScreen { get; set; }
        public bool AggressiveWindowHiding { get; set; }
        public bool? Installed { get; set; }
        public int StarRating { get; set; }

        public float CommunityOrLocalStarRating { get; }

        public float StarRatingFloat { get; set; }
        public float CommunityStarRating { get; set; }
        public int CommunityStarRatingTotalVotes { get; set; }
        public string Status { get; set; }
        public int? LaunchBoxDbId { get; set; }
        public int? WikipediaId { get; set; }
        public string WikipediaUrl { get; set; }
        public bool UseDosBox { get; set; }
        public bool UseScummVm { get; set; }
        public string Version { get; set; }
        public string Series { get; set; }
        public string PlayMode { get; set; }
        public string Region { get; set; }
        public int PlayCount { get; set; }
        public bool Portable { get; set; }
        public string VideoPath { get; set; }
        public string ThemeVideoPath { get; set; }
        public bool Hide { get; set; }
        public bool Broken { get; set; }
        public string CloneOf { get; set; }
        public string GenresString { get; set; }

        public BlockingCollection<string> Genres { get; }

        public string[] PlayModes { get; }

        public string[] Developers { get; }

        public string[] Publishers { get; }

        public string[] SeriesValues { get; }

        public string SortTitleOrTitle { get; }

        public string ReleaseType { get; set; }
        public int? MaxPlayers { get; set; }
        public string VideoUrl { get; set; }

        public IAdditionalApplication AddNewAdditionalApplication()
        {
            throw new NotImplementedException();
        }

        public IAlternateName AddNewAlternateName()
        {
            throw new NotImplementedException();
        }

        public ICustomField AddNewCustomField()
        {
            throw new NotImplementedException();
        }

        public IMount AddNewMount()
        {
            throw new NotImplementedException();
        }

        public string Configure()
        {
            throw new NotImplementedException();
        }

        public IAdditionalApplication[] GetAllAdditionalApplications()
        {
            // Pour le mode débug
            return AdditionnalApplications.ToArray();
        }

        public IAlternateName[] GetAllAlternateNames()
        {
            throw new NotImplementedException();
        }

        public ICustomField[] GetAllCustomFields()
        {
            throw new NotImplementedException();
        }

        public ImageDetails[] GetAllImagesWithDetails()
        {
            throw new NotImplementedException();
        }

        public ImageDetails[] GetAllImagesWithDetails(string imageType)
        {
            throw new NotImplementedException();
        }

        public IMount[] GetAllMounts()
        {
            throw new NotImplementedException();
        }

        public string GetBigBoxDetails(bool showPlatform)
        {
            throw new NotImplementedException();
        }

        public string GetManualPath()
        {
            throw new NotImplementedException();
        }

        public string GetMusicPath()
        {
            throw new NotImplementedException();
        }

        public string GetNewManualFilePath(string extension)
        {
            throw new NotImplementedException();
        }

        public string GetNewMusicFilePath(string extension)
        {
            throw new NotImplementedException();
        }

        public string GetNewThemeVideoFilePath(string extension)
        {
            throw new NotImplementedException();
        }

        public string GetNewVideoFilePath(string extension)
        {
            throw new NotImplementedException();
        }

        public string GetNextAvailableImageFilePath(string extension, string imageType, string region)
        {
            throw new NotImplementedException();
        }

        public string GetNextVideoFilePath(string videoType, string extension)
        {
            throw new NotImplementedException();
        }

        public string GetThemeVideoPath()
        {
            throw new NotImplementedException();
        }

        public string GetVideoPath(bool prioritizeThemeVideos = false)
        {
            throw new NotImplementedException();
        }

        public string GetVideoPath(string videoType)
        {
            throw new NotImplementedException();
        }

        public string OpenFolder()
        {
            throw new NotImplementedException();
        }

        public string OpenManual()
        {
            throw new NotImplementedException();
        }

        public string Play()
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveAdditionalApplication(IAdditionalApplication additionalApplication)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveAlternateNames(IAlternateName alternateName)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveCustomField(ICustomField customField)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveMount(IMount mount)
        {
            throw new NotImplementedException();
        }

        // --- Utilisé en Debug, rajouté

        private List<IAdditionalApplication> AdditionnalApplications = new List<IAdditionalApplication>();



        internal void AddNewAdditionalApplication(MvAdditionnalApplication app)
        {
            AdditionnalApplications.Add(app);
        }
    }
}
