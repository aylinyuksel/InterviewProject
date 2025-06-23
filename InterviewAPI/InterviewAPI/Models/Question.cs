using System.Collections.Generic;
using InterviewAPI.Models;
public class Question
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int DifficultyId { get; set; }
    public string QuestionText { get; set; }
    public bool IsActive { get; set; }

    public Category Category { get; set; }
    public Difficulty Difficulty { get; set; }
    public ICollection<Answer> Answers { get; set; }
    public ICollection<UserResponse> UserResponses { get; set; }
    public ICollection<Feedback> Feedbacks { get; set; }
}

