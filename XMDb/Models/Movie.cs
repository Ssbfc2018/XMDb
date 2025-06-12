namespace XMDb.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Director { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public int Budget { get; set; }
        public TimeSpan? Runtime { get; set; }
        public double? AverageRating { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
        public virtual ICollection<MovieActor> MovieActors { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }

        public Movie()
        {
            MovieGenres = new HashSet<MovieGenre>();
            MovieActors = new HashSet<MovieActor>();
            Ratings = new HashSet<Rating>();
        }

        public void AddNewRating(Rating newRating)
        {
            Ratings.Add(newRating);
            CalculateAverageRating();
        }
        public void ChangeRating(string userId, int newRating)
        {
            Ratings.FirstOrDefault(r => r.UserId == userId).RatingValue=newRating;
            CalculateAverageRating();
        }
        public double CalculateAverageRating()
        {
            AverageRating = Ratings.Average(r => r.RatingValue);

            return Ratings.Average(r => r.RatingValue);
        }
    }
}
