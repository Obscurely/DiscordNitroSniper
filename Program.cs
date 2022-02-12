using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordNitroSniper
{
    class Program
    {
        static Config config;
        static ulong checkedCodes = 0;
        static int currentThreadNumber = 1;

        static async Task Main()
        {
            Console.ForegroundColor = System.ConsoleColor.Blue;
            Console.WriteLine(
            "███╗   ██╗██╗████████╗██████╗  ██████╗     ███████╗███╗   ██╗██╗██████╗ ███████╗██████╗ \n" +
            "████╗  ██║██║╚══██╔══╝██╔══██╗██╔═══██╗    ██╔════╝████╗  ██║██║██╔══██╗██╔════╝██╔══██╗ \n" +
            "██╔██╗ ██║██║   ██║   ██████╔╝██║   ██║    ███████╗██╔██╗ ██║██║██████╔╝█████╗  ██████╔╝ \n" +
            "██║╚██╗██║██║   ██║   ██╔══██╗██║   ██║    ╚════██║██║╚██╗██║██║██╔═══╝ ██╔══╝  ██╔══██╗ \n" +
            "██║ ╚████║██║   ██║   ██║  ██║╚██████╔╝    ███████║██║ ╚████║██║██║     ███████╗██║  ██║ \n" +
            "╚═╝  ╚═══╝╚═╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝     ╚══════╝╚═╝  ╚═══╝╚═╝╚═╝     ╚══════╝╚═╝  ╚═╝ "
            );

            Console.ForegroundColor = System.ConsoleColor.Cyan;
            Console.WriteLine("\nGenerates random possible nitro codes and checks them with the api through a proxy, if it works it gets activated on the account right away!\n");

            // Loads config
            config = await Config.GetConfig("config.json");
            // Checks if config loaded
            if (config.IsConfigLoaded)
            {
                PrintCurrentTime();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[+] Config loaded successfully!");
            }
            else
            {
                PrintCurrentTime();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Config failed to load, check your config and try again!");
                Environment.Exit(100);
            }

            // Downloads proxies to separate files (file amount is the threads number) for the sniper to use
            PrintCurrentTime();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[*] Started downloading proxies with {config.ProxiesTimeoutMs}ms timeout to {config.ThreadsNumber} files, the specified number of threads.");

            await ProxyManager.DownloadProxiesForAllFiles(config.ThreadsNumber, config.ProxiesTimeoutMs);

            PrintCurrentTime();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[+] Downloaded proxies with {config.ProxiesTimeoutMs}ms timeout to {config.ThreadsNumber} files, the specified number of threads.");

            // Starts loop checks nitro codes for the amount of specified threads.
            PrintCurrentTime();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[*] Starting loops to check for nitro codes and try activate on {config.ThreadsNumber} threads.");

            StartLoopForSpecifiedThreadsNumber(config.ThreadsNumber, config.ProxiesTimeoutMs);

            PrintCurrentTime();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[+] Started loops to check for nitro codes and try activate on {config.ThreadsNumber} threads.");

            // Tells user sniper fully intialized.
            PrintCurrentTime();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[+] Discord Nitro Sniper fully initialized! Press ctrl + c to stop!");

            // Prints checked codes status
            PrintCheckedCodesStatus();

            // Keeps proccess opened unless ctrl + c pressed
            while(true)
            {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Loop for generating and trying to activate nitro codes, used for activating withing a background thread.
        /// </summary>
        /// <param name="threadId">thread's id for this loop's instance</param>
        /// <param name="proxiesTimeoutMs">proxy's max timeout</param>
        static async Task LoopGenerateAndTryActivateNitroCode(int threadId, int proxiesTimeoutMs)
        {
            ProxyManager proxyManager = new(currentThreadNumber, proxiesTimeoutMs);
            currentThreadNumber++;
            DiscordNitro discordNitro = new(config.UserToken, proxiesTimeoutMs);
            discordNitro.ChangeHttpClientProxy(await proxyManager.PickProxy());

            string nitroCode = discordNitro.GenerateRandomNitroCode();
            while (true)
            {
                try
                {
                    if (await discordNitro.TryActivateNitroCode(nitroCode))
                    {
                        PrintCurrentTime();
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine($"[+] Successfully activated nitro code {nitroCode}! You now have nitro, enjoy :)");

                        Environment.Exit(0);
                    }
                    else
                    {
                        checkedCodes++;
                        // Prints checked codes status
                        PrintCheckedCodesStatus();

                        nitroCode = discordNitro.GenerateRandomNitroCode();
                    }
                }
                catch (Exception)
                {
                    discordNitro.ChangeHttpClientProxy(await proxyManager.PickProxy());
                }
            }
        }

        /// <summary>
        /// Starts loop for generating and checking nitro codes for the specified threads amount using a max specified proxy timeout
        /// </summary>
        /// <param name="threadsNumber">how many threads to start</param>
        /// <param name="proxiesTimeoutMs"></param>
        static void StartLoopForSpecifiedThreadsNumber(int threadsNumber, int proxiesTimeoutMs)
        {
            for (int i = 1; i <= threadsNumber; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(async (state) => await LoopGenerateAndTryActivateNitroCode(i, proxiesTimeoutMs)));
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Prints current system's time to the console in a nice way.
        /// </summary>
        static void PrintCurrentTime()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(DateTime.Now.ToString("HH:mm:ss") + " ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("- ");
            Console.ResetColor();
        }

        /// <summary>
        /// Prints the current status on checked nitro codes to the console in a nice way by replacing the previous console line.
        /// </summary>
        static void PrintCheckedCodesStatus()
        {
            // Clears console output and prints the status of the program execution.
            Console.Write("\r                                                                                  \r");
            PrintCurrentTime();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[*] Checked {checkedCodes} codes so far!");
        }
    }
}