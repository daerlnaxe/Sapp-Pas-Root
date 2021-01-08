using SPR.Enums;

namespace SPR.Containers
{
    internal class AAppPath : C_PathsDouble
    {
        internal string Id { get; set; }

        // internal C_PathsCollec Paths { get; set; }

        public AAppPath(string id, string orelatlink) : base(orelatlink, nameof(PathType.AdditionalApplication))
        {
            Id = id;

            //Type = type;
            if (string.IsNullOrEmpty(orelatlink))
                return;

            base.FormatPath(orelatlink, "AdditionnalApplication");

            // new Paths = new C_PathsCollec(orelatlink, "AdditionnalApplication");
            /*Paths.OldRelatPath = orelatlink;
            Paths.OldHardPath = Path.GetFullPath(Path.Combine(Global.LaunchBoxPath, orelatlink));*/
            /*

            Destination_RLink = Languages.Lang.Waiting;
            Destination_HLink = Languages.Lang.Waiting;*/
        }



    }
}
