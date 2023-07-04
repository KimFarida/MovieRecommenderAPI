# MovieRecommenderAPI
Movie Recommender API

The Movie Recommender API is a RESTful web service that provides personalized movie recommendations based on user preferences.
It utilizes machine learning models to generate top movie recommendations, allows searching for movies by title, filtering movies by genre, and provides randomly suggested movies.
The API is built using .NET 6 and ASP.NET Core, and it includes integration with Swagger for easy API documentation. 

- `/api/movie/recommendations/{userId}` - Retrieves top movie recommendations for a specific user.
- `/api/movie/id/{movieId}` - Retrieves details of a movie by its ID.
- `/api/movie/genre/{genre}` - Retrieves movies based on a specific genre.
- `/api/movie/genres` - Retrieves available genres in the movie database.
- `/api/movie/search?query={query}` - Searches for movies based on a search query in the title.
- `/api/movie/feeling-lucky` - Retrieves a list of randomly suggested movies.

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

### Installation

1. Clone the repository: `git clone https://github.com/your-username/movie-recommender-api.git`
2. Navigate to the project directory: `cd movie-recommender-api`
3. Build the project: `dotnet build`
4. Run the API: `dotnet run`

By default, the API runs on `http://localhost:7216`.

## Usage

You can interact with the API using a tool like [Postman](https://www.postman.com) or by making HTTP requests using a programming language of your choice.

### Example Requests

- Get movie recommendations for user 1:
`GET /api/Movie/recommendations/1`

- Get details of a movie with ID 123:
  `GET /api/Movie/id/123`

- Get movies of the "Action" genre:
`GET api/Movie/genre/Action`

- Get available genres:
`GET /api/Movie/genres`

- Search for movies with the title "Star Wars":
`GET /api/Movie/search?query=Star Wars`


## License

This project is licensed under the [MIT License](LICENSE).
