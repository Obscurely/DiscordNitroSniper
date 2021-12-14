using System;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordNitroSniper
{
    /// <summary>
    /// An object for generating random nitro codes, checking them and if they are valid activating them on the user's account via their login token.
    /// </summary>
    public class DiscordNitro
    {
        // FIELDS
        // CONST
        private const string _possibleNitroCodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string _activateGiftApiUrl = "https://discord.com/api/v9/entitlements/gift-codes/$GIFTCODE/redeem";
        private const string _checkGiftApiUrl = "https://discord.com/api/v9/entitlements/gift-codes/$GIFTCODE?with_application=false&with_subscription_plan=true";
        // READONLY
        private readonly string _userToken;
        private readonly Random _rand = new(Guid.NewGuid().GetHashCode());
        // REGULAR
        private HttpClient _clientWithAuthentication;
        private HttpClient _clientWithProxy;
        private HttpClientHandler _clientHandler;
        private readonly WebProxy _proxy = new();

        // PROPERTIES
        // READONLY
        private string UserToken { get { return _userToken; } }
        private string PossibleNitroCodeChars { get { return _possibleNitroCodeChars; } }
        private Random Rand { get { return _rand; } }
        private string ActivateGiftApiUrl { get { return _activateGiftApiUrl; }}
        private string CheckGiftApiUrl { get { return _checkGiftApiUrl; } }
        // REGULAR
        private HttpClient ClientWithAuthentication { get { return _clientWithAuthentication; } set { _clientWithAuthentication = value; } }
        private HttpClient ClientWithProxy { get { return _clientWithProxy; } set { _clientWithProxy = value; } }
        private HttpClientHandler ClientHandler { get { return _clientHandler; } set { _clientHandler = value; } }
        private WebProxy Proxy { get { return _proxy; } }

        // CONSTRUCTORS
        /// <summary>
        /// Initializes DiscordNitro object, object to activate generated nitro codes with a proxy
        /// </summary>
        /// <param name="userToken">user authorization token</param>
        /// <param name="proxyTimeoutMs">the timeout for the proxy in order to apply to the httpclient too.</param>
        public DiscordNitro(string userToken, int proxyTimeoutMs)
        {
            // sets the user's login token as a global var for this object.
            _userToken = userToken;

            // HttpClient with set token but without proxy for activating found nitro code straight away.
            ClientWithAuthentication = new();
            ClientWithAuthentication.DefaultRequestHeaders.Add("Authorization", UserToken);

            // Client with proxy for checking the random generated nitro codes.
            _clientHandler = new() { AllowAutoRedirect = true, AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip, Proxy = _proxy };
            ClientWithProxy = new(ClientHandler);
            ClientWithProxy.Timeout = TimeSpan.FromMilliseconds(proxyTimeoutMs);
        }

        // METHODS
        /// <summary>
        /// Generates a random nitro code to be checked.
        /// </summary>
        /// <returns>String with the random nitro code.</returns>
        public string GenerateRandomNitroCode()
        {
            // using stringbuilder because of string performance issue in loops.
            StringBuilder nitroCode = new();
            for (int i = 0; i < 24; i++)
            {
                nitroCode.Append(PossibleNitroCodeChars[Rand.Next(62)]);
            }

            return nitroCode.ToString();
        }

        /// <summary>
        /// Changes the proxy used by the HttpClient instance.
        /// </summary>
        /// <param name="proxy">the proxy to be used.</param>
        public void ChangeHttpClientProxy(string proxy)
        {
            ClientWithProxy.CancelPendingRequests();

            // If the proxy is well formated the current proxy gets set if not it skips changing it. (some proxies can have a bad host or not be accepted by C#)
            if (Uri.IsWellFormedUriString($"http://{proxy}", UriKind.Absolute))
            {
                Proxy.Address = new Uri($"http://{proxy}");
            }

        }

        /// <summary>
        /// Tries to activate the given nitro code. If the code was valid and activated successfully it returns true, if not false
        /// </summary>
        /// <param name="nitroCode">nitro code to check and try activate</param>
        /// <returns>True if code checked and activated successfully, otherwise false.</returns>
        public async Task<bool> TryActivateNitroCode(string nitroCode)
        {
            using(HttpResponseMessage responseCheck = await ClientWithProxy.GetAsync(CheckGiftApiUrl.Replace("$GIFTCODE", nitroCode)))
            {
                if (responseCheck.StatusCode == HttpStatusCode.OK)
                {
                    using(HttpResponseMessage response = await ClientWithAuthentication.PostAsync(ActivateGiftApiUrl.Replace("$GIFTCODE", nitroCode), null))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return false;
                        }
                        else
                        {
                            throw new Exception("Rate limited!");
                        }
                    }
                }
                else if (responseCheck.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                else
                {
                    throw new Exception("Rate limited!");
                }
            }
        }
    }
}