# Telegram Weather Bot
This is a Telegram bot that provides user with precise weather forecast for today, tomorrow and hourly in a human readable format.

## Available commands
| Command      | Description                       |
|--------------|--------------------------------|
| **/today**    | today's forecast |
| **/tomorrow** | tomorrow's forecast |
| **/hourly**   | hourly forecast for today |

## Details
- **Written in:** C#  
- **Platform:** .NET 9  
- **Deployed on:** [Render](https://render.com/)  

This project is implemented using Domain Driven Design (DDD). Application is divided into several layers, and all dependencies are directed in a single direction.
That makes possible to reuse code. The abstraction system makes it possible to change the external API for retreiving a forecast, by changing the configuration.

## Launch
- Clone repository.
- Notice that you can launch only the ConsoleUI assembly locally: ```cd ConsoleUI```
- Build the project: ```dotnet build```
- Run the project: ```dotnet run```
