using System;
using System.Collections.Generic;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    public ICollection<Question> Questions { get; set; }
}

