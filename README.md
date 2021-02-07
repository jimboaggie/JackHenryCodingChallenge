# Twitter Sample Streaming Example

## Points of Interest

* The application was written using Microsoft's Blazor technology. It utilizes Server Side rendering and WebApi contollers
* A deployed version of the application is at https://twitterstatstest.azurewebsites.net/
* The API Swagger documentation is published at https://twitterstatstest.azurewebsites.net/swagger
* The unit tests are in the TwitterStats.Tests
* A basic repository pattern was implemented in the TwitterStats.Services project. However, this exercise did not provide an opportunity to utilizes it.
* The Twitter library was based on Jamie Maguire's SocialOpinion C# libary.
* Due to time constraints there are a limited number of unit tests using xUnit and Moq.
* Also due to time constrants I was only able to get a few of the analytics coded. The Twitter access and return model is fairly large and complex. Given enough time one could easily add numerous other analytics.


### Prerequisites

- .NET Core SDK ([3.1.300](https://dotnet.microsoft.com/download/dotnet-core/3.1) or later)
- Visual Studio Code, **OR**
- Visual Studio 2019 16.6 or later

### Visual Studio

1. Open the solution file 'TwitterStats.sln'.
2. Ensure the `TwitterStats` project is set as the start up project.
3. You can set the Twitter Api Keys in the appsettings.Development.json file or you can add a user secrets file called TwitterSecrets
