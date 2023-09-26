# Game-Server
This project is a game server that provides an admin panel, rest api and a realtime sockets feature. It is developed with ASP.NET Core 7 and adopts a DDD Clean architecture design. It uses SignalR for realtime and duplex communications. It supports both Cookie (for admin panel) and Jwt Bearer (for rest api and SignalR) authentication and authorization methods. The admin panel is built with Blazor and MudBlazor UI library.

To run the server project, you need to open it in Rider, Visual Studio or VS Code and click Run.

Using the BestHttp asset, you can access all the web APIs and SignalR hubs that this server provides in the Unity3D engine.

Features:
- Match-Making, Chat, Realtime Game Hubs with SignalR
- Admin Panel for managing any data with Blazor and MudBlazor
- DDD and Clean architecture using https://github.com/ardalis/CleanArchitecture
- Complete Unity Game and Client Sample using BestHttp Unity Asset

Refrences:
- https://github.com/MudBlazor/MudBlazor
- https://github.com/ardalis/CleanArchitecture/tree/main
- https://assetstore.unity.com/packages/tools/network/best-http-2-155981
