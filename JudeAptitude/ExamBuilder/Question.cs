using JudeAptitude.ExamBuilder.Marking.Interfaces;
using JudeAptitude.ExamBuilder.Marking.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.ExamBuilder
{
    public abstract class Question
    {
        public Guid Id { get; }
        public string Prompt { get; set; }
        public bool IsMarked { get; set; }


        public IMarkingStrategy MarkingStrategy 
        { 
            get
            {
                return _markingStrategy;
            }
        }

        private IMarkingStrategy _markingStrategy { get; set; }
        
        public Question()
        {
            Id = Guid.NewGuid();
            _markingStrategy = new AllOrNothingStrategy();
        }
    }

    public class MultipleChoiceQuestion : Question
    {
        private IMarkingStrategy _markingStrategy { get; set; }


        public List<string> Options { get; set; }
        public List<string> CorrectAnswers { get; set; }


        public MultipleChoiceQuestion()
        {
            Options = new List<string>();
            CorrectAnswers = new List<string>();
        }

        public void SetToAllOrNothingMarking()
        {
            _markingStrategy = new AllOrNothingStrategy();
        }

        public void SetToPartialMarking()
        {
            _markingStrategy = new PartialCreditStrategy();
        }
    }


    public class FreeTextQuestion : Question
    {
        private IMarkingStrategy _markingStrategy { get; set; }


        public string ExpectedAnswer { get; set; }

        public List<string> Keywords { get; set; }

        public bool UseExactMatch { get; set; }

        public FreeTextQuestion()
        {
            Keywords = new List<string>();
            UseExactMatch = true;
            _markingStrategy = new FreeTextMarkingStrategy();
        }
    }



    public class SliderQuestion : Question
    {
        private IMarkingStrategy _markingStrategy { get; set; }

        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 10;

        public SliderQuestion()
        {
            _markingStrategy = new SliderThresholdStrategy();
        }

    }






}
