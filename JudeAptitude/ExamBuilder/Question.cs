using JudeAptitude.ExamBuilder.Marking.Interfaces;
using JudeAptitude.ExamBuilder.Marking.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudeAptitude.ExamBuilder
{
    public abstract class Question
    {
        public Guid Id { get; }
        public string Prompt { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }

        public bool CountsTowardsMarking { get; set; }

        protected IMarkingStrategy _markingStrategy;
        public IMarkingStrategy MarkingStrategy => _markingStrategy;


        public Question()
        {
            Id = Guid.NewGuid();
            CountsTowardsMarking = true;
        }

        public abstract ValidationResult ValidateQuestion();

        public abstract decimal MaximumPossibleMark();
    }

    public class MultipleChoiceQuestion : Question
    {


        public List<string> Options { get; set; }
        public List<string> CorrectAnswers { get; set; }


        public MultipleChoiceQuestion()
        {
            Options = new List<string>();
            CorrectAnswers = new List<string>();
            SetToAllOrNothingMarking();
        }

        public void SetToAllOrNothingMarking()
        {
            _markingStrategy = new AllOrNothingStrategy();
        }

        public void SetToPartialMarking()
        {
            _markingStrategy = new PartialCreditStrategy();
        }

        public override ValidationResult ValidateQuestion()
        {
            var validationErrors = new List<string>();

            if (Options.Count < 2)
            {
                validationErrors.Add($"A multiple choice question must have at least 2 options. {Id}");
            }

            if (CountsTowardsMarking)
            {
                if (CorrectAnswers.Count < 1)
                {
                    validationErrors.Add($"A multiple choice question must have at least 1 correct answer. {Id}");
                }

                foreach (var correctAnswer in CorrectAnswers)
                {
                    if (Options.Where(x => x == correctAnswer).Count() == 0)
                    {
                        validationErrors.Add($"Correct answer '{correctAnswer}' is not in the list of options. {Id}");
                    }
                }
            }

            return validationErrors.Count == 0 ? ValidationResult.Valid() : ValidationResult.Invalid(validationErrors);
        }

        public override decimal MaximumPossibleMark()
        {
            if (CountsTowardsMarking)
            {
                if (_markingStrategy is PartialCreditStrategy)
                {
                    return CorrectAnswers.Distinct().Count() * 1.0m;
                }
                else
                {
                    return 1m;
                }
            }

            return 0m;
        }
    }


    public class FreeTextQuestion : Question
    {
        public string ExpectedAnswer { get; set; }

        public List<string> Keywords { get; set; }

        public bool UseExactMatch { get; set; }

        public FreeTextQuestion()
        {
            Keywords = new List<string>();
            UseExactMatch = true;
            _markingStrategy = new FreeTextMarkingStrategy();
        }


        public override ValidationResult ValidateQuestion()
        {
            var validationErrors = new List<string>();

            if (CountsTowardsMarking)
            {
                if (UseExactMatch == true && ExpectedAnswer.Length == 0)
                {
                    validationErrors.Add($"A Free Text question needs an Expected Answer {Id}");
                }

                if (UseExactMatch == false && Keywords.Count() == 0)
                {
                    validationErrors.Add($"A Free Text question needs Expected Keywords {Id}");
                }
            }

            return validationErrors.Count == 0 ? ValidationResult.Valid() : ValidationResult.Invalid(validationErrors);
        }


        public override decimal MaximumPossibleMark()
        {
            if (CountsTowardsMarking)
            {
                return 1m;
            }

            return 0m;
        }
    }



    public class SliderQuestion : Question
    {
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 10;

        public bool ReversePassingThreshold { get; set; } = false;
        public int PassingThresholdValue { get; set; } = 7;

        public SliderQuestion()
        {
            _markingStrategy = new SliderThresholdStrategy();
        }

        public override ValidationResult ValidateQuestion()
        {
            var validationErrors = new List<string>();

            if (MinValue >= MaxValue)
            {
                validationErrors.Add($"A Slider question needs a Min Value less than the Max Value {Id}");
            }

            if (PassingThresholdValue > MaxValue || PassingThresholdValue < MinValue)
            {
                validationErrors.Add($"A Slider question needs a valid Passing Threshold {Id}");
            }

            return validationErrors.Count == 0 ? ValidationResult.Valid() : ValidationResult.Invalid(validationErrors);
        }

        public override decimal MaximumPossibleMark()
        {
            if (CountsTowardsMarking)
            {
                return 1m;
            }

            return 0m;
        }
    }

}
