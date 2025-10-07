using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder.Marking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Strategies
{
    public class SliderThresholdStrategy : IMarkingStrategy
    {
        public SliderThresholdStrategy()
        {

        }

        public decimal Evaluate(MultipleChoiceQuestion question, MultipleChoiceAnswer answer)
        {
            throw new NotImplementedException("This strategy is not for multiple choice questions.");
        }

        public decimal Evaluate(FreeTextQuestion question, FreeTextAnswer answer)
        {
            throw new NotImplementedException("This strategy is not for free text questions.");
        }

        public decimal Evaluate(SliderQuestion question, SliderAnswer answer)
        {
            var answerValue = answer.GivenNumber;

            if (question.ReversePassingThreshold == false && answer.GivenNumber >= question.PassingThresholdValue)
            {
                return 1.0m;
            }
            else if (question.ReversePassingThreshold == true && answer.GivenNumber <= question.PassingThresholdValue)
            {
                return 1.0m;
            }

            return 0.0m;
        }

    }
}
