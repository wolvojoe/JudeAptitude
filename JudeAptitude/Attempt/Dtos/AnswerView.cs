using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.Attempt.Dtos
{
    [Serializable]
    public abstract class AnswerView
    {
        public Guid QuestionId { get; set; }
        public decimal Mark { get; set; }
    }

    [Serializable]
    public class MultipleChoiceAnswerView : AnswerView
    {
        public List<string> GivenAnswers { get; set; }
    }

    [Serializable]
    public class FreeTextAnswerView : AnswerView
    {
        public string GivenText { get; set; }
    }

    [Serializable]
    public class SliderAnswerView : AnswerView
    {
        public int GivenNumber { get; set; }
    }
}
