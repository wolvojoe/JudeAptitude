using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.ExamBuilder.Marking.Interfaces
{
    public interface IMarkingStrategy
    {
        decimal Evaluate(MultipleChoiceQuestion question, Answer answer);
    }
}
