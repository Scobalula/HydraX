using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace HydraX.Library
{
    public class HydraUpdater
    {
        public class ApplicationRelease
        {
            [JsonProperty("tag_name")]
            public string Version { get; set; }
        }

        /// <summary>
        /// Checks for updates for our vicious beast
        /// </summary>
        public static bool CheckForUpdates(Version currentVersion)
        {
            try
            {
                // Initialize at 0
                Version latestVersion = new Version(0, 0, 0, 0);
                // Request update
                var webRequest = WebRequest.Create(@"https://api.github.com/repos/Scobalula/HydraX/releases") as HttpWebRequest;
                webRequest.UserAgent = "HydraX";
                // Load Input
                using (var reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    // Load versions
                    List<ApplicationRelease> applicationReleases = LoadGithubReleases(reader);
                    // Loop through results
                    foreach (var release in applicationReleases)
                    {
                        // Convert to version
                        var releaseVersion = new Version(release.Version);
                        // Check for it
                        if (releaseVersion > latestVersion)
                            // Set Values
                            latestVersion = releaseVersion;
                    }
                }
                // Check latest against us
                return latestVersion > currentVersion;
            }
            catch
            {
                // fk
                return false;
            }
        }

        /// <summary>
        /// Returns a basic list of Github release data
        /// </summary>
        private static List<ApplicationRelease> LoadGithubReleases(StreamReader input)
        {
            return JsonConvert.DeserializeObject<List<ApplicationRelease>>(input.ReadToEnd());
        }
    }
}
