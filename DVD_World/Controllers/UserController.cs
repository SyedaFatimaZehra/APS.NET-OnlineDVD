using DVD_World.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DVD_World.Controllers
{
    public class UserController : Controller
    {

        private readonly DvdContext db;
        private readonly IWebHostEnvironment env;

        public UserController(DvdContext DvdContext, IWebHostEnvironment env)
        {
            this.db = DvdContext;
            this.env = env;
        }

        public IActionResult UserLogin(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                TempData["Error"] = string.Join("<br>", errors);
                return View("UserLogin", user);
            }

            var existingUser = await db.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email || u.Username == user.Username);

            if (existingUser != null)
            {
                TempData["Error"] = "User already exists with this email or username!";
                return View("UserLogin");
            }

            // Ensure Role is set
            user.Role = "u";

            db.Users.Add(user);
            await db.SaveChangesAsync();

            TempData["Success"] = "Registration successful! You can now log in.";
            return RedirectToAction("UserLogin");
        }

        /*[HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);

            if (user == null)
            {
                TempData["Error"] = "Invalid username or password!";
                return RedirectToAction("UserLogin","User");
            }

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);
                //HttpContext.Session.SetString("UserRole", user.Role);
                return RedirectToAction("Index","User"); // Redirect to Home Page
            }

            TempData["Error"] = "Invalid username or password!";
            return RedirectToAction("UserLogin","User");
        }
*/


        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);

            if (user == null)
            {
                TempData["Error"] = "Invalid username or password!";
                return RedirectToAction("UserLogin", "User");
            }

            // Store user session
            HttpContext.Session.SetString("FullName", user.Name);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);

            // Retrieve return URL from TempData (if available)
            string returnUrl = TempData["ReturnUrl"] as string;

            if (!string.IsNullOrEmpty(user.Image))
            {
                HttpContext.Session.SetString("UserImage", user.Image);
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl); // Redirect to the intended page
            }

            return RedirectToAction("Index", "User"); // Default redirect
        }


        public IActionResult UserLogout()
        {
            HttpContext.Session.Clear(); // Clear all session values
            return RedirectToAction("UserLogin");
        }

        public IActionResult MyAccount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to view this page!";
                return RedirectToAction("UserLogin");
            }

            var user = db.Users.Find(userId);
            return View(user);
        }


        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser, string CurrentPassword, string NewPassword, string ConfirmPassword, IFormFile Image)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to view this page!";
                return RedirectToAction("UserLogin");
            }

            var user = db.Users.Find(userId);
            if (user != null)
            {
                // Update basic details
                user.Name = updatedUser.Name;
                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;

                // Password update logic
                if (!string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword))
                {
                    if (user.Password != CurrentPassword)
                    {
                        TempData["Error"] = "Current password is incorrect!";
                        return RedirectToAction("MyAccount");
                    }

                    if (NewPassword.Length < 8 || NewPassword.Length > 20)
                    {
                        TempData["Error"] = "Password must be between 8 to 20 characters!";
                        return RedirectToAction("MyAccount");
                    }

                    if (NewPassword != ConfirmPassword)
                    {
                        TempData["Error"] = "New password and confirm password do not match!";
                        return RedirectToAction("MyAccount");
                    }

                    // Update the password
                    user.Password = NewPassword;
                }

                if (Image != null)
                {
                    string fileName = Path.GetFileName(Image.FileName);
                    string filePath = Path.Combine("userImages", fileName);
                    string fullPath = Path.Combine(env.WebRootPath, "userImages", fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    user.Image = filePath;
                }

                db.Users.Update(user);
                db.SaveChanges();

                // Update session with new values
                HttpContext.Session.SetString("FullName", user.Name);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserEmail", user.Email);
                if (user.Image != null)
                {
                    HttpContext.Session.SetString("UserImage", user.Image);
                }

                TempData["Success"] = "Profile updated successfully!";
            }
            else
            {
                TempData["Error"] = "User not found!";
            }

            return RedirectToAction("MyAccount");
        }



        public async Task<IActionResult> Index()
        {
            var songs = await db.Songs.Include(s => s.Artist).Include(s => s.Album).ToListAsync();
            ViewBag.Artists = await db.Artists.ToListAsync(); 
            ViewBag.Albums = await db.Albums.ToListAsync(); 
            ViewBag.Movies = await db.Movies.ToListAsync();
            ViewBag.MovieTrailers = await db.MovieTrailers.Include(mt => mt.Movie).ToListAsync();
            ViewBag.GameTrailers = await db.GameTrailers.Include(gt => gt.Game).ToListAsync();

            ViewBag.Songs = songs;
            return View();
        }



        public async Task<IActionResult> SongDetail(int id)
        {
            var song = await db.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            ViewData["Feedback"] = new Feedback();

            return View(song);
        }

        public async Task<IActionResult> AlbumDetail(int id)
        {
            var album = await db.Albums
                .Include(a => a.Artist)
                .Include(a => a.Songs) 
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album); 
        }

        public async Task<IActionResult> MovieDetail(int id)
        {
            var movie = await db.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        public IActionResult MovieTrailerDetail(int id)
        {
            var movieTrailer = db.MovieTrailers
                .Include(mt => mt.Movie) 
                .FirstOrDefault(mt => mt.Id == id);

            if (movieTrailer == null)
            {
                return NotFound();
            }

            return View(movieTrailer);
        }

        public async Task<IActionResult> GameDetail(int id)
        {
            var game = await db.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        public IActionResult GameTrailerDetail(int id)
        {
            var gameTrailer = db.GameTrailers
                .Include(gt => gt.Game) 
                .FirstOrDefault(gt => gt.Id == id);

            if (gameTrailer == null)
            {
                return NotFound();
            }

            return View(gameTrailer);
        }

        public async Task<IActionResult> AllMovies()
        {
            var movies = await db.Movies.ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> AllMovieTrailers()
        {
            var movieTrailers = await db.MovieTrailers.Include(mt => mt.Movie).ToListAsync();
            return View(movieTrailers);
        }

        public async Task<IActionResult> AllGames()
        {
            var games = await db.Games.ToListAsync();
            return View(games);
        }

        public async Task<IActionResult> AllGameTrailers()
        {
            var gameTrailers = await db.GameTrailers.Include(gt => gt.Game).ToListAsync();
            return View(gameTrailers);
        }

        public async Task<IActionResult> Shop()
        {
            var products = await db.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to view this page!";
                return RedirectToAction("UserLogin");
            }
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        private const string CartSessionKey = "CartItems";

        // ✅ My Products (Cart View)
        public IActionResult MyProducts()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to view this page!";
                return RedirectToAction("UserLogin");
            }

            var cart = GetCartItems();
            return View(cart);
        }

        // ✅ Add Product to Cart
        [HttpGet("Product/AddToCart/{id}")]
        public IActionResult AddToCart(int id)
        {

            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            var cart = GetCartItems();
            cart.Add(product);
            SaveCartItems(cart);

            return RedirectToAction("MyProducts");
        }


        // ✅ Remove Product from Cart
        [HttpGet("Product/RemoveFromCart/{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();
            var product = cart.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                cart.Remove(product);
                SaveCartItems(cart);
            }

            return RedirectToAction("MyProducts");
        }

        // ✅ Get Cart Items from Session
        private List<Product> GetCartItems()
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            return sessionData != null ? JsonSerializer.Deserialize<List<Product>>(sessionData) : new List<Product>();
        }


        // ✅ Save Cart Items to Session
        private void SaveCartItems(List<Product> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        // Check out the cart

        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to view this page!";
                return RedirectToAction("UserLogin");
            }

            var cart = GetCartItems();

            if (cart.Count == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("MyProducts");
            }

            var viewModel = new CheckoutViewModel
            {
                Products = cart,
                TotalAmount = cart.Sum(p => p.Price)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId"); // Get logged-in user ID
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to place an order.";
                return RedirectToAction("UserLogin", "User");
            }

            var cart = GetCartItems();

            if (cart.Count == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("MyProducts");
            }

            foreach (var product in cart)
            {
                var order = new Order
                {
                    UserId = (int)userId, // Assign logged-in user ID
                    ProductId = product.Id,
                    OrderDetails = $"Order for {product.Name}",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = product.Price, /**/// Total amount based on quantity
                    Status = "Pending"
                };

                db.Orders.Add(order);
            }

            await db.SaveChangesAsync();

            // **Clear the cart after order placement**
            ClearCart();

            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToAction("OrderSuccess");
        }

        // **Method to Clear Cart**
        private void ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            Console.WriteLine("Cart has been cleared successfully!"); // Debugging message
        }

        public IActionResult OrderSuccess()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to view this page!";
                return RedirectToAction("UserLogin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] Feedback feedback)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Json(new { success = false, message = "Please login first to submit feedback!" });
            }

            if (feedback == null || feedback.Rating < 1 || feedback.Rating > 5 || string.IsNullOrEmpty(feedback.Message))
            {
                Console.WriteLine("Invalid Data Received: Rating=" + feedback?.Rating + ", Message=" + feedback?.Message);
                return Json(new { success = false, message = "Invalid data submitted!" });
            }

            feedback.UserId = (int)userId;

            db.Feedbacks.Add(feedback);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Feedback submitted successfully!" });
        }


        // ✅ Get All Feedbacks
        /*[HttpGet]
        public IActionResult GetFeedbacks()
        {
            var feedbacks = db.Feedbacks
                .Select(f => new
                {
                    UserName = db.Users.FirstOrDefault(u => u.Id == f.UserId).Name,
                    f.Message,
                    f.Rating
                })
                .ToList();

            return Json(feedbacks);
        }*/

        // Advertisement

        public async Task<IActionResult> AllAdvertisements()
        {
            var ads = await db.Advertisements.ToListAsync();
            return View(ads);
        }

        // About

        public IActionResult About()
        {
            return View();
        }

        // Contact
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields!";
                return RedirectToAction("Contact");
            }

            db.Contacts.Add(contact);
            await db.SaveChangesAsync();

            TempData["Success"] = "Your message has been sent successfully!";
            return RedirectToAction("Contact"); 
        }






    }
}
