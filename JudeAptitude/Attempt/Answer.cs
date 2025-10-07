using JudeAptitude.ExamBuilder;
using JudeAptitude.ExamBuilder.Marking.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.Attempt
{
    public abstract class Answer
    {
        public Guid QuestionId { get; set; }
        public decimal Mark { get; set; }
    }

    public class MultipleChoiceAnswer : Answer
    {
        public List<string> GivenAnswers { get; set; }
    }

    public class FreeTextAnswer : Answer
    {
        public string GivenText { get; set; }
    }

    public class SliderAnswer : Answer
    {
        public int GivenNumber { get; set; }
    }
}
