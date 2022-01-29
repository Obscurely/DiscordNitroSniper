using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiscordNitroSniper
{
    /// <summary>
    /// Simple, fast proxy manager manipulating proxyscrape's free api.
    /// </summary>
    public class ProxyManager
    {
        // FIELDS
        // Readonly
        private readonly string _proxiesUrl = "https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=$TIMEOUT&country=all&ssl=all&anonymity=elite&simplified=true";
        private readonly string _osFileSep;
        private readonly string _proxiesFilePath = "proxies";
        private readonly HttpClient _client = new();
        private readonly Random _rand = new(Guid.NewGuid().GetHashCode());

        // PROPERTIES
        // Readonly
        private string ProxiesUrl { get { return _proxiesUrl; } }
        private string OsFileSep { get { return _osFileSep; } }
        private string ProxiesFilePath { get { return _proxiesFilePath; } }
        private HttpClient Client { get { return _client; } }
        private Random Rand { get { return _rand; } }

        // CONSTRUCTORS
        /// <summary>
        /// Initializes a proxy manager object with a specified id and a timeout.
        /// </summary>
        /// <param name="proxyFileId">The id of this proxy manager object, has to be unique.</param>
        /// <param name="proxyTimeoutMs">A timeout int value between 50ms and 10000ms, defaults to 3000 if not specified.</param>
        public ProxyManager(int id, int proxyTimeoutMs)
        {
            // gets file separator, \ for windows and / for everything else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _osFileSep = "\\";
            }
            else
            {
                _osFileSep = "/";
            }

            // sets proxies file location should look like for example: /home/user/DiscordNitroSniper/proxies/list0.proxies
            _proxiesFilePath = Directory.GetCurrentDirectory() + OsFileSep + "Proxies" + OsFileSep + "list" + id + ".proxies";

            // checks if given timeout is valid if so sets it if not sets 3000
            if (proxyTimeoutMs >= 50 && proxyTimeoutMs <= 10000)
            {
                _proxiesUrl = _proxiesUrl.Replace("$TIMEOUT", proxyTimeoutMs.ToString());
            }
            else
            {
                _proxiesUrl = _proxiesUrl.Replace("$TIMEOUT", "3000");
            }
        }

        // METHODS
        /// <summary>
        /// Downloads proxies to the file coresponding to the id of this instance of proxymanager.
        /// </summary>
        public async Task DownloadProxies()
        {
            // keeps track if proxies are downloaded
            bool areProxiesDownloaded = false;

            // tries downloading proxies until they are downloaded successfully
            // it can happend that if too many threads run out proxies and try to download new ones at the same time the proxyscrape's api will timeout us
            // so we wait a random amount seconds between 3 and 8 seconds
            while (!areProxiesDownloaded)
            {
                try
                {
                    using (Stream stream = await Client.GetStreamAsync(ProxiesUrl))
                    {
                        using (FileStream fileStream = new(ProxiesFilePath, FileMode.Create))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                    areProxiesDownloaded = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(Rand.Next(3, 9) * 1000); // it's 9 and not 8 because it's an exclusive upper bound not inclusive
                }
            }


        }

        /// <summary>
        /// Downloads proxies and adds them to file for the amount of threads specified, downloads proxies once and writes the same download to every file.
        /// Each created has it's own proxies file, that's why we need to download for each of them.
        /// </summary>
        /// <param name="threads">how many threads to download for</param>
        /// <param name="proxyTimeoutMs">proxy timeout in ms</param>
        /// <returns></returns>
        public static async Task DownloadProxiesForAllFiles(int threads, int proxyTimeoutMs)
        {
            // gets file separator, \ for windows and / for everything else
            string osFileSep;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osFileSep = "\\";
            }
            else
            {
                osFileSep = "/";
            }

            // Checks if Proxies dir is present, if not it creates one.
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory());
            if (Array.IndexOf(dirs, "Proxies") == -1)
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + osFileSep + "Proxies");
            }

            // Downloads proxies to needed files.
            string proxyFilePath = Directory.GetCurrentDirectory() + osFileSep + "Proxies" + osFileSep;
            string proxiesUrl = $"https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout={proxyTimeoutMs}&country=all&ssl=all&anonymity=elite&simplified=true";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(proxiesUrl);
                for (int i = 1; i <= threads; i++)
                {
                    using (FileStream fileStream = new(proxyFilePath + $"list{i}.proxies", FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
            }
        }

        /// <summary>
        /// Picks a random proxy from the file linked to this instance of proxymanager and removes it from the file, if there aren't anymore proxies in the file or there isn't a proxies
        /// file it downloads new ones.
        /// </summary>
        /// <returns>Random picked proxy from the list of this proxymanager instance.</returns>
        public async Task<string> PickProxy()
        {
            // Checks if there isn't a proxies file in dir or the proxies file is empty and if so downloads new proxies.
            if (!File.Exists(ProxiesFilePath))
            {
                await DownloadProxies();
            }

            // Checks if the file has proxies in it, if not it downloads them
            string[] proxiesInFile = await File.ReadAllLinesAsync(ProxiesFilePath);
            if (proxiesInFile.Length <= 0)
            {
                await DownloadProxies();
                proxiesInFile = await File.ReadAllLinesAsync(ProxiesFilePath);
            }

            // Give back a proxy and remove it from the file
            string pickedProxy = proxiesInFile[Rand.Next(proxiesInFile.Length)];
            List<string> newProxiesList = new();
            newProxiesList.AddRange(proxiesInFile);
            newProxiesList.Remove(pickedProxy);

            // Write new proxy list to file
            await File.WriteAllLinesAsync(ProxiesFilePath, newProxiesList);

            return pickedProxy;
        }
    }
}
