# Telegram Weather Bot
This is a Telegram bot that gives user precisely enough weather forecast today, tomorrow and hourly in a human readable format.

## Available commands
| Command      | Description                       |
|--------------|--------------------------------|
| **/today**    | today forecast |
| **/tomorrow** | tomorrow forecast |
| **/hourly**   | hourly today forecast |

## Details
- **Written on:** C#  
- **Platform:** .NET 9  
- **Deployed on:** [Render](https://render.com/)  

This project is implemented using Domain Driven Design (DDD). Application is devided by several layers, all the dependencies are directed in one direction.
That makes possible to reuse code. An abstraction system makes possible to change an outer api for retreiving a forecast, just changing a configuration.

## Launch
- Clone repository.
- Notice that you can launch only the ConsoleUI assembly locally: ```cd ConsoleUI```
- ```dotnet build```
- ```dotnet run```
