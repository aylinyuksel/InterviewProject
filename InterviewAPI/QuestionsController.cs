using InterviewAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterviewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly InterviewDbContext _context;

        public QuestionsController(InterviewDbContext context)
        {
            _context = context;
        }

        // GET: api/questions
        [HttpGet]
        public async Task<IActionResult> GetActiveQuestions()
        {
            var questions = await _context.Questions
                .Where(q => q.IsActive)
                .OrderBy(q => Guid.NewGuid()) // rastgele sırala
                .Take(4)                      // ilk 4 tanesini al
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText
                })
                .ToListAsync();

            return Ok(questions);
        }
    }
}
