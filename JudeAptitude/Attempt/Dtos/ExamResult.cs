using System;
using System.Collections.Generic;
using System.Text;

namespace JudeAptitude.Attempt.Dtos
{

    public class ExamResult
    {
        public Guid ExamId { get; set; }
        public Guid ExamAttemptId { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public ExamStatus ExamStatus { get; set; }
        public decimal? Mark { get; set; }
        public decimal? MaximumPossibleMark { get; set; }
        public decimal? PassingMark { get; set; }

        public List<Answer> Answers { get; set; }
    }
}
