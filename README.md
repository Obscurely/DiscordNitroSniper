<div id="top"></div>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/Obscurely/DiscordNitroSniper">
    <img src="images/logo.png" alt="Logo" width="128" height="100">
  </a>

  <h3 align="center">Discord Nitro Sniper</h3>

  <p align="center">
    Fast multi-threaded discord nitro sniper written in C# with dotnet 6 (as fast as GO or even faster at requests) using proxies from proxy scrape's API. Auto activates code on account after found.
    <br />
    <a href="https://github.com/Obscurely/DiscordNitroSniper"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Obscurely/DiscordNitroSniper">View Demo</a>
    ·
    <a href="https://github.com/Obscurely/DiscordNitroSniper/issues">Report Bug</a>
    ·
    <a href="https://github.com/Obscurely/DiscordNitroSniper/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>


## About The Project
Fast multi-threaded discord nitro sniper written in C# with dotnet 6 (as fast as GO or even faster at requests) using proxies from proxy scrape's API. Auto activates code on account after found. Rewrite of my old [Discord-Nitro-Sniper Repo](https://github.com/Obscurely/Discord-Nitro-Sniper/), old one was old and bad quite bad written since I was at start with API's client requests and c# in general.



### Built with
Only the stock libraries.
* [C# 10.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10)
* [.NET 6](https://devblogs.microsoft.com/dotnet/announcing-net-6/?WT.mc_id=dotnet-35129-website)



## Getting Started

### Running The Program
1. Go to [Releases](https://github.com/Obscurely/DiscordNitroSniper/releases) tab and download latest stable binary for your platform, if your platform is not present refer to [Compilation](#compilation) and compile it for your platform.
2. Extract archive to a folder, wherever you want.
3. Open config.json with your favourite file editor of choice and edit the file to fit your computer and use.
```
{
  "user_token": "user_token",
  "threads_number": "threads_amount",
  "proxies_timeout_ms": "3000"
}
```
Replace user_token value (right side) with your discord user token, refer to [how to get discord user token](#how-to-get-user-token) if you don't know how.<div>
Theads
5. 
6. Open a new terminal/cmd window.
7. 

### Compilation
Any of [.NET 6 available platforms](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md) (distributions like Arch aren't specified, but can still be run if you compile them for linux).

## General Info
Discord nitro sniper. 

## Why C# and not GO for Mass Http Requesting
Written in C# instead of GO like people usually do for mass http requesting because from my testing dotnet 6 compared to latest GO version, by making a lot of requests, C# even proved faster than GO with 1ms on average, that's insignificant, but the point is it doesn't matter so I chose to go with C# since I like it more and is more optimized IMO for multi threading.

## Technologies
Project is created with:
* C# 10.0
* .NET 6.0

## Program Dependencies
Nothing extra needs to be installed. Only uses default .NET C# libraries.

## Compilation

## How to Use

## How it Works

## Screenshots

## Other Notes

More to come! :)

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/Obscurely/DiscordNitroSniper.svg?style=for-the-badge
[contributors-url]: https://github.com/Obscurely/DiscordNitroSniper/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Obscurely/DiscordNitroSniper.svg?style=for-the-badge
[forks-url]: https://github.com/Obscurely/DiscordNitroSniper/network/members
[stars-shield]: https://img.shields.io/github/stars/Obscurely/DiscordNitroSniper.svg?style=for-the-badge
[stars-url]: https://github.com/Obscurely/DiscordNitroSniper/stargazers
[issues-shield]: https://img.shields.io/github/issues/Obscurely/DiscordNitroSniper.svg?style=for-the-badge
[issues-url]: https://github.com/Obscurely/DiscordNitroSniper/issues
[license-shield]: https://img.shields.io/github/license/Obscurely/DiscordNitroSniper.svg?style=for-the-badge
[license-url]: https://github.com/Obscurely/DiscordNitroSniper/blob/master/LICENSE
[product-screenshot]: images/screenshot.png
