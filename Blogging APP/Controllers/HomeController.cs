using Blogging_APP.Data;
using Blogging_APP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http; // For session
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;

// Your existing namespace here, if any
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(Login model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Registrations
                .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password); // ⚠️ Consider hashing passwords

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);
                return RedirectToAction("Blog");
            }

            ViewBag.Error = "Invalid email or password";
        }

        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registration(Registration model)
    {
        if (ModelState.IsValid)
        {
            var exists = _context.Registrations.Any(u => u.Email == model.Email);
            if (exists)
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(model);
            }

            _context.Registrations.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        return View(model);
    }
    public IActionResult Blog()
    {
        var blogs = _context.Blogs.Select(b => new BlogPost
        {
            Id = b.Id,
            Title = b.Title,
            Category = b.Category,
            Tags = b.Tags,
            Content = b.Content
        }).ToList();

        return View(blogs);
    }

    [HttpGet]
    public IActionResult CreateBlog()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateBlog(CreateBlog model)
    {
        if (ModelState.IsValid)
        {
            var blogPost = new BlogPost
            {
                Title = model.Title,
                Category = model.Category,
                Tags = model.Tags,
                Content = model.Content
            };

            _context.Blogs.Add(blogPost);
            _context.SaveChanges();
            return RedirectToAction("Blog");
        }

        return View(model);
    }

    public IActionResult ManageBlogs()
    {
        if (!IsAdmin()) return Unauthorized();

        var blogs = _context.Blogs.Select(b => new BlogPost
        {
            Id = b.Id,
            Title = b.Title,
            Category = b.Category,
            Tags = b.Tags,
            Content = b.Content
        }).ToList();

        return View(blogs);
    }

    [HttpGet]
    public IActionResult EditBlog(int id)
    {
        if (!IsAdmin()) return Unauthorized();

        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null) return NotFound();

        // Explicitly specify view path (optional safety net)
        return View("EditBlog", blog); // Looks for Views/Home/EditBlog.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditBlog(BlogPost model)
    {
        if (!IsAdmin()) return Unauthorized();

        if (ModelState.IsValid)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == model.Id);
            if (blog == null) return NotFound();

            blog.Title = model.Title;
            blog.Category = model.Category;
            blog.Tags = model.Tags;
            blog.Content = model.Content;

            _context.SaveChanges();
            TempData["Success"] = "Blog updated successfully.";
            return RedirectToAction("ManageBlogs");
        }

        return View(model);
    }


    [HttpGet]
    public IActionResult DeleteBlog(int id)
    {
        if (!IsAdmin()) return Unauthorized();

        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null) return NotFound();

        _context.Blogs.Remove(blog);
        _context.SaveChanges();
        TempData["Success"] = "Blog deleted successfully.";
        return RedirectToAction("ManageBlogs");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private bool IsAdmin()
    {
        var role = HttpContext.Session.GetString("UserRole");
        return role == "Admin";
    }

    // --------------- Gemini AI Integration Code ----------------------

    public class AIRequest
    {
        public string Content { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> AISuggestions([FromBody] AIRequest request)
    {
        string prompt = $@"
Suggest a catchy blog title and 5 relevant tags for the following blog content.

Return the response as JSON in this format:
{{ ""title"": ""..."", ""tags"": [""..."", ""..."", ""..."", ""..."", ""...""] }}

Blog content:
{request.Content}";


        string apiKey = "AIzaSyB1BMdHNLP26OEZwVpglYK-Z2xz1V3AtzU";

        string response = await CallGeminiAPI(prompt, apiKey);
        var parts = ParseResponse(response);

        return Json(new
        {
            title = parts.Title,
            tags = parts.Tags
        });
    }

    private async Task<string> CallGeminiAPI(string prompt, string apiKey)
    {
        string model = "gemini-2.5-flash-preview-04-17";
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            return await response.Content.ReadAsStringAsync();
        }
    }

    private (string Title, string[] Tags) ParseResponse(string response)
    {
        string title = "Generated Title";
        string[] tags = new[] { "Tag1", "Tag2", "Tag3", "Tag4", "Tag5" };

        if (string.IsNullOrWhiteSpace(response))
            return (title, tags);

        try
        {
            dynamic jsonResponse = JsonConvert.DeserializeObject(response);

            // Extract the actual text response from Gemini
            string? textResponse = jsonResponse?.candidates?[0]?.content?.parts?[0]?.text;

            if (!string.IsNullOrEmpty(textResponse))
            {
                // Try parsing the text as a JSON object
                var aiData = JsonConvert.DeserializeObject<Dictionary<string, object>>(textResponse);

                if (aiData != null && aiData.ContainsKey("title") && aiData.ContainsKey("tags"))
                {
                    title = aiData["title"]?.ToString() ?? title;
                    tags = ((JArray)aiData["tags"]).Select(t => t.ToString()).ToArray();
                }
            }
        }
        catch (Exception ex)
        {
            // Log or ignore
        }

        return (title, tags);
    }


}