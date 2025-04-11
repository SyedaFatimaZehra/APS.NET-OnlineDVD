using DVD_World.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
/*using static DVD_World.Models.Song;
*/

namespace DVD_World.Controllers
{
    public class AdminController : Controller
    {
        private readonly DvdContext db;
        private readonly IWebHostEnvironment env;

        public AdminController(DvdContext DvdContext, IWebHostEnvironment env)
        {
            this.db = DvdContext;
            this.env = env;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login"); // Redirect if not logged in
            }


            ViewBag.TotalSongs = await db.Songs.CountAsync();
            ViewBag.TotalAlbums = await db.Albums.CountAsync();
            ViewBag.TotalMovies = await db.Movies.CountAsync();
            ViewBag.TotalGames = await db.Games.CountAsync();
            ViewBag.TotalUsers = await db.Users.CountAsync();
            ViewBag.TotalMovieTrailers = await db.MovieTrailers.CountAsync();
            ViewBag.TotalGameTrailers = await db.GameTrailers.CountAsync();
            ViewBag.TotalProducts = await db.Products.CountAsync();

            return View();
        }


        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var admin = await db.Admins
                .FirstOrDefaultAsync(a => a.Email == Email && a.Password == Password);

            if (admin == null)
            {
                TempData["Error"] = "Invalid email or password!";
                return RedirectToAction("Login");
            }

            // Store admin session
            HttpContext.Session.SetString("AdminEmail", admin.Email);
            HttpContext.Session.SetInt32("AdminId", admin.Id);
            HttpContext.Session.SetString("AdminName", admin.FullName);
            HttpContext.Session.SetString("AdminUsername", admin.Username);
            if (!string.IsNullOrEmpty(admin.ImageUrl))
            {
                HttpContext.Session.SetString("AdminImage", admin.ImageUrl);
            }

            return RedirectToAction("Dashboard"); 
        }

        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear(); // Clear session
            return RedirectToAction("Login");
        }

        


        // GET: All Games

        public async Task<IActionResult> Games()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            var games = await db.Games.ToListAsync();
            return View(games);
        }


        // POST: Add Game
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGame(Game game, IFormFile? GameUrl, IFormFile? GameImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (GameUrl != null)
                    {
                        string gameFileName = Path.GetFileName(GameUrl.FileName);
                        string gameFilePath = Path.Combine("games", gameFileName);
                        string fullGamePath = Path.Combine(env.WebRootPath, "games", gameFileName);

                        await using (var gameFileStream = new FileStream(fullGamePath, FileMode.Create))
                        {
                            await GameUrl.CopyToAsync(gameFileStream);
                        }
                        game.GameUrl = gameFilePath;
                    }

                    if (GameImage != null)
                    {
                        string imageFileName = Path.GetFileName(GameImage.FileName);
                        string imageFilePath = Path.Combine("gamesImages", imageFileName);
                        string fullImagePath = Path.Combine(env.WebRootPath, "gamesImages", imageFileName);

                        await using (var imageFileStream = new FileStream(fullImagePath, FileMode.Create))
                        {
                            await GameImage.CopyToAsync(imageFileStream);
                        }
                        game.GameImage = imageFilePath;
                    }

                    db.Add(game);
                    await db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Game added successfully!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Add Error: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while adding the game.";
                }
            }
            return RedirectToAction(nameof(Games));
        }


        // POST: Edit Game
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGame(int Id, Game updatedGame, IFormFile? GameUrl, IFormFile? GameImage)
        {
            var game = await db.Games.FindAsync(Id);
            if (game == null)
            {
                TempData["ErrorMessage"] = "Game not found!";
                return RedirectToAction(nameof(Games));
            }

            game.GameName = updatedGame.GameName;
            game.Description = updatedGame.Description;

            try
            {
                if (GameUrl != null)
                {
                    string gameFileName = Path.GetFileName(GameUrl.FileName);
                    string gameFilePath = Path.Combine("games", gameFileName);
                    string fullGamePath = Path.Combine(env.WebRootPath, "games", gameFileName);

                    await using (var gameFileStream = new FileStream(fullGamePath, FileMode.Create))
                    {
                        await GameUrl.CopyToAsync(gameFileStream);
                    }
                    game.GameUrl = gameFilePath;
                }

                if (GameImage != null)
                {
                    string imageFileName = Path.GetFileName(GameImage.FileName);
                    string imageFilePath = Path.Combine("gamesImages", imageFileName);
                    string fullImagePath = Path.Combine(env.WebRootPath, "gamesImages", imageFileName);

                    await using (var imageFileStream = new FileStream(fullImagePath, FileMode.Create))
                    {
                        await GameImage.CopyToAsync(imageFileStream);
                    }
                    game.GameImage = imageFilePath;
                }

                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Game updated successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while updating the game.";
            }

            return RedirectToAction(nameof(Games));
        }

        // POST: Delete Game

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGame(int Id)
        {
            var game = await db.Games.FindAsync(Id);
            if (game == null)
            {
                TempData["ErrorMessage"] = "Game not found!";
                return RedirectToAction(nameof(Games));
            }

            try
            {
                db.Games.Remove(game);
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Game deleted successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the game.";
            }

            return RedirectToAction(nameof(Games));
        }



        // GET: All Movies
        public async Task<IActionResult> Movies()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login"); 
            }

            var movies = await db.Movies.ToListAsync();
            return View(movies);
        }

        // POST: Add Movie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie(Movie movie, IFormFile MovieUrl, IFormFile MovieImage)
        {


            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid movie data.";
                return RedirectToAction(nameof(Movies)); 
            }

            if (MovieUrl == null || MovieImage == null)
            {
                TempData["ErrorMessage"] = "Movie file and image are required.";
                return RedirectToAction(nameof(Movies));
            }

            try
            {
                // Save Movie File
                string movieFileName = Path.GetFileName(MovieUrl.FileName);
                string movieFilePath = Path.Combine("movies", movieFileName);
                string fullMoviePath = Path.Combine(env.WebRootPath, movieFilePath);

                await using (var fileStream = new FileStream(fullMoviePath, FileMode.Create))
                {
                    await MovieUrl.CopyToAsync(fileStream);
                }

                string imageFileName = Path.GetFileName(MovieImage.FileName);
                string imageFilePath = Path.Combine("moviesImage", imageFileName);
                string fullImagePath = Path.Combine(env.WebRootPath, imageFilePath);

                await using (var imageStream = new FileStream(fullImagePath, FileMode.Create))
                {
                    await MovieImage.CopyToAsync(imageStream);
                }

                // Save Data to DB
                movie.MovieUrl = movieFilePath; 
                movie.MovieImage = imageFilePath; 

                db.Movies.Add(movie);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Movie added successfully!";
                return RedirectToAction(nameof(Movies)); // Ensure Index exists
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Movies));
            }
        }

        // POST: Edit Movie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(int Id, Movie updatedMovie, IFormFile? MovieUrl, IFormFile? MovieImage)
        {
            var movie = await db.Movies.FindAsync(Id);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "Movie not found!";
                return RedirectToAction(nameof(Movies));
            }

            // Update movie properties
            movie.MovieName = updatedMovie.MovieName;
            movie.Description = updatedMovie.Description;

            try
            {
                // Ensure directories exist
                string movieDirectory = Path.Combine(env.WebRootPath, "movies");
                if (!Directory.Exists(movieDirectory))
                    Directory.CreateDirectory(movieDirectory);

                string imageDirectory = Path.Combine(env.WebRootPath, "moviesImage");
                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);

                // Update movie file if a new one is uploaded
                if (MovieUrl != null)
                {
                    string movieFileName = Path.GetFileName(MovieUrl.FileName);
                    string fullMoviePath = Path.Combine(movieDirectory, movieFileName);

                    await using (var movieFileStream = new FileStream(fullMoviePath, FileMode.Create))
                    {
                        await MovieUrl.CopyToAsync(movieFileStream);
                    }
                    movie.MovieUrl = Path.Combine("movies", movieFileName);
                }

                // Update movie image if a new one is uploaded
                if (MovieImage != null)
                {
                    string imageFileName = Path.GetFileName(MovieImage.FileName);
                    string fullImagePath = Path.Combine(imageDirectory, imageFileName);

                    await using (var imageFileStream = new FileStream(fullImagePath, FileMode.Create))
                    {
                        await MovieImage.CopyToAsync(imageFileStream);
                    }
                    movie.MovieImage = Path.Combine("moviesImage", imageFileName);
                }

                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Movie updated successfully!";
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"EF Core Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                TempData["ErrorMessage"] = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit Error: {ex}");
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Movies));
        }

        // POST: Delete Movie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovie(int Id)
        {
            var movie = await db.Movies.FindAsync(Id);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "Movie not found!";
                return RedirectToAction(nameof(Movies));
            }

            db.Movies.Remove(movie);
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Movie deleted successfully!";
            return RedirectToAction(nameof(Movies));
        }

        // Display Songs
        public async Task<IActionResult> AllSongs()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var songs = await db.Songs.Include(s => s.Artist).Include(s => s.Album).ToListAsync();
            ViewBag.Artists = await db.Artists.ToListAsync(); 
            ViewBag.Albums = await db.Albums.ToListAsync(); 
            ViewBag.Songs = songs;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSong(Song song, IFormFile SongUrl, IFormFile SongImage)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Invalid data: " + string.Join(", ", errors);
                return RedirectToAction(nameof(AllSongs));
            }

            if (SongUrl == null || SongImage == null)
            {
                TempData["ErrorMessage"] = "Song file and image are required.";
                return RedirectToAction(nameof(AllSongs));
            }

            try
            {
                // Upload Song File
                string songFileName = Path.GetFileName(SongUrl.FileName);
                string songFilePath = Path.Combine("songs", songFileName);
                string fullSongPath = Path.Combine(env.WebRootPath, "songs", songFileName);

                await using (var songFileStream = new FileStream(fullSongPath, FileMode.Create))
                {
                    await SongUrl.CopyToAsync(songFileStream);
                }

                // Upload Song Image
                string imageFileName = Path.GetFileName(SongImage.FileName);
                string imageFilePath = Path.Combine("songsImage", imageFileName);
                string fullImagePath = Path.Combine(env.WebRootPath, "songsImage", imageFileName);

                await using (var imageFileStream = new FileStream(fullImagePath, FileMode.Create))
                {
                    await SongImage.CopyToAsync(imageFileStream);
                }

                // Save File Paths to Database
                song.SongUrl = songFilePath;   
                song.SongImage = imageFilePath;

                db.Songs.Add(song);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Song uploaded successfully!";
                return RedirectToAction(nameof(AllSongs));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while uploading the song.";
                return RedirectToAction(nameof(AllSongs));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSong(int Id, Song updatedSong, IFormFile? SongUrl, IFormFile? SongImage)
        {
            var song = await db.Songs.FindAsync(Id);
            if (song == null)
            {
                TempData["ErrorMessage"] = "Song not found!";
                return RedirectToAction(nameof(AllSongs));
            }

            // Update song properties
            song.SongName = updatedSong.SongName;
            song.ArtistId = updatedSong.ArtistId;
            song.AlbumId = updatedSong.AlbumId;
            song.Genre = updatedSong.Genre;
            song.Year = updatedSong.Year;

            try
            {
                // Update song file if new file is uploaded
                if (SongUrl != null)
                {
                    string songFileName = Path.GetFileName(SongUrl.FileName);
                    string songFilePath = Path.Combine("songs", songFileName);
                    string fullSongPath = Path.Combine(env.WebRootPath, "songs", songFileName);

                    await using (var songFileStream = new FileStream(fullSongPath, FileMode.Create))
                    {
                        await SongUrl.CopyToAsync(songFileStream);
                    }
                    song.SongUrl = songFilePath;
                }

                // Update song image if new image is uploaded
                if (SongImage != null)
                {
                    string imageFileName = Path.GetFileName(SongImage.FileName);
                    string imageFilePath = Path.Combine("songsImage", imageFileName);
                    string fullImagePath = Path.Combine(env.WebRootPath, "songsImage", imageFileName);

                    await using (var imageFileStream = new FileStream(fullImagePath, FileMode.Create))
                    {
                        await SongImage.CopyToAsync(imageFileStream);
                    }
                    song.SongImage = imageFilePath;
                }

                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Song updated successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while updating the song.";
            }

            return RedirectToAction(nameof(AllSongs));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSong(int Id)
        {
            var song = await db.Songs.FindAsync(Id);
            if (song == null)
            {
                TempData["ErrorMessage"] = "Song not found!";
                return RedirectToAction(nameof(AllSongs));
            }

            try
            {
                db.Songs.Remove(song);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Song deleted successfully!";
                return RedirectToAction(nameof(AllSongs));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the song.";
                return RedirectToAction(nameof(AllSongs));
            }
        }


        // GET: Artists List
        public IActionResult Artist()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            var artists = db.Artists.ToList();
            ViewBag.Artists = artists;
            return View();
        }

        // POST: Create Artist
        [HttpPost]
        public async Task<IActionResult> AddArtist(Artist artist, IFormFile? ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Artist");
            }

            if (ImageUrl != null)
            {
                string artistFileName = Path.GetFileName(ImageUrl.FileName);
                string imagePath = Path.Combine("artistImages", artistFileName);
                string fullArtistPath = Path.Combine(env.WebRootPath, "artistImages", artistFileName);

                // Ensure directory exists
                string artistDirectory = Path.Combine(env.WebRootPath, "artistImages");
                if (!Directory.Exists(artistDirectory))
                {
                    Directory.CreateDirectory(artistDirectory);
                }

                using (var stream = new FileStream(fullArtistPath, FileMode.Create))
                {
                    await ImageUrl.CopyToAsync(stream);
                }

                artist.ImageUrl = imagePath;
            }

            db.Artists.Add(artist);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Artist added successfully.";
            return RedirectToAction("Artist");
        }

        [HttpPost]
        public async Task<IActionResult> EditArtist(Artist artist, IFormFile? ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Artist");
            }

            var existingArtist = await db.Artists.FindAsync(artist.Id);
            if (existingArtist == null)
            {
                TempData["ErrorMessage"] = "Artist not found.";
                return RedirectToAction("Artist");
            }

            if (ImageUrl != null)
            {
                string artistDirectory = Path.Combine(env.WebRootPath, "artistImages");
                if (!Directory.Exists(artistDirectory))
                {
                    Directory.CreateDirectory(artistDirectory);
                }

                string artistFileName = Path.GetFileName(ImageUrl.FileName);
                string fullArtistPath = Path.Combine(artistDirectory, artistFileName);

                using (var stream = new FileStream(fullArtistPath, FileMode.Create))
                {
                    await ImageUrl.CopyToAsync(stream);
                }

                existingArtist.ImageUrl = Path.Combine("artistImages", artistFileName);
            }

            existingArtist.Name = artist.Name;
            existingArtist.Bio = artist.Bio;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Artist updated successfully.";
            return RedirectToAction("Artist");
        }

        // POST: Delete Artist
        [HttpPost]
        public IActionResult DeleteArtist(int id)
        {
            var artist = db.Artists.Find(id);
            if (artist != null)
            {
                db.Artists.Remove(artist);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Artist deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Artist not found.";
            }
            return RedirectToAction("Artist");
        }


        // GET: Albums

        public async Task<IActionResult> Albums()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            var albums = await db.Albums.Include(a => a.Artist).ToListAsync();
            ViewBag.Artists = await db.Artists.ToListAsync(); 
            return View(albums);
        }

        [HttpPost]
        public async Task<IActionResult> AddAlbum(Album album, IFormFile? ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Albums");
            }

            if (ImageUrl != null)
            {
                string albumFileName = Path.GetFileName(ImageUrl.FileName);
                string imagePath = Path.Combine("albumImages", albumFileName);
                string fullAlbumPath = Path.Combine(env.WebRootPath, "albumImages", albumFileName);

                // Ensure directory exists
                string albumDirectory = Path.Combine(env.WebRootPath, "albumImages");
                if (!Directory.Exists(albumDirectory))
                {
                    Directory.CreateDirectory(albumDirectory);
                }

                using (var stream = new FileStream(fullAlbumPath, FileMode.Create))
                {
                    await ImageUrl.CopyToAsync(stream);
                }
                album.ImageUrl = imagePath;
            }

            db.Albums.Add(album);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Album added successfully.";
            return RedirectToAction("Albums");
        }

        [HttpPost]
        public async Task<IActionResult> EditAlbum(Album album, IFormFile? ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Albums");
            }

            var existingAlbum = await db.Albums.FindAsync(album.Id);
            if (existingAlbum == null)
            {
                TempData["ErrorMessage"] = "Album not found.";
                return RedirectToAction("Albums");
            }

            if (ImageUrl != null)
            {
                string albumDirectory = Path.Combine(env.WebRootPath, "albumImages");
                if (!Directory.Exists(albumDirectory))
                {
                    Directory.CreateDirectory(albumDirectory);
                }

                string albumFileName = Path.GetFileName(ImageUrl.FileName);
                string fullAlbumPath = Path.Combine(albumDirectory, albumFileName);

                using (var stream = new FileStream(fullAlbumPath, FileMode.Create))
                {
                    await ImageUrl.CopyToAsync(stream);
                }

                existingAlbum.ImageUrl = Path.Combine("albumImages", albumFileName);
            }

            existingAlbum.Title = album.Title;
            existingAlbum.ArtistId = album.ArtistId;
            existingAlbum.Price = album.Price;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Album updated successfully.";
            return RedirectToAction("Albums");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            var album = await db.Albums.FindAsync(id);
            if (album != null)
            {
                db.Albums.Remove(album);
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Album deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete album.";
            }
            return RedirectToAction("Albums");
        }

        // GET: News
        public async Task<IActionResult> News()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login"); 
            }
            var newsList = await db.News.ToListAsync();
            return View(newsList);
        }

        [HttpPost]
        public async Task<IActionResult> AddNews(News news, IFormFile? NewsImage)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("News");
            }

            if (NewsImage != null)
            {
                string newsFileName = Path.GetFileName(NewsImage.FileName);
                string imagePath = Path.Combine("newsImages",newsFileName);
                string fullNewsPath = Path.Combine(env.WebRootPath, "newsImages", newsFileName);
                using (var stream = new FileStream(fullNewsPath, FileMode.Create))
                {
                    await NewsImage.CopyToAsync(stream);
                }
                news.NewsImage = imagePath;
            }

            db.News.Add(news);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "News added successfully.";
            return RedirectToAction("News");
        }
        [HttpPost]
        public async Task<IActionResult> EditNews(News news, IFormFile? NewsImage)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("News");
            }

            var existingNews = await db.News.FindAsync(news.Id);
            if (existingNews == null)
            {
                TempData["ErrorMessage"] = "News not found.";
                return RedirectToAction("News");
            }

            if (NewsImage != null)
            {
                // Ensure directory exists
                string newsDirectory = Path.Combine(env.WebRootPath, "newsImages");
                if (!Directory.Exists(newsDirectory))
                {
                    Directory.CreateDirectory(newsDirectory);
                }

                // Save the new file
                string newsFileName = Path.GetFileName(NewsImage.FileName);
                string fullNewsPath = Path.Combine(newsDirectory, newsFileName);

                using (var stream = new FileStream(fullNewsPath, FileMode.Create))
                {
                    await NewsImage.CopyToAsync(stream);
                }

                existingNews.NewsImage = Path.Combine("newsImages", newsFileName);
            }

            // Update other fields
            existingNews.Title = news.Title;
            existingNews.Content = news.Content;
            existingNews.Type = news.Type;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "News updated successfully.";
            return RedirectToAction("News");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await db.News.FindAsync(id);
            if (news == null)
            {
                TempData["ErrorMessage"] = "News not found.";
                return RedirectToAction("News");
            }

            db.News.Remove(news);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "News deleted successfully.";
            return RedirectToAction("News");
        }


        // GET: Game Trailers
        public async Task<IActionResult> GameTrailer()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var gameTrailers = await db.GameTrailers.Include(g => g.Game).ToListAsync();
            ViewBag.Games = await db.Games.ToListAsync();
            return View(gameTrailers);
        }

        [HttpPost]
        public async Task<IActionResult> AddTrailer(GameTrailer trailer, IFormFile TrailerUrl)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = "Invalid input: " + string.Join("; ", errors);
                ViewBag.Games = await db.Games.ToListAsync();
                return View("GameTrailer");
            }

            bool gameExists = db.Games.Any(g => g.Id == trailer.GameId);
            if (!gameExists)
            {
                TempData["ErrorMessage"] = "Selected game does not exist.";
                ViewBag.Games = await db.Games.ToListAsync();
                return View("GameTrailer");
            }

            if (TrailerUrl != null)
            {
                string fileName = Path.GetFileName(TrailerUrl.FileName);
                string filePath = Path.Combine("gameTrailers", fileName);
                string fullFilePath = Path.Combine(env.WebRootPath, filePath);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await TrailerUrl.CopyToAsync(stream);
                }

                trailer.TrailerUrl = filePath;
            }

            db.GameTrailers.Add(trailer);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Game Trailer added successfully.";
            return RedirectToAction("GameTrailer");
        }


        [HttpPost]
        public async Task<IActionResult> EditTrailer(GameTrailer trailer, IFormFile TrailerUrl)
        {
          

            var existingTrailer = await db.GameTrailers.FindAsync(trailer.Id);
            if (existingTrailer == null)
            {
                TempData["ErrorMessage"] = "Trailer not found.";
                return RedirectToAction("GameTrailer");
            }

            if (TrailerUrl != null)
            {
                string fileName = Path.GetFileName(TrailerUrl.FileName);
                string filePath = Path.Combine("gameTrailers", fileName);
                string fullFilePath = Path.Combine(env.WebRootPath, filePath);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await TrailerUrl.CopyToAsync(stream);
                }
                existingTrailer.TrailerUrl = filePath;
            }

            existingTrailer.Title = trailer.Title;
            existingTrailer.GameId = trailer.GameId;
            existingTrailer.ReleaseDate = trailer.ReleaseDate;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Game Trailer updated successfully.";
            return RedirectToAction("GameTrailer");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrailer(int id)
        {
            var trailer = await db.GameTrailers.FindAsync(id);
            if (trailer == null)
            {
                TempData["ErrorMessage"] = "Trailer not found.";
                return RedirectToAction("GameTrailer");
            }

            if (!string.IsNullOrEmpty(trailer.TrailerUrl))
            {
                string filePath = Path.Combine(env.WebRootPath, trailer.TrailerUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            db.GameTrailers.Remove(trailer);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Game Trailer deleted successfully.";
            return RedirectToAction("GameTrailer");
        }

        public async Task<IActionResult> MovieTrailer()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var movieTrailers = await db.MovieTrailers.Include(m => m.Movie).ToListAsync();
            ViewBag.Movies = await db.Movies.ToListAsync();
            return View(movieTrailers);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieTrailer(MovieTrailer trailer, IFormFile TrailerUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("MovieTrailer");
            }

            if (TrailerUrl != null)
            {
                string fileName = Path.GetFileName(TrailerUrl.FileName);
                string filePath = Path.Combine("movieTrailers", fileName);
                string fullFilePath = Path.Combine(env.WebRootPath, filePath);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await TrailerUrl.CopyToAsync(stream);
                }
                trailer.TrailerUrl = filePath;
            }

            db.MovieTrailers.Add(trailer);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie Trailer added successfully.";
            return RedirectToAction("MovieTrailer");
        }

        [HttpPost]
        public async Task<IActionResult> EditMovieTrailer(MovieTrailer trailer, IFormFile TrailerUrl)
        {
            

            var existingTrailer = await db.MovieTrailers.FindAsync(trailer.Id);
            if (existingTrailer == null)
            {
                TempData["ErrorMessage"] = "Trailer not found.";
                return RedirectToAction("MovieTrailer");
            }

            if (TrailerUrl != null)
            {
                string fileName = Path.GetFileName(TrailerUrl.FileName);
                string filePath = Path.Combine("movieTrailers", fileName);
                string fullFilePath = Path.Combine(env.WebRootPath, filePath);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await TrailerUrl.CopyToAsync(stream);
                }
                existingTrailer.TrailerUrl = filePath;
            }

            existingTrailer.Title = trailer.Title;
            existingTrailer.MovieId = trailer.MovieId;
            existingTrailer.ReleaseDate = trailer.ReleaseDate;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie Trailer updated successfully.";
            return RedirectToAction("MovieTrailer");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMovieTrailer(int Id)
        {
            var trailer = await db.MovieTrailers.FindAsync(Id);
            if (trailer == null)
            {
                TempData["ErrorMessage"] = "Trailer not found.";
                return RedirectToAction("MovieTrailer");
            }

            db.MovieTrailers.Remove(trailer);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie Trailer deleted successfully.";
            return RedirectToAction("MovieTrailer");
        }

        // GET: All Products
        public async Task<IActionResult> Products()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Albums = await db.Albums.ToListAsync();
            ViewBag.Movies = await db.Movies.ToListAsync();
            ViewBag.Games = await db.Games.ToListAsync();

            var products = await db.Products
                .Include(p => p.Album)
                .Include(p => p.Movie)
                .Include(p => p.Game)
                .ToListAsync();

            return View(products);
        }

        // POST: Add Product
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile? ProductImage)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Products");
            }

            if (ProductImage != null)
            {
                string fileName = Path.GetFileName(ProductImage.FileName);
                string imagePath = Path.Combine("productImages", fileName);
                string fullPath = Path.Combine(env.WebRootPath, "productImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ProductImage.CopyToAsync(stream);
                }
                product.ImageUrl = imagePath;
            }

            db.Products.Add(product);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product added successfully.";
            return RedirectToAction("Products");
        }

        // POST: Edit Product
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? ProductImage)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Products");
            }

            var existingProduct = await db.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Products");
            }

            // Handle image update
            if (ProductImage != null)
            {
                string fileName = Path.GetFileName(ProductImage.FileName);
                string fullPath = Path.Combine(env.WebRootPath, "productImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ProductImage.CopyToAsync(stream);
                }

                existingProduct.ImageUrl = Path.Combine("productImages", fileName);
            }

            // Update product details
            existingProduct.Name = product.Name;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;

            // Ensure AlbumId, MovieId, and GameId are correctly updated
            existingProduct.AlbumId = product.AlbumId;
            existingProduct.MovieId = product.MovieId;
            existingProduct.GameId = product.GameId;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product updated successfully.";
            return RedirectToAction("Products");
        }


        // POST: Delete Product
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var product = await db.Products.FindAsync(Id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Products");
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction("Products");
        }

        // GET: All Orders
        public async Task<IActionResult> Orders()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var orders = await db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();


            return View(orders);
        }


        // Get: Advertisements
        public async Task<IActionResult> Advertisements()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var ads = await db.Advertisements.ToListAsync();
            return View(ads);
        }

        // POST: Add Advertisement
        [HttpPost]
        public async Task<IActionResult> AddAdvertisement(Advertisement ad, IFormFile? BannerUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Advertisements");
            }

            if (BannerUrl != null)
            {
                string fileName = Path.GetFileName(BannerUrl.FileName);
                string filePath = Path.Combine("advertisementImages", fileName);
                string fullPath = Path.Combine(env.WebRootPath, "advertisementImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await BannerUrl.CopyToAsync(stream);
                }
                ad.BannerUrl = filePath;
            }

            db.Advertisements.Add(ad);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Advertisement added successfully.";
            return RedirectToAction("Advertisements");
        }

        // POST: Edit Advertisement
        [HttpPost]
        public async Task<IActionResult> EditAdvertisement(Advertisement ad, IFormFile? BannerUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Advertisements");
            }

            var existingAd = await db.Advertisements.FindAsync(ad.Id);
            if (existingAd == null)
            {
                TempData["ErrorMessage"] = "Advertisement not found.";
                return RedirectToAction("Advertisements");
            }

            if (BannerUrl != null)
            {
                string fileName = Path.GetFileName(BannerUrl.FileName);
                string filePath = Path.Combine("advertisementImages", fileName);
                string fullPath = Path.Combine(env.WebRootPath, "advertisementImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await BannerUrl.CopyToAsync(stream);
                }
                existingAd.BannerUrl = filePath;
            }

            existingAd.Name = ad.Name;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Advertisement updated successfully.";
            return RedirectToAction("Advertisements");
        }

        // POST: Delete Advertisement
        [HttpPost]
        public async Task<IActionResult> DeleteAdvertisement(int Id)
        {
            var ad = await db.Advertisements.FindAsync(Id);
            if (ad == null)
            {
                TempData["ErrorMessage"] = "Advertisement not found.";
                return RedirectToAction("Advertisements");
            }

            db.Advertisements.Remove(ad);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Advertisement deleted successfully.";
            return RedirectToAction("Advertisements");
        }

        // Get: Feedback
        public async Task<IActionResult> Feedback()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var feedbacks = await db.Feedbacks.Include(f => f.User).ToListAsync();
            return View(feedbacks);
        }

        // Get : All Users
        public async Task<IActionResult> AllUsers()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var users = await db.Users.ToListAsync();
            return View(users);
        }

        // POST: Edit User with image
        [HttpPost]
        public async Task<IActionResult> EditUser(User user, IFormFile? Image)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("AllUsers");
            }

            var existingUser = await db.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("AllUsers");
            }

            if (Image != null)
            {
                string fileName = Path.GetFileName(Image.FileName);
                string filePath = Path.Combine("userImages", fileName);
                string fullPath = Path.Combine(env.WebRootPath, "userImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                existingUser.Image = filePath;
            }

            existingUser.Name = user.Name;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction("AllUsers");
        }

        // POST: Delete User
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await db.Users.FindAsync(Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("AllUsers");
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction("AllUsers");
        }

        // GET: Admin
        public IActionResult Admin()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var admin = db.Admins.ToList();
            return View(admin);
        }

        public async Task<IActionResult> AddAdmin(Admin admin, IFormFile? ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Admin");
            }

            if (ImageUrl != null)
            {
                string fileName = Path.GetFileName(ImageUrl.FileName);
                string filePath = Path.Combine("adminImages", fileName);
                string fullPath = Path.Combine(env.WebRootPath, "adminImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ImageUrl.CopyToAsync(stream);
                }
                admin.ImageUrl = filePath;
            }

            db.Admins.Add(admin);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Admin added successfully.";
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> EditAdmin(Admin admin, IFormFile? ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return RedirectToAction("Admin");
            }

            var existingAdmin = await db.Admins.FindAsync(admin.Id);
            if (existingAdmin == null)
            {
                TempData["ErrorMessage"] = "Admin not found.";
                return RedirectToAction("Admin");
            }

            if (ImageUrl != null)
            {
                string fileName = Path.GetFileName(ImageUrl.FileName);
                string filePath = Path.Combine("adminImages", fileName);
                string fullPath = Path.Combine(env.WebRootPath, "adminImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ImageUrl.CopyToAsync(stream);
                }
                existingAdmin.ImageUrl = filePath;
            }

            existingAdmin.FullName = admin.FullName;
            existingAdmin.Username = admin.Username;
            existingAdmin.Email = admin.Email;

            if (!string.IsNullOrEmpty(admin.Password))
            {
                existingAdmin.Password = admin.Password; // Ideally, hash the password before saving.
            }

            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Admin updated successfully.";
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(int Id)
        {
            var admin = await db.Admins.FindAsync(Id);
            if (admin == null)
            {
                TempData["ErrorMessage"] = "Admin not found.";
                return RedirectToAction("Admin");
            }

            db.Admins.Remove(admin);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Admin deleted successfully.";
            return RedirectToAction("Admin");
        }


        // Edit Profile
        [HttpPost]
        public async Task<IActionResult> EditProfile(int Id, string FullName, string Username, string Email, string Password, IFormFile? ProfileImage)
        {
            var admin = await db.Admins.FindAsync(Id);
            if (admin == null)
            {
                TempData["Error"] = "Admin not found.";
                return RedirectToAction("Dashboard");
            }

            admin.FullName = FullName;
            admin.Username = Username;
            admin.Email = Email;

            if (!string.IsNullOrEmpty(Password))
            {
                admin.Password = Password;
            }

            if (ProfileImage != null)
            {
                string fileName = Path.GetFileName(ProfileImage.FileName);
                string filePath = Path.Combine("adminImages", fileName);
                string fullPath = Path.Combine(env.WebRootPath, "adminImages", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                admin.ImageUrl = filePath;
            }

            await db.SaveChangesAsync();

            // Update session with new values
            HttpContext.Session.SetString("AdminName", admin.FullName);
            HttpContext.Session.SetString("AdminUsername", admin.Username);
            HttpContext.Session.SetString("AdminEmail", admin.Email);
            if (admin.ImageUrl != null)
            {
                HttpContext.Session.SetString("AdminImage", admin.ImageUrl);
            }

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Dashboard");
        }


    }
}
