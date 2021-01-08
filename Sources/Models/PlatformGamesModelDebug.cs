using SPR.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Models
{
    /// <summary>
    /// Initialize parameters with fake needed entries
    /// </summary>
    partial class PlatformGamesModel
    {
        private void DebugInit()
        {
            Log = "Debug Mode Activé" + Environment.NewLine;

            _IPlatformGames = new IGame[7];

            MvGame cosmic = new MvGame()
            {
                Title = "CosmicFantasy2",
                ApplicationPath = @"..\..\Plateformes\Games\NEC TurboGrafx-CD\Cosmic Fantasy 2 (USA)\Cosmic_Fantasy_2_(NTSC-U)_[WTG990301].cue"
            };
            _IPlatformGames[0] = cosmic;

            MvAdditionnalApplication app1 = new MvAdditionnalApplication()
            {

            };

            cosmic.AddNewAdditionalApplication(app1);


            //---

            MvGame exile2 = new MvGame()
            {
                Title = "Exile 2",
                ApplicationPath = @"..\..\Plateformes\Games\NEC TurboGrafx-CD\Exile 2\Exile_2_-_Wicked_Phenomenon_(NTSC-U)_[WTG990102].cue",
                ManualPath = @"..\..\Plateformes\Manuals\NEC TurboGrafx-CD\Exile.pdf",
                MusicPath = @"..\..\Plateformes\Musics\NEC TurboGrafx-CD\Exile.mp3"

            };

            _IPlatformGames[1] = exile2;

            //---           

            _IPlatformGames[2] = new MvGame()
            {
                Title = "Gate of Thunder",
                ApplicationPath = @"..\..\Plateformes\Games\NEC TurboGrafx-CD\Gate Of Thunder\Gate of Thunder (J).cue",
                ManualPath = @"..\..\Plateformes\Manuals\NEC TurboGrafx-CD\Gate_of_Thunder_-_Manual_-_T16.pdf",
                MusicPath = @"..\..\Plateformes\Musics\NEC TurboGrafx-CD\Gate of Thunder - Intro Music.mp3"


            };

            //---

            _IPlatformGames[3] = new MvGame()
            {
                Title = "Lords of Thunder",

            };

            // ---

            _IPlatformGames[4] = new MvGame()
            {
                Title = "Toe Jam and Earl",

            };

            // ---

            _IPlatformGames[5] = new MvGame()
            {
                Title = "Flashback",

            };

            // ---

            _IPlatformGames[6] = new MvGame()
            {
                Title = "Sonic",

            };



        }
    }
}
