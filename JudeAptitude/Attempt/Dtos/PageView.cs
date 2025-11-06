using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.Attempt.Dtos
{
    public class PageView
    {
        public string Title { get; }
        public string Description { get; }
        public int QuestionsCount { get; }

        public PageView(string title, string description, int questionCount)
        {
            Title = title;
            Description = description;
            QuestionsCount = questionCount;
        }
    }


    public class PageOrder
    {
        public Guid PageId { get; set; }
        public int Order { get; set; }
        public bool IsCurrentPage { get; set; }
    }
}
