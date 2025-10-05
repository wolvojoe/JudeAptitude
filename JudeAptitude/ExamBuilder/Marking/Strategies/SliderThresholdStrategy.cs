using JudeAptitude.ExamBuilder.Marking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Strategies
{
    public class SliderThresholdStrategy : IMarkingStrategy
    {
        private int _threshold;

        private bool _isGreaterThan;

        public SliderThresholdStrategy(int minimumThreshold = 7, bool isGreaterThan = true)
        {
            _threshold = minimumThreshold;
            _isGreaterThan = isGreaterThan;
        }

        public decimal Evaluate(MultipleChoiceQuestion question, Answer answer)
        {
            throw new NotImplementedException("This strategy is not for multiple choice questions.");
        }

        public decimal Evaluate(FreeTextQuestion question, Answer answer)
        {
            throw new NotImplementedException("This strategy is not for free text questions.");
        }

        public decimal Evaluate(SliderQuestion question, Answer answer)
        {
            var answerValue = answer.GivenNumber;

            if (_isGreaterThan && answerValue >= _threshold)
            {
                return 1.0m;
            }
            else if (_isGreaterThan == false && answerValue <= _threshold)
            {
                return 1.0m;
            }

            return 0.0m;
        }

    }
}
