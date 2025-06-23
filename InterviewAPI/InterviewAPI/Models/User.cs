namespace InterviewAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserSurname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ICollection<UserResponse> Responses { get; set; } = new List<UserResponse>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
