using System;
using System.Drawing;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Containers
{
    class MvAdditionnalApplication : IAdditionalApplication
    {
        public string Id { get; set; }

        public string GameId { get; set; }

        public string Name { get; set; }
        public string ApplicationPath { get; set; }


        public int PlayCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoRunAfter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoRunBefore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string CommandLine { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool UseDosBox { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool UseEmulator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool WaitForExit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime? ReleaseDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Developer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Publisher { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Region { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime? LastPlayed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? Disc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string EmulatorId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SideA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SideB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Priority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? Installed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Image GetIconImage(IGame game)
        {
            throw new NotImplementedException();
        }

        public string Launch(IGame game)
        {
            throw new NotImplementedException();
        }
    }
}
