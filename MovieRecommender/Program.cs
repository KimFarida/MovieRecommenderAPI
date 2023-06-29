using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using MovieRecommender;

// Initialize MLContext
MLContext mlContext = new MLContext();

// Load training and test data
(IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);

// Build and train the recommendation model
ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);

// Evaluate the model's performance on test data
EvaluateModel(mlContext, testDataView, model);


// Use the model to make a single movie recommendation
UseModelForSinglePrediction(mlContext, model);

// Load the movie data
var movieDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-movies.csv");
var movieData = LoadMovieData(movieDataPath);

// Get recommendations for a specific user
int userId = 6;
var topRecommendations = GetTopRecommendationsWithTitle(mlContext, model, movieData, userId, 5);

// Display the top recommendations
Console.WriteLine($"Top 5 movie recommendations for user {userId}:");
foreach (var movie in topRecommendations)
{
    Console.WriteLine($"Movie ID: {movie.MovieId}, Title: {movie.Title}, Genres: {movie.Genres}");
}

// Save the model to a file
SaveModel(mlContext, trainingDataView.Schema, model);



(IDataView training, IDataView test) LoadData(MLContext mlContext)
{
    var trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-train.csv");
    var testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-test.csv");

    IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
    IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

    return (trainingDataView, testDataView);
}

ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
{
    var estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
        .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));

    var options = new MatrixFactorizationTrainer.Options
    {
        MatrixColumnIndexColumnName = "userIdEncoded",
        MatrixRowIndexColumnName = "movieIdEncoded",
        LabelColumnName = "Label",
        NumberOfIterations = 20,
        ApproximationRank = 100
    };

    var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

    Console.WriteLine("=============== Training the model ===============");
    ITransformer model = trainerEstimator.Fit(trainingDataView);

    return model;
}

void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
{
    Console.WriteLine("=============== Evaluating the model ===============");

    var prediction = model.Transform(testDataView);
    var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");

    Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
    Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
}

void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
{
    Console.WriteLine("=============== Making a prediction ===============");
    var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);

    var testInput = new MovieRating { userId = 6, movieId = 10 };

    var movieRatingPrediction = predictionEngine.Predict(testInput);

    if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
    {
        Console.WriteLine("Movie " + testInput.movieId + " is recommended for user " + testInput.userId);
    }
    else
    {
        Console.WriteLine("Movie " + testInput.movieId + " is not recommended for user " + testInput.userId);
    }
}

void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
{
    var modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "MovieRecommenderModel.zip");

    Console.WriteLine("=============== Saving the model to a file ===============");
    mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
}

IEnumerable<Movie> LoadMovieData(string movieDataPath)
{
    var movieDataViewSchema = SchemaDefinition.Create(typeof(Movie));
    var movieDataView = mlContext.Data.LoadFromTextFile<Movie>(movieDataPath, hasHeader: true, separatorChar: ',');

    var movieDataEnumerable = mlContext.Data.CreateEnumerable<Movie>(movieDataView, false);
    return movieDataEnumerable.ToList();
}



IEnumerable<Movie> GetTopRecommendationsWithTitle(MLContext mlContext, ITransformer model, IEnumerable<Movie> movieData, int userId, int topK)
{
    var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);

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
