using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder.Marking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Strategies
{
    public class AllOrNothingStrategy : IMarkingStrategy
    {
        public decimal Evaluate(MultipleChoiceQuestion question, Answer answer)
        {
            var correct = question.CorrectAnswers.OrderBy(x => x, StringComparer.OrdinalIgnoreCase);
            var given = answer.GivenAnswers.OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

            return correct.SequenceEqual(given) ? 1.0m : 0.0m;
        }

        public decimal Evaluate(FreeTextQuestion question, Answer answer)
        {
            throw new NotImplementedException("This strategy is not for free text questions.");
        }

        public decimal Evaluate(SliderQuestion question, Answer answer)
        {
            throw new NotImplementedException("This strategy is not for slider questions.");
        }
    }
}
