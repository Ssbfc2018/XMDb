using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XMDb.Data;
using XMDb.Models;

namespace XMDb.Controllers
{
    [AllowAnonymous]
    public class XMDbController : Controller
    {
        private XMDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public XMDbController(XMDbContext DB, ILogger<HomeController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _db = DB;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> MovieIndex(string title, string? currentPageIndex)
        {
            //Recent or Actor
            List<Movie> movies = null;
            if (title.Equals("recent"))
            {//recent
                movies = _db.Movies.Include(m => m.MovieGenres).ToList();
                ViewBag.title = "Recent";
            }
            else
            {
                ViewBag.title = title;
                movies = new List<Movie>();
                List<Movie> allMovies = _db.Movies.Include(m => m.MovieGenres).ToList();
                Actor a = _db.Actors.FirstOrDefault(a => a.FullName.Equals(title));
                if (_db.Actors.FirstOrDefault(a => a.FullName.Equals(title)) != null)
                {//actor
                    movies = movieActorList(_db.Actors
                        .Include(a => a.MovieActors)
                        .FirstOrDefault(a => a.FullName.Equals(title)), allMovies, movies);
                }
                else
                {//genre
                    movies = movieGenreList(_db.Genres
                        .Include(g => g.MovieGenres)
                        .FirstOrDefault(g => g.Name.Equals(title)), allMovies, movies);
                }
            }

            //Deliver genres
            ViewBag.Genres = _db.Genres.ToList();

            //Pagination
            int page = 1;
            ViewBag.PageIndex = 1;
            if (currentPageIndex != null)
            {
                ViewBag.PageIndex = Int32.Parse(currentPageIndex);
                page = Int32.Parse(currentPageIndex);
            }
            ViewBag.Next = ViewBag.PageIndex + 1;
            ViewBag.Prev = ViewBag.PageIndex - 1;

            ViewBag.PageSize = movies.Count() / 10 + 1;
            //Eg. page 2: skip Movie 1-10 and show Movie 11-20
            List<Movie> currentPage = null;
            if (page == movies.Count() / 10 + 1)
            {
                currentPage = movies.GetRange((page - 1) * 10, movies.Count() - (page - 1) * 10);
            }
            else
            {
                currentPage = movies.GetRange((page - 1) * 10, 10);
            }

            return View(currentPage.ToList());
        }
        public List<Movie> movieActorList(Actor actor, List<Movie> allMovies, List<Movie> movies)
        {
            foreach (var ma in actor.MovieActors)
            {
                movies.Add(allMovies.FirstOrDefault(m => m.Id == ma.MovieId));
            }
            return (movies);
        }
        public List<Movie> movieGenreList(Genre genre, List<Movie> allMovies, List<Movie> movies)
        {
            foreach (var mg in genre.MovieGenres)
            {
                movies.Add(allMovies.FirstOrDefault(m => m.Id == mg.MovieId));
            }
            return (movies);
        }

        public IActionResult ActorIndex()
        {
            List<Actor> actors = _db.Actors.OrderBy(a => a.FullName).ToList();

            return View(actors);
        }

        // GET
        public IActionResult GenreIndex()
        {
            ViewBag.Genres = new SelectList(_db.Genres, "Id", "Name");

            return View();
        }
        [HttpPost]
        public IActionResult GenreIndex(int GenreId)
        {
            Genre genre = _db.Genres.First(g => g.Id == GenreId);
            return RedirectToAction("MovieIndex", new { title = genre.Name });
        }
    }
}
