namespace XMDb.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }

        public Genre()
        {
            MovieGenres = new HashSet<MovieGenre>();
        }
    }
}
