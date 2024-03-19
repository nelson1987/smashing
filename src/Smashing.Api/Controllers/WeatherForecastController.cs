using Microsoft.AspNetCore.Mvc;

namespace Smashing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private List<Post> posts;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            posts = new List<Post>();
        }

        [HttpGet(Name = "GetAll")]
        public async Task<List<Post>> Get()
        {
            return await Task.FromResult(posts);
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<Post?> GetById(Guid id)
        {
            return await Task.FromResult(posts.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost(Name = "GetById")]
        public async Task Post()
        {
            posts.Add(new Controllers.Post()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Title = "Title",
                UserName = "Name",
            });
            await Task.CompletedTask;
        }
    }

    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}