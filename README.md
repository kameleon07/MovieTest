# Movie API & Frontned

### Prerequisites

- .NET 8.0
- A web browser

### Running the Application

1. Clone or download this repository
2. Open a terminal in the project (\GitHub\MovieApi\MovieApi) directory
3. Run the .NET project:
   ```
   dotnet run
   ```
4. The API will start on a local port 
5. Open the HTML file (movie-frontend.html) in your browser to interact with the API
6. You can now search, filter, and explore movies locally

### Deployments and Pipelines

For azure or any other pipelines deployment, we can add a 'azure-pipelines-yaml' file that would install the .NET sdk
build the solution, publish the api etc.

Could also add a dockerfile to containeraize my app (not done much of that before :)

### Notes*

Looking at the https://www.kaggle.com/datasets/disham993/9000-movies-dataset database, the data provided contains no actor entries, 
so filter by actor functionality was replaced with the language filter.