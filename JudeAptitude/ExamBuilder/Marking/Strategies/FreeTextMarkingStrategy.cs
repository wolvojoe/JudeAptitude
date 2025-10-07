using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder.Marking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Strategies
{
    public class FreeTextMarkingStrategy : IMarkingStrategy
    {
        public decimal Evaluate(MultipleChoiceQuestion question, MultipleChoiceAnswer answer)
        {
            throw new NotImplementedException("This strategy is not for multiple choice questions.");
        }

        public decimal Evaluate(FreeTextQuestion question, FreeTextAnswer answer)
        {
            var response = answer.GivenText?.Trim().ToLower() ?? "";

            if (question.UseExactMatch)
            {
                return response == question.ExpectedAnswer.Trim().ToLower() ? 1.0m : 0.0m;
            }

            // Keyword match
            int matchedKeywords = question.Keywords.Count(k =>
                response.Contains(k.Trim().ToLower()));

            return matchedKeywords > 0m ? 1.0m : 0.0m;
        }

        public decimal Evaluate(SliderQuestion question, SliderAnswer answer)
        {
            throw new NotImplementedException("This strategy is not for slider questions.");
        }
    }
}
