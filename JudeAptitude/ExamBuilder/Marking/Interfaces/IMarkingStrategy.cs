using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Interfaces
{
    public interface IMarkingStrategy
    {
        decimal Evaluate(MultipleChoiceQuestion question, MultipleChoiceAnswer answer);

        decimal Evaluate(FreeTextQuestion question, FreeTextAnswer answer);

        decimal Evaluate(SliderQuestion question, SliderAnswer answer);
    }
}
