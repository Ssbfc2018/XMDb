using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XMDb.Data;
using XMDb.Models;

namespace XMDb.Controllers
{
    [AllowAnonymous]
    public class ItemController : Controller
    {
        private XMDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ItemController(XMDbContext DB, ILogger<HomeController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _db = DB;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: ItemController/MovieDetails
        public ActionResult MovieDetails(int id)
        {
            string username = User.Identity.Name;
            ViewBag.Username = username;

            Movie movie = _db.Movies.Include(m => m.Ratings).FirstOrDefault(m => m.Id == id);

            // if rating or not
            if (_db.Movies.FirstOrDefault(m => m.Id == id).AverageRating != null)
            {
                ViewBag.rating = _db.Movies.FirstOrDefault(m => m.Id == id).AverageRating + "/10";
            }
            else
            {
                ViewBag.rating = "No Ratings";
            }

            // Rating or Changing: if user rating or not
            if (_db.Ratings.FirstOrDefault(r => r.User.UserName.Equals(username)) != null)
            {
                ViewBag.ratingORchanging = "Change Rating";
            }
            else
            {
                ViewBag.ratingORchanging = "Rating the Movie";
            }

            return View(_db.Movies.FirstOrDefault(m => m.Id == id));
        }

        // POST: ItemController/RatingMovie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RatingMovie(int ratingValue, int movieId)
        {
            string username = User.Identity.Name;
            User user = _db.Users.Include(u => u.Ratings).FirstOrDefault(u => u.UserName == username);

            Movie movie = _db.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieActors)
                .Include(m => m.Ratings)
                .FirstOrDefault(m => m.Id == movieId);
            if (_db.Ratings.FirstOrDefault(r => r.UserId == user.Id) != null){
                _db.Ratings.FirstOrDefault(r => r.UserId == user.Id).RatingValue = ratingValue;
                movie.AverageRating = movie.CalculateAverageRating();
                movie.ChangeRating(user.Id, ratingValue);
                user.ChangeRating(movie.Id, ratingValue);
            }
            else
            {
                Rating r=new Rating()
                {
                    UserId = user.Id,
                    MovieId = movie.Id,
                    RatingValue = ratingValue,
                    User = user,
                    Movie = movie
                };
                _db.Ratings.Add(r);
                movie.AverageRating = movie.CalculateAverageRating();
                movie.AddNewRating(r);
                user.AddNewRating(r);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("MovieDetails", new { id = movieId });
        }
    }
}
