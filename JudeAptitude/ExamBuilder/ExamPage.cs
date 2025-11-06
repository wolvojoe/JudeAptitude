using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.ExamBuilder
{
    [Serializable]
    public class ExamPage
    {
        public Guid Id { get; }

        public string Title { get; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }
        public bool RandomiseQuestionOrder { get; set; }
        public int Order { get; set; }


        public ExamPage(string title)
        {
            Title = title;
            Id = Guid.NewGuid();
            Questions = new List<Question>();
            RandomiseQuestionOrder = false;
            Order = 1;
        }

        public decimal MaximumPossibleMark()
        {
            decimal maxMark = 0.0m;

            foreach (var question in Questions)
            {
                maxMark += question.MaximumPossibleMark();
            }

            return maxMark;
        }
    }
}
