# 🧰 .NET Community Toolkit

.NET Community Toolkit is a collection of helpers and APIs that work for all .NET developers and are agnostic of any specific UI platform. The toolkit is maintained and published by Microsoft, and part of the .NET Foundation.

## 👀 What does this repo contain?

This repository contains several .NET libraries (originally developed as part of the [Windows Community Toolkit](https://github.com/CommunityToolkit/WindowsCommunityToolkit)) that can be used both by application developers (regardless on the specific UI framework in use, they work everywhere!) and library authors. These libraries are also being used internally at Microsoft to power many of our first party apps (such as the new Microsoft Store) and constantly improved by listening to feedbacks from other teams, external partners and other developers from the community. Here's a quick breakdown of the various components you'll find in this repository:

Package | Latest stable | Latest Preview | Description
---------|---------------|---------------|------------
[`CommunityToolkit.Common`](https://learn.microsoft.com/dotnet/api/?term=communitytoolkit.common) | [![CommunityToolkit.Common](https://img.shields.io/nuget/v/CommunityToolkit.Common)](https://nuget.org/packages/CommunityToolkit.Common/) | [![CommunityToolkit.Common](https://img.shields.io/nuget/vpre/CommunityToolkit.Common)](https://nuget.org/packages/CommunityToolkit.Common/absoluteLatest) | A set of helper APIs shared with other [`CommunityToolkit`](https://learn.microsoft.com/windows/communitytoolkit) libraries.
[`CommunityToolkit.Diagnostics`](https://learn.microsoft.com/windows/communitytoolkit/diagnostics/introduction) | [![CommunityToolkit.Diagnostics](https://img.shields.io/nuget/v/CommunityToolkit.Diagnostics)](https://nuget.org/packages/CommunityToolkit.Diagnostics/) | [![CommunityToolkit.Diagnostics](https://img.shields.io/nuget/vpre/CommunityToolkit.Diagnostics)](https://nuget.org/packages/CommunityToolkit.Diagnostics/absoluteLatest) | A set of helper APIs (specifically, [`Guard`](https://learn.microsoft.com/windows/communitytoolkit/developer-tools/guard) and [`ThrowHelper`](https://learn.microsoft.com/windows/communitytoolkit/developer-tools/throwhelper)) that can be used for cleaner, more efficient and less error-prone argument validation and error checking.
[`CommunityToolkit.HighPerformance`](https://learn.microsoft.com/windows/communitytoolkit/high-performance/introduction) | [![CommunityToolkit.HighPerformance](https://img.shields.io/nuget/v/CommunityToolkit.HighPerformance)](https://nuget.org/packages/CommunityToolkit.HighPerformance/) | [![CommunityToolkit.HighPerformance](https://img.shields.io/nuget/vpre/CommunityToolkit.HighPerformance)](https://nuget.org/packages/CommunityToolkit.HighPerformance/absoluteLatest) | A collection of helpers for working in high-performance scenarios. It includes APIs such as [pooled buffer helpers](https://learn.microsoft.com/windows/communitytoolkit/high-performance/memoryowner), a fast [string pool](https://learn.microsoft.com/windows/communitytoolkit/high-performance/stringpool) type, a 2D variant of `Memory<T>` and `Span<T>` ([`Memory2D<T>`](https://learn.microsoft.com/windows/communitytoolkit/high-performance/memory2d) and [`Span2D<T>`](https://learn.microsoft.com/windows/communitytoolkit/high-performance/span2d)) also supporting discontiguous regions, helpers for bit shift operations (such as [`BitHelper`](https://learn.microsoft.com/windows/communitytoolkit/high-performance/span2d), also used in [Paint.NET](https://getpaint.net)), and more.
[`CommunityToolkit.Mvvm` (aka MVVM Toolkit)](https://aka.ms/mvvmtoolkit/docs) | [![CommunityToolkit.Mvvm](https://img.shields.io/nuget/v/CommunityToolkit.Mvvm)](https://nuget.org/packages/CommunityToolkit.Mvvm/) | [![CommunityToolkit.Mvvm](https://img.shields.io/nuget/vpre/CommunityToolkit.Mvvm)](https://nuget.org/packages/CommunityToolkit.Mvvm/absoluteLatest) | A fast, modular, platform-agnostic MVVM library, which is the official successor of `MvvmLight`. It's used extensively in the Microsoft Store and other first party apps. [The sample app repository is here](https://aka.ms/mvvmtoolkit/samples).

## 🙌 Getting Started

Please read the [Getting Started with the .NET Community Toolkit](https://learn.microsoft.com/windows/communitytoolkit/getting-started) page for more detailed information.

## 📃 Documentation

All documentation for the toolkit is hosted on [Microsoft Docs](https://learn.microsoft.com/dotnet/communitytoolkit/).

All API documentation can be found at the [.NET API Browser](https://learn.microsoft.com/dotnet/api/?view=win-comm-toolkit-dotnet-stable).

## 🚀 Contribution

Do you want to contribute?

Check out our [.NET Community Toolkit Wiki](https://aka.ms/wct/wiki) page to learn more about contribution and guidelines!

## 📦 NuGet Packages

NuGet is a standard package manager for .NET applications which is built into Visual Studio. When you open solution in Visual Studio, choose the *Tools* menu > *NuGet Package Manager* > *Manage NuGet packages for solution…* Enter one of the package names mentioned in [.NET Community Toolkit NuGet Packages](https://learn.microsoft.com/windows/communitytoolkit/nuget-packages) table to search for it online.

## 🌍 Roadmap

Read what we [plan for next iterations](https://github.com/CommunityToolkit/dotnet/milestones), and feel free to ask questions.

Check out our [Preview Packages Wiki Page](https://github.com/CommunityToolkit/dotnet/wiki/Preview-Packages) to learn more about updating your NuGet sources in Visual Studio, then you can also get pre-release packages of upcoming versions to try.

## 📄 Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](CODE_OF_CONDUCT.md).

## 🏢 .NET Foundation

This project is supported by the [.NET Foundation](http://dotnetfoundation.org).

## 🏆 Contributors

[![Toolkit Contributors](https://contrib.rocks/image?repo=CommunityToolkit/dotnet)](https://github.com/CommunityToolkit/dotnet/graphs/contributors)

Made with [contrib.rocks](https://contrib.rocks).
