using System;
using System.Collections.Generic;

public class Difficulty
{
    public int DifficultyId { get; set; }
    public string DifficultyName { get; set; }

    public ICollection<Question> Questions { get; set; }
}

