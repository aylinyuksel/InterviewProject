using System;

using Microsoft.AspNetCore.Mvc;
using InterviewAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterviewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly InterviewDbContext _context; // Veritabanı bağlantısı için context

        public QuestionsController(InterviewDbContext context) //Constructor ile context enjekte edilir
        {
            _context = context;
        }

        [HttpGet] // HTTP GET isteğiyle çalışır
        public async Task<IActionResult> GetQuestions() // API uç noktası: Yalnızca aktif soruları çeker
        {
            var questions = await _context.Questions
                .Where(q => q.IsActive) // Yalnızca aktif olan soruları al
                .Select(q => new { q.Id, q.QuestionText }) // Sadece Id ve metni alınır
                .ToListAsync(); // Listeye dönüştürülür

            return Ok(questions);
        }

    }
}

