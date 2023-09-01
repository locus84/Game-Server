# Game-Server
This project is a full-fledged game server that includes an admin panel, rest api and a realtime sockets feature. It is built with ASP.NET Core 7 and follows a DDD Clean architecture design. For realtime and duplex communications, it uses SignalR. It supports both Cookie (for admin panel) and Jwt Bearer (for rest api and SignalR) authentication and authorization methods. The front-end of the admin panel is developed in Blazor and utilizes MudBlazor UI library.

To run the server project, simply open it in Rider, Visual Studio or VS Code and click Run.

The Unity project needs BestHttp asset to run.
https://assetstore.unity.com/packages/tools/network/best-http-2-155981

Features:
- Match-Making, Chat, Realtime Game Hubs with SignalR
- Admin Panel for controlling any data with Blazor and MudBlazor
- DDD and Clean architecture using https://github.com/ardalis/CleanArchitecture
- Complete Unity Game and Client Sample using BestHttp Unity Asset
