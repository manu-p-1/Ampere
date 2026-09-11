<p align="center">
  <a href="https://github.com/manu-p-1/Ampere/" target="_blank">
    <img src="https://github.com/manu-p-1/Ampere/blob/main/assets/AmpereLogo.png" alt="Ampere Logo">
  </a>
  <br>
</p>
<p align="center">
<a href="https://github.com/manu-p-1/Ampere/releases" alt="License">
      <img src="https://img.shields.io/nuget/dt/Ampere"/></a>
    
   <a href="https://github.com/manu-p-1/Ampere/pulse" alt="Activity">
      <img src="https://img.shields.io/github/commit-activity/m/manu-p-1/Ampere?color=%20%230099ff"/></a>
        
   <a href="https://github.com/manu-p-1/Ampere/issues" alt="Open Issues">
      <img src="https://img.shields.io/github/issues/manu-p-1/Ampere"/></a>
  
   <a href="https://github.com/manu-p-1/Ampere/actions/workflows/codeql-analysis.yml" alt="CodeQL">
      <img src="https://github.com/manu-p-1/Ampere/actions/workflows/codeql-analysis.yml/badge.svg"/></a>
        
   <a href="#" alt="Repo Size">
      <img src="https://img.shields.io/github/repo-size/manu-p-1/Ampere?label=size&color=informational"/></a>
        
   <a href="https://github.com/manu-p-1/Ampere/blob/main/LICENSE" alt="License">
      <img src="https://img.shields.io/github/license/manu-p-1/Ampere?color=informational"/></a>
</p>

> ## Archived
> After several years, I finally decided to archive Ampere. It began as a college pet project and I was glad to keep maintaining it afterwards, but the time has come to focus my efforts on [PowerPlug](https://github.com/manu-p-1/PowerPlug) rather than spread them across several projects.
>
> Ampere will remain available as a read-only reference, but it will not receive new features, releases, pull-request reviews, or security updates. There are also many excellent .NET libraries covering much of the same ground, including [MoreLINQ](https://github.com/morelinq/MoreLINQ) for enumerable extensions, [Math.NET Numerics](https://numerics.mathdotnet.com/) for mathematics and statistics, and the [Community Toolkit](https://github.com/CommunityToolkit/dotnet) libraries for broadly useful .NET helpers. It was fun while it lasted...a new chapter opens.

## Introduction
Ampere is a robust .NET utility library written in C#. It's main purpose is to make development more productive and easier. The Ampere docs can be found at [Ampere Docs](https://manu-p-1.github.io/Ampere/>).
## Execution
### Add the dependency to your project with NuGet
You can install the latest release with NuGet by clicking `Managing NuGet Packages...` under your Visual Studio project name. Browse for the project by typing `Ampere`, view the information, and choose the version you would like to install.

### Install from GitHub Packages
The latest NuGet release can be found under the [GitHub Releases Page](https://github.com/manu-p-1/Ampere/packages/). You can use the instructions to install from the command line. 

## Contributing
Ampere is archived and does not accept contributions. You are welcome to fork it under the terms of the GPLv3 license.

### Building Ampere
Prerequisites:
- Visual Studio 2022 or Later OR VSCode
- .NET 8 or .NET 10 SDK

The project multi-targets `net8.0` and `net10.0`. The project can be built using `dotnet build` and the output will display the `AssemblyPath`.

### Documentation
There are two components to Ampere documentation:

1. Assembly Documentation
3. Miscellaneous Documentation (README.md, Wiki's, Discussions, etc...)

All Ampere methods are documented using .NET XML documentation. This is compiled using [DocFX](https://dotnet.github.io/docfx/) with the `docfx.json` file under the [DocFX folder](https://github.com/manu-p-1/Ampere/tree/master/DocFx). DocFX creates static
HTML pages which are used by [Ampere Docs](https://manu-p-1.github.io/Ampere/>). Lastly, as a fluid repository, many files change and constantly need to be documented and updated.

## Roadmap
There is no further roadmap. See [CHANGELOG.md](CHANGELOG.md) for the project's release history.

## State
Ampere is an archived project provided as-is. Existing issues and documentation remain available for reference, but new issues and pull requests will not be reviewed.

## Licensing
Ampere is licensed under the [**GNU General Public License v3.0**](https://www.gnu.org/licenses/gpl-3.0.en.html). The GNU General Public License is a free, copyleft license for software and other kinds of works.

