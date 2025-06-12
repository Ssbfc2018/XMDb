namespace XMDb.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public virtual ICollection<MovieActor> MovieActors { get; set; }

        public Actor()
        {
            MovieActors = new HashSet<MovieActor>();
        }
    }
}
