using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JudeAptitude.ExamBuilder
{
    public class Exam
    {
        public Guid Id { get; }
        public string Title { get; }


        public string Description { get; set; }
        public string Subject { get; set; }
        public List<string> Tags { get; set; }
        public DifficultyLevel Difficulty { get; set; }


        public bool IsMarked { get; }
        public List<Page> Pages { get; set; }
        public decimal _passingMark { get; set; }

        public Exam(string title, bool isMarked)
        {
            Id = Guid.NewGuid();
            Title = title;
            IsMarked = isMarked;

            Pages = new List<Page>();
            Tags = new List<string>();
            Difficulty = DifficultyLevel.NotSpecified;

            _passingMark = 0.7m;
        }

        #region Validation

        public ValidationResult ValidateExam()
        {
            var validationErrors = new List<string>();

            var examHasPages = Pages != null && Pages.Count > 0;
            var examHasQuestions = AllQuestions().Count > 0;

            var examHasMarkedQuestions = false;

            if (IsMarked)
            {
                examHasMarkedQuestions = AllQuestionsCountingTowardsMark().Count > 0;

                if (examHasMarkedQuestions == true)
                {
                    validationErrors.AddRange(ValidateQuestionsCountingTowardsMark());
                }
                else
                {
                    validationErrors.Add("Exam is Marked but has no Questions that count towards Mark");
                }
            }

            if (examHasPages == false)
            {
                validationErrors.Add("Exam has no Pages");           
            }

            if (examHasQuestions == false)
            {
                validationErrors.Add("Exam has no Questions");
            }

            if (validationErrors.Count() > 0)
            {
                return ValidationResult.Invalid(validationErrors);
            }

            return ValidationResult.Valid();
        }

        private List<string> ValidateQuestionsCountingTowardsMark()
        {
            var validationErrors = new List<string>();

            var questionsCountingTowardsMark = AllQuestionsCountingTowardsMark();

            foreach(var question in questionsCountingTowardsMark)
            {
                var result = question.ValidateQuestion();
                if (!result.IsValid)
                {
                    validationErrors.AddRange(result.Errors);
                }
            }

            return validationErrors;
        }

        #endregion

        public List<Question> AllQuestionsCountingTowardsMark()
        {
            return Pages.SelectMany(p => p.Questions).Where(x => x.CountsTowardsMarking).ToList();
        }

        public List<Question> AllQuestions()
        {
            return Pages.SelectMany(p => p.Questions).ToList();
        }

        public bool SetPassingMark(decimal passingMark)
        {
            if (passingMark < 0.0m || passingMark > 1.0m)
            {
                return false;
            }

            _passingMark = passingMark;
            return true;
        }

        public decimal PassingMark()
        {
            var maxMark = MaximumPossibleMark();

            return maxMark * _passingMark;
        }

        public decimal MaximumPossibleMark()
        {
            decimal maxMark = 0.0m;

            foreach (var page in Pages)
            {
                maxMark += page.MaximumPossibleMark();
            }

            return maxMark;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }

        private ValidationResult(bool isValid, List<string> errors)
        {
            IsValid = isValid;
            Errors = errors;
        }

        public static ValidationResult Valid() => new ValidationResult(true, new List<string>());
        public static ValidationResult Invalid(List<string> errors) => new ValidationResult(false, errors);
    }

    public enum DifficultyLevel
    {
        NotSpecified,
        Easy,
        Medium,
        Hard
    }
}
