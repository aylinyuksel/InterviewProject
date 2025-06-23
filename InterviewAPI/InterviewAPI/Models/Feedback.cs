using System;

namespace InterviewAPI.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string FeedbackText { get; set; } = string.Empty;
        public DateTime FeedbackDate { get; set; }

        public User? User { get; set; }
        public Question? Question { get; set; }
    }
}
