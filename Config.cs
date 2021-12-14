using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DiscordNitroSniper
{
    public class Config
    {
        // FIELDS
        // READONLY
        private string _userToken;
        private int _threadsNumber;
        private int _proxiesTimeoutMs;
        private bool _isConfigLoaded;

        // PROPERTIES
        // READONLY
        public string UserToken { get { return _userToken; } }
        public int ThreadsNumber { get { return _threadsNumber; } }
        public int ProxiesTimeoutMs { get { return _proxiesTimeoutMs; } }
        public bool IsConfigLoaded { get { return _isConfigLoaded; } }

        /// <summary>
        /// Empty private constructor in order to force use of custom async GetConfig "constructor"
        /// </summary>
        private Config()
        {
            // empty private constructor in order to force use of custom async GetConfig "constructor"
        }

        /// <summary>
        /// Constructor for config that gets back a parsed config and checked.
        /// </summary>
        /// <param name="configPath">the config file location</param>
        public async static Task<Config> GetConfig(string configPath)
        {
            // the new Config instance
            Config newConfig = new();
            // new empty dict to translate the json file to
            Dictionary<string, string> config = new();

            // checks if there is actually a config file and if it's formated as json
            try
            {
                string configText = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<Dictionary<string, string>>(configText);
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("The config.json file either doesn't exist or is not formated as a json file, please check it again or redownload it from the repo.");
                Environment.Exit(100);
            }


            // Check with discord api if it's valid and set user token from config
            newConfig._userToken = config["user_token"];
            HttpResponseMessage response;
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Add("Authorization", newConfig._userToken);
                response = await client.GetAsync("https://discord.com/api/v9/users/@me");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("User token is invalid, please check it again.");
                Environment.Exit(100);
            }

            // Check and set threads number from config
            if(!int.TryParse(config["threads_number"], out newConfig._threadsNumber))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Invalid threads_number value in config, it has to be a valid number, double your available cpu threads (not cores) for most efficiency" +
                    " (going over will not work or slow the application down)");
                Environment.Exit(100);
            }

            // Check and set proxies timeout ms from config
            if (!int.TryParse(config["proxies_timeout_ms"], out newConfig._proxiesTimeoutMs))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("You entered an invalid nubmer for proxies_timeout_ms in the config, it has to be a number and in range 50-10000");
                Environment.Exit(100);
            }
            else if (!(newConfig._proxiesTimeoutMs >= 50) && !(newConfig._proxiesTimeoutMs <= 10000))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("The proxies_timeout_ms has to be in range 50-10000, your number was outside the bounds.");
                Environment.Exit(100);
            }

            newConfig._isConfigLoaded = true; // indicates if the config is fully loaded, useful for extra checks in case there was an unreported exception

            return newConfig;
        }
    }
}