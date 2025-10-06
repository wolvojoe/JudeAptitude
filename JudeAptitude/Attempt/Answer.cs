using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.Attempt
{
    public class Answer
    {
        public Question Question { get; set; }

        public List<string> GivenAnswers { get; set; }
        public string GivenText { get; set; }
        public int? GivenNumber { get; set; }

        public decimal Mark { get; set; }

        public Answer() 
        {
            GivenAnswers = new List<string>();
            GivenText = null;
            Mark = 0m;
        }

    }
}
