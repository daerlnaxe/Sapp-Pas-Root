using DxPaths.Windows;
using SPR.Containers;
using System;
using System.Collections.Generic;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR
{
    internal static class DebugResources
    {

        /// <summary>
        /// Categories
        /// </summary>
        /// <remarks>
        /// Directory /Category
        /// </remarks>
        static Dictionary<string, string> _Categs = new Dictionary<string, string>()
        {
            { "Manual", "Manuals"  },
            { "Music", "Musics"},
            { "Video", "Videos"},
            { "Banner", "Covers" },
            { "Steam Banner", "Covers" },
            { "Arcade - Cabinet", "Covers" },
            { "Fanart - Box - Front", "Covers" },
            { "Fanart - Card - Front", "Covers" }

        };

        /*  
          /// <summary>
          /// Plateformes
          /// </summary>
          public static Dictionary<string, string> Platform = new Dictionary<string, string>() 
          {
          };*/


        /// <summary>
        /// Renvoi un tableau des chemins de la plateforme
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        internal static IPlatformFolder[] Get_PlatformPaths()
        {
            IPlatformFolder[] raoul = new IPlatformFolder[7];

            MvFolder mvManuel = new MvFolder();
            mvManuel.FolderPath = @"G:\Frontend\LaunchBox\Games\Manuels\Sega Mega Drive";
            mvManuel.MediaType = "Manual";
            raoul[0] = mvManuel;


            MvFolder mvMusic = new MvFolder();
            mvMusic.FolderPath = @"..\..\Games\Music\Sega Mega Drive";
            mvMusic.MediaType = "Music";
            raoul[1] = mvMusic;


            MvFolder mvVideo = new MvFolder();
            mvVideo.FolderPath = @"..\..\Games\Videos\Sega Mega Drive";
            mvVideo.MediaType = "Video";
            raoul[2] = mvVideo;

            // --- Images

            raoul[3] = new MvFolder()
            {
                MediaType = "Banner",
                FolderPath = @"..\..\Games\Covers\Sega Mega Drive\Steam Banner"
            };

            raoul[4] = new MvFolder()
            {
                MediaType = "Arcade - Cabinet",
                FolderPath = @"..\..\Games\Covers\Sega Mega Drive\Arcade - Cabinet"

            };

            raoul[5] = new MvFolder()
            {
                MediaType = "Fanart - Box - Front",
                FolderPath = @"..\..\Games\Covers\Sega Mega Drive\Fanart - Box - Front"
            };

            raoul[6] = new MvFolder()
            {
                MediaType = "Fanart - Card - Front",
                FolderPath = @"..\..\Games\Covers\Sega Mega Drive\Fanart - Card - Front",
            };

            return raoul;
        }

        internal static IGame[] GamesPaths { get; private set; }

        internal static void Set_GamesPaths(string platformName)
        {
            IGame[] iPlatformGames = new IGame[7];

            MvGame cosmic = new MvGame()
            {
                Id = "0",
                Title = "CosmicFantasy2",
                ApplicationPath = $@"..\..\Plateformes\Games\{platformName}\Cosmic Fantasy 2 (USA)\Cosmic_Fantasy_2_(NTSC-U)_[WTG990301].cue"
            };
            iPlatformGames[0] = cosmic;

            MvAdditionnalApplication app1 = new MvAdditionnalApplication()
            {
                Id = "AddAppCosmic1",
                GameId = "1",
                ApplicationPath = @$"..\..\Plateformes\Games\{platformName}\Cosmic Fantasy 2 (USA)\Cosmic_Fantasy_2_(FR)_[WTG990301].cue"

            };

            cosmic.AddNewAdditionalApplication(app1);


            //---

            MvGame exile2 = new MvGame()
            {
                Id = "1",
                Title = "Exile 2",
                ApplicationPath = @$"..\..\Plateformes\Games\{platformName}\Exile 2\Exile_2_-_Wicked_Phenomenon_(NTSC-U)_[WTG990102].cue",
                ManualPath = @$"..\..\Plateformes\Manuals\{platformName}\Exile.pdf",
                MusicPath = @$"..\..\Plateformes\Musics\{platformName}\Exile.mp3",


            };

            exile2.AddNewAdditionalApplication(new MvAdditionnalApplication()
            {
                Id = "addapp1",
                GameId = "1",
                ApplicationPath = @$"..\..\Plateformes\Games\{platformName}\Exile 2\Exile_2_-_Wicked_Phenomenon_(FR).cue",
            });
            exile2.AddNewAdditionalApplication(new MvAdditionnalApplication()
            {
                Id = "addapp2",
                GameId = "1",
                ApplicationPath = @$"..\..\Plateformes\Games\{platformName}\Exile 2\Exile_2_-_Wicked_Phenomenon_(US).cue",
            });

            iPlatformGames[1] = exile2;

            //---           

            iPlatformGames[2] = new MvGame()
            {
                Id = "2",
                Title = "Gate of Thunder",
                ApplicationPath = @$"..\..\Plateformes\Games\{platformName}\Gate Of Thunder\Gate of Thunder (J).cue",
                ManualPath = @$"..\..\Plateformes\Manuals\{platformName}\Gate_of_Thunder_-_Manual_-_T16.pdf",
                MusicPath = @$"..\..\Plateformes\Musics\{platformName}\Gate of Thunder - Intro Music.mp3"


            };

            //---

            MvGame lOT = new MvGame()
            {
                Id = "3",
                Title = "Lords of Thunder",
                ApplicationPath = @$"..\..\Plateformes\Games\{platformName}\Mouais\Lords of Thunder (J).cue",

            };
            lOT.AddNewAdditionalApplication(new MvAdditionnalApplication()
            {
                Id = "addapp2",
                GameId = "1",
                ApplicationPath = @$"..\..\FauxGames\Games\{platformName}\Exile 2\Exile_2_-_Wicked_Phenomenon_(US).cue",
            });
            iPlatformGames[3] = lOT;


            // ---

            iPlatformGames[4] = new MvGame()
            {
                Id = "4",
                Title = "Toe Jam and Earl",
                ApplicationPath = @$".\Games\{platformName}\Toe Jam & Earl\t&j.zip",
                MusicPath = @$".\Musics\{platformName}\Toe Jame & Earl - Funkotron.mp3"

            };

            // ---

            iPlatformGames[5] = new MvGame()
            {
                Id = "5",
                Title = "Flashback",
                ApplicationPath = @$"..\..\Plateformes\{platformName}\{platformName}\FlashBack\Flashback.zip",
            };

            // ---

            iPlatformGames[6] = new MvGame()
            {
                Id = "6",
                Title = "Sonic",

                VideoPath = ""

            };

            GamesPaths = iPlatformGames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="letterDrive">Lettre du lecteur </param>
        /// <param name="root">Root </param>
        /// <param name="folderCont"></param>
        /// <returns></returns>
        /// <remarks>(Ne pas mettre de \)</remarks>
        internal static IPlatformFolder[] Get_PlatformPaths(string name, string letterDrive = null, string root = null)
        {
            // On rajoute un \
            if (!string.IsNullOrEmpty(letterDrive))
                letterDrive += "\\";

            if (!string.IsNullOrEmpty(root))
                root += "\\";

            //string root = string.Empty;

            //IPlatformFolder[] raoul = new IPlatformFolder[7];
            //
            List<IPlatformFolder> lPlat = new List<IPlatformFolder>();
            foreach (KeyValuePair<string, string> categ in _Categs)
            {
                MvFolder mvPath = new MvFolder();

                switch (categ.Value)
                {
                    // --- Images
                    case "Covers":
                        mvPath.FolderPath = DxPath.To_Relative(Global.LaunchBoxRoot, @$"{letterDrive}{root}{categ.Value}\{name}\{categ.Key}");
                        mvPath.MediaType = categ.Key;
                        break;

                    default:
                        mvPath.FolderPath = DxPath.To_Relative(Global.LaunchBoxRoot, @$"{letterDrive}{root}{categ.Value}\{name}");
                        mvPath.MediaType = categ.Key;
                        break;

                }
                lPlat.Add(mvPath);
            }


            return lPlat.ToArray();
        }
    }
}
