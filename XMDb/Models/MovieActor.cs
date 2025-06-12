namespace XMDb.Models
{
    public class MovieActor
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual Actor Actor { get; set; }
    }
}
