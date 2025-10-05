using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking
{
    public class Answer
    {
        public Guid QuestionId { get; set; }
        public List<string> GivenAnswers { get; set; }
        public string GivenText { get; set; }
        public int? GivenNumber { get; set; }

        public decimal Score { get; set; }

        public Answer() 
        {
            GivenAnswers = new List<string>();
            GivenText = null;
        }

    }
}
