using JudeAptitude.ExamBuilder;
using JudeAptitude.ExamBuilder.Marking.Strategies;
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

        public Answer()
        {
            GivenAnswers = new List<string>();
            GivenText = null;
        }

        public decimal Mark()
        {
            if (Question is MultipleChoiceQuestion mcq && Question.MarkingStrategy is AllOrNothingStrategy allStrategy)
            {
                return allStrategy.Evaluate(mcq, this);
            }
            else if (Question is FreeTextQuestion ftq && Question.MarkingStrategy is FreeTextMarkingStrategy ftStrategy)
            {
                return ftStrategy.Evaluate(ftq, this);
            }
            else if (Question is SliderQuestion sq && Question.MarkingStrategy is SliderThresholdStrategy sqStrategy)
            {
                return sqStrategy.Evaluate(sq, this);
            }

            return 0m;
        }

    }
}
