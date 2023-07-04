# MovieRecommenderAPI
Movie Recommender API

The Movie Recommender API is a RESTful web service that provides personalized movie recommendations based on user preferences.
It utilizes machine learning models to generate top movie recommendations, allows searching for movies by title, filtering movies by genre, and provides randomly suggested movies.
The API is built using .NET 6 and ASP.NET Core, and it includes integration with Swagger for easy API documentation. 

![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/MovieRecommenderAPI.png)


## API ROUTES
- `/api/movie/recommendations/{userId}` - Retrieves top movie recommendations for a specific user.
  ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.22.22.png)

- `/api/movie/id/{movieId}` - Retrieves details of a movie by its ID.
  ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.23.57.png)

- `/api/movie/genre/{genre}` - Retrieves movies based on a specific genre.
  ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.24.40.png)

- `/api/movie/genres` - Retrieves available genres in the movie database.
  ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.25.07.png)

- `/api/movie/search?query={query}` - Searches for movies based on a search query in the title.
  ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.25.41.png)

- `/api/movie/feeling-lucky` - Retrieves a list of randomly suggested movies.
  ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.26.07.png)


## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

### Installation

1. Clone the repository: `git clone https://github.com/KimFarida/MovieRecommenderAPI.git`
2. Navigate to the project directory: `cd MovieRecommenderAPI`
3. Build the project: `dotnet build`
4. Run the API: `dotnet run`

By default, the API runs on `http://localhost:7216`.

## Usage

You can interact with the API using a tool like [Postman](https://www.postman.com) or by making HTTP requests using a programming language of your choice.

### Example Requests

- Get movie recommendations for user 1:
`GET /api/Movie/recommendations/1`
    ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.42.47.png)

- Get details of a movie with ID 123:
  `GET /api/Movie/id/123`
  ![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.43.55.png)

- Get movies of the "Action" genre:
`GET api/Movie/genre/Action`
![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.44.48.png)

- Get available genres:
`GET /api/Movie/genres`
![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.46.27.png)
- Search for movies with the title "Star Wars":
`GET /api/Movie/search?query=Star Wars`
![Screenshot](https://github.com/KimFarida/MovieRecommenderAPI/blob/main/img/Screenshot%202023-06-29%20at%2006.47.47.png)

## License

This project is licensed under the [MIT License](LICENSE).
