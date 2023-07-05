using System;
using Microsoft.ML.Data;

namespace MovieRecommenderAPI
{
    public class Movie
    {
        [LoadColumn(0)]
        public int MovieId { get; set; }

        [LoadColumn(1)]
        public string? Title { get; set; }

        [LoadColumn(2)]
        public string? Genres { get; set; }
    }

}

