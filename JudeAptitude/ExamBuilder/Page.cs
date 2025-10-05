using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.ExamBuilder
{
    public class Page
    {
        public Guid PageId { get; }

        public string Title { get; }

        public string Description { get; set; }

        public List<Question> Questions { get; set; }

        public Page(string title)
        {
            Title = title;
            PageId = Guid.NewGuid();
            Questions = new List<Question>();
        }
    }
}
