using System;

public class UserResponse
{
    public int ResponseId { get; set; }
    public int UserId { get; set; }
    public int QuestionId { get; set; }
    public int? AnswerId { get; set; }
    public DateTime ResponseTime { get; set; }

    public User User { get; set; }
    public Question Question { get; set; }
    public Answer Answer { get; set; }
}

