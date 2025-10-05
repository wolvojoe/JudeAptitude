using JudeAptitude.ExamBuilder.Marking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Strategies
{
    public class PartialCreditStrategy : IMarkingStrategy
    {
        private readonly decimal _pointPerCorrect = 1.0m;
        private readonly decimal _penaltyPerIncorrect = 0.5m;

        public PartialCreditStrategy(decimal pointPerCorrect = 1.0m, decimal penaltyPerIncorrect = 0.5m)
        {
            _pointPerCorrect = pointPerCorrect;
            _penaltyPerIncorrect = penaltyPerIncorrect;
        }

        public decimal Evaluate(MultipleChoiceQuestion question, Answer answer)
        {
            var correctSet = new HashSet<string>(question.CorrectAnswers, StringComparer.OrdinalIgnoreCase);
            var givenSet = new HashSet<string>(answer.GivenAnswers, StringComparer.OrdinalIgnoreCase);

            int correctCount = givenSet.Count(g => correctSet.Contains(g));
            int incorrectCount = givenSet.Count(g => !correctSet.Contains(g));

            decimal score = correctCount * _pointPerCorrect - incorrectCount * _penaltyPerIncorrect;

            return Math.Max(score, 0.0m);
        }
    }
}
