using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder.Marking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Strategies
{

    [Serializable]
    public class PartialCreditStrategy : IMarkingStrategy
    {
        private readonly decimal _pointPerCorrect = 1.0m;
        private readonly decimal _penaltyPerIncorrect = 0.5m;

        public PartialCreditStrategy(decimal pointPerCorrect = 1.0m, decimal penaltyPerIncorrect = 0.5m)
        {
            _pointPerCorrect = pointPerCorrect;
            _penaltyPerIncorrect = penaltyPerIncorrect;
        }

        public decimal Evaluate(MultipleChoiceQuestion question, MultipleChoiceAnswer answer)
        {
            var correctSet = new HashSet<string>(question.CorrectAnswers, StringComparer.OrdinalIgnoreCase);
            var givenSet = new HashSet<string>(answer.GivenAnswers, StringComparer.OrdinalIgnoreCase);

            int correctCount = givenSet.Count(x => correctSet.Contains(x));
            int incorrectCount = givenSet.Count(x => !correctSet.Contains(x));

            decimal score = correctCount * _pointPerCorrect - incorrectCount * _penaltyPerIncorrect;

            return Math.Max(score, 0.0m);
        }

        public decimal Evaluate(FreeTextQuestion question, FreeTextAnswer answer)
        {
            throw new NotImplementedException("This strategy is not for free text questions.");
        }

        public decimal Evaluate(SliderQuestion question, SliderAnswer answer)
        {
            throw new NotImplementedException("This strategy is not for slider questions.");
        }
    }
}
