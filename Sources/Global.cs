using SPR.Common;
using SPR.Containers;
using SPR.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR
{
    /*
     * Constantes globales
     */
    internal class Global
    {
        /// <summary>
        /// Lien vers le CORE
        /// </summary>
        /// <remarks>
        /// Apparemment l'application se lance par l'exe du Core
        /// </remarks>
        public static string LaunchBoxPath { get; set; }

        /// <summary>
        /// Racine du dossier
        /// </summary>
        public static string LaunchBoxRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Determinate Launchbox root folder
        /// </summary>
        /// <remarks>
        /// Puisque l'application est lancée par le core qui est un sub, la racine sera un cran en arrière
        /// </remarks>
        public static void SetLaunchBoxRoot()
        {
            string[] tmp = Global.LaunchBoxPath.Split(@"\");
            LaunchBoxRoot = String.Join(@"\", tmp, 0, tmp.Length - 2);
        }

        #region Plugin Location
        public static string PluginLocation { get; private set; }

        /// <summary>
        /// Stocke l'emplacement de la dll
        /// </summary>
        internal static void SetPluginLocation() => PluginLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        #endregion


        #region
        public static Configuration Config { get; internal set; }

        public static void InitializeConfig()
        {
            string confLocat = Path.Combine(PluginLocation, Properties.Settings.Default.ConfigFile);
            if (!File.Exists(confLocat))
            {
                Configuration cfg = new Configuration();
                cfg.SerializeMe(confLocat);
            }

            Config = Configuration.DeserializeMe(confLocat);
        }
        #endregion



        /// <summary>
        /// Mode debug ou pas
        /// </summary>
        public static bool DebugMode { get; internal set; }

        public static bool Forbidden4Paths(char c)
        {
            switch (c)
            {
                case '|':
                case '"':
                case ':':
                case '<':
                case '>':
                case '*':
                case '?':
                case '\\':
                case '/':
                    return false;

            }

            return true;

        }

        /// <summary>
        /// Analyse une chaine et la renvoie selon le format voulu
        /// </summary>
        /// <param name="text"></param>
        /// <param name="formatModel"></param>
        /// <returns></returns>
        internal static string FormatString(string text, StringFormat formatModel)
        {
            string tmp = null;

            bool result = false;
            foreach (char c in text)
            {
                switch (formatModel)
                {
                    case StringFormat.Folder:
                        result = Global.Forbidden4Paths(c);
                        break;
                }

                if (result)
                    tmp += c;
            }

            return tmp;
        }

        /// <summary>
        /// Vérifie une chaine de caractères via des préréglages de Regex
        /// </summary>
        /// <param name="text"></param>
        /// <param name="formatModel"></param>
        /// <returns></returns>
        internal static bool VerifString(string text, StringFormat formatModel)
        {
            Regex r = null;
            switch (formatModel)
            {
                case StringFormat.Folder:
                    r = new Regex("[*?\\\\:/|<>]");
                    break;
                case StringFormat.Path:
                    r = new Regex("[*?|<>:/]");
                    break;
            }


            return r.IsMatch(text);
        }



        public static Dictionary<MediaType, C_Paths> Make_DicPlatformPaths(string applicationPath, IPlatformFolder[] platformFolders)
        {
            Dictionary<MediaType, C_Paths> result = new Dictionary<MediaType, C_Paths>();
            //string nameApp = nameof(PathType.ApplicationPath);

            string tmpAppPath = applicationPath;
            if (string.IsNullOrEmpty(tmpAppPath))
                tmpAppPath = "Games";

            result.Add(MediaType.Application, new C_Paths(tmpAppPath, nameof(PathType.ApplicationPath)));


            foreach (IPlatformFolder pPath in platformFolders)
            {
                Enum.TryParse(pPath.MediaType, out MediaType myStatus);
                if (myStatus == MediaType.None)
                    continue;

                result.Add(myStatus, new C_Paths(pPath.FolderPath, pPath.MediaType));
            }


            return result;
        }

    }
}
