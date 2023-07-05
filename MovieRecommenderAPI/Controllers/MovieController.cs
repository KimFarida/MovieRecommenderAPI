using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using MovieRecommenderAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MovieRecommenderAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class MovieController : Controller
    {
        private readonly ILogger<MovieController> _logger;
        private readonly MLContext _mlContext;
        private readonly PredictionEngine<MovieRating, MovieRatingPrediction> _predictionEngine;
        private readonly IEnumerable<Movie> _movieData;

        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
            _mlContext = new MLContext();

            // Load the model from the zipped file
            var modelPath = Path.Combine(AppContext.BaseDirectory, "Data", "MovieRecommenderModel.zip");
            ITransformer model = _mlContext.Model.Load(modelPath, out var modelSchema);

            // Create the prediction engine
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);

            // Load the movie data
            var movieDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "recommendation-movies.csv");
            _movieData = LoadMovieData(movieDataPath);
        }

        [HttpGet("recommendations/{userId}")]
        public IActionResult GetRecommendations(int userId, int topK = 5)
        {
            // Get recommendations for the specified user
            var recommendations = GetTopRecommendationsWithTitle(_mlContext, _predictionEngine, _movieData, userId, topK);

            return Ok(recommendations);
        }

        [HttpGet("id/{movieId}")]
        public IActionResult GetMovie(int movieId)
        {
            // Find the movie by its ID
            var movie = _movieData.FirstOrDefault(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // Other controller methods for additional endpoints, data manipulation, etc.



        [HttpGet("genre/{genre}")]
        public IActionResult GetMoviesByGenre(string genre, int count = 5)
        {
            // Filter movies based on the specified genre
            var movies = _movieData.Where(m => m.Genres.Contains(genre, StringComparison.OrdinalIgnoreCase))
                                  .Take(count);

            return Ok(movies);
        }

        [HttpGet("genres")]
        public IActionResult GetGenres()
        {
            // Get distinct genres from the movie data
            var genres = _movieData.SelectMany(m => m.Genres.Split('|')).Distinct();

            return Ok(genres);
        }

        [HttpGet("search")]
        public IActionResult SearchMoviesByTitle(string query, int count = 5)
        {
            // Filter movies based on the search query in the title
            var movies = _movieData.Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                                  .Take(count);

            return Ok(movies);
        }

        [HttpGet("feeling-lucky")]
        public IActionResult GetFeelingLucky()
        {
            var randomMovies = _movieData.OrderBy(x => Guid.NewGuid()).Take(10);

            var movieDetails = randomMovies.Select(movie => new
            {
                movie.Title,
                movie.Genres
            });

            return Ok(movieDetails);
        }



        private IEnumerable<Movie> LoadMovieData(string movieDataPath)
        {
            // Load the movie data from the CSV file
            var movieDataEnumerable = _mlContext.Data.LoadFromTextFile<Movie>(movieDataPath, hasHeader: true, separatorChar: ',')
                .GetColumn<string>("Title")
                .ToArray();

            // Load the genre data from the CSV file
            var genreDataEnumerable = _mlContext.Data.LoadFromTextFile<Movie>(movieDataPath, hasHeader: true, separatorChar: ',')
                .GetColumn<string>("Genres")
                .ToArray();

            var movieData = new List<Movie>();

            for (int i = 0; i < movieDataEnumerable.Length; i++)
            {
                var movieId = i + 1;
                var title = movieDataEnumerable[i];
                var genres = genreDataEnumerable[i]; // Get the genres for the current movie

                var movie = new Movie
                {
                    MovieId = movieId,
                    Title = title,
                    Genres = genres
                };

                movieData.Add(movie);
            }

            return movieData;
        }

        private IEnumerable<Movie> GetTopRecommendationsWithTitle(MLContext mlContext, PredictionEngine<MovieRating, MovieRatingPrediction> predictionEngine, IEnumerable<Movie> movieData, int userId, int topK)
        {
            var recommendations = new List<(float score, int movieId)>();

            foreach (var movie in movieData)
            {
                var input = new MovieRating
                {
                    userId = userId,
                    movieId = movie.MovieId
                };

                var prediction = predictionEngine.Predict(input);
                recommendations.Add((prediction.Score, movie.MovieId));
            }

            recommendations.Sort((x, y) => y.score.CompareTo(x.score));

            var topRecommendations = recommendations.GetRange(0, Math.Min(topK, recommendations.Count));

            var topMovies = new List<Movie>();

            foreach (var recommendation in topRecommendations)
            {
                var movie = movieData.FirstOrDefault(m => m.MovieId == recommendation.movieId);
                if (movie != null)
                {
                    topMovies.Add(movie);
                }
            }

            return topMovies;
        }


    }

}
