using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.Attempt.Dtos
{
    public abstract class AnswerView
    {
        public Guid QuestionId { get; set; }
        public decimal Mark { get; set; }
    }

    public class MultipleChoiceAnswerView : AnswerView
    {
        public List<string> GivenAnswers { get; set; }
    }

    public class FreeTextAnswerView : AnswerView
    {
        public string GivenText { get; set; }
    }

    public class SliderAnswerView : AnswerView
    {
        public int GivenNumber { get; set; }
    }
}
