using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.Attempt.Dtos
{
    public abstract class QuestionView
    {
        public Guid QuestionId { get; set; }
        public string Prompt { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
        public string Feedback { get; set; }
        public bool CountsTowardsMarking { get; set; }

    }

    public class MultipleChoiceQuestionView : QuestionView
    {
        public List<string> CorrectAnswers { get; set; }

        public List<string> Options { get; set; }
    }

    public class FreeTextQuestionView : QuestionView
    {
        public string ExpectedAnswer { get; set; }

        public List<string> Keywords { get; set; }
    }

    public class SliderQuestionView : QuestionView
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public bool ReversePassingThreshold { get; set; }
        public int PassingThresholdValue { get; set; }
    }
}
