using Microsoft.AspNetCore.Identity;

namespace XMDb.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Rating> Ratings { get; set; }

        public User()
        {
            Ratings = new HashSet<Rating>();
        }

        public void AddNewRating(Rating newRating)
        {
            Ratings.Add(newRating);
        }
        public void ChangeRating(int movieId, int newRating)
        {
            Ratings.FirstOrDefault(r => r.MovieId == movieId).RatingValue = newRating;
        }
    }
}
