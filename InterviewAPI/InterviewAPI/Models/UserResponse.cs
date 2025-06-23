using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewAPI.Models
{
    public class UserResponse
    {
        [Key] // <- Bu satırı ekle
        public int ResponseId { get; set; }

        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public DateTime ResponseTime { get; set; }

        public User User { get; set; }
        public Question Question { get; set; }
        public Answer Answer { get; set; }
    }
}
