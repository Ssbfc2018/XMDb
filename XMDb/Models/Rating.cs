namespace XMDb.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public int RatingValue { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;

        public Rating() { }
        public Rating(User user, Movie movie, int ratingValue)
        {
            User = user;
            UserId = user.Id;

            Movie = movie;
            MovieId = movie.Id;

            RatingValue = ratingValue;
        }
    }
}
