using Microsoft.AspNetCore.Mvc;

namespace Smashing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private List<Student> posts;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
            posts = new List<Student>();
        }

        [HttpGet(Name = "GetAll")]
        public async Task<ActionResult<List<Student>>> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get");
            return Ok(posts);
        }

        [HttpGet("{id:guid}", Name = "GetById")]
        public async Task<ActionResult<Student?>> GetById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetById");

            return Ok(posts.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost(Name = "Post")]
        public async Task<ActionResult> Post(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Post");
            posts.Add(new Student()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Title = "Title",
                UserName = "Name",
            });
            return StatusCode(201);
        }
    }

    public class Student
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}