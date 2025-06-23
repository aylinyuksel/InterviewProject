using System;

public class Feedback
{
    public int FeedbackId { get; set; }
    public int UserId { get; set; }
    public int QuestionId { get; set; }
    public string FeedbackText { get; set; }
    public DateTime FeedbackDate { get; set; }

    public User User { get; set; }
    public Question Question { get; set; }
}

