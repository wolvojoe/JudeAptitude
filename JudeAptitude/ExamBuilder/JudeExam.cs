using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JudeAptitude.ExamBuilder
{
    public class JudeExam
    {
        public Guid Id { get; }
        public string Title { get; }


        public string Description { get; set; }
        public string Subject { get; set; }
        public List<string> Tags { get; set; }
        public DifficultyLevel Difficulty { get; set; }


        public bool IsMarked { get; }
        public bool RandomisePageOrder { get; set; }

        public List<ExamPage> Pages { get; set; }

        public decimal PassingMarkPercentage
        {
            get
            {
                return _passingMarkPercentage;
            }
        }

        private decimal _passingMarkPercentage { get; set; }

        public JudeExam(string title, bool isMarked)
        {
            Id = Guid.NewGuid();
            Title = title;
            IsMarked = isMarked;

            Pages = new List<ExamPage>();
            Tags = new List<string>();
            Difficulty = DifficultyLevel.NotSpecified;

            _passingMarkPercentage = 0.7m;
            RandomisePageOrder = false;
        }

        /// <summary>
        /// Validate that the Exam can be Attempted
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all Questions that count towards Marking
        /// </summary>
        /// <returns></returns>
        public List<Question> AllQuestionsCountingTowardsMark()
        {
            return Pages.SelectMany(p => p.Questions).Where(x => x.CountsTowardsMarking).ToList();
        }

        /// <summary>
        /// Get all Questions
        /// </summary>
        /// <returns></returns>
        public List<Question> AllQuestions()
        {
            return Pages.SelectMany(p => p.Questions).ToList();
        }

        /// <summary>
        /// Set the Passing Mark Percentage for the Exam
        /// Must be between 0.0 and 1.0
        /// </summary>
        /// <param name="passingMark"></param>
        /// <returns></returns>
        public bool SetPassingMarkPercentage(decimal passingMark)
        {
            if (passingMark < 0.0m || passingMark > 1.0m)
            {
                return false;
            }

            _passingMarkPercentage = passingMark;
            return true;
        }

        /// <summary>
        /// Get the Total Passing Mark for the Exam
        /// </summary>
        /// <returns></returns>
        public decimal PassingMarkTotal()
        {
            var maxMark = MaximumPossibleMark();

            return maxMark * _passingMarkPercentage;
        }

        /// <summary>
        /// Maximum Possible Mark for the Exam
        /// </summary>
        /// <returns></returns>
        public decimal MaximumPossibleMark()
        {
            decimal maxMark = 0.0m;

            foreach (var page in Pages)
            {
                maxMark += page.MaximumPossibleMark();
            }

            return maxMark;
        }



        private List<string> ValidateQuestionsCountingTowardsMark()
        {
            var validationErrors = new List<string>();

            var questionsCountingTowardsMark = AllQuestionsCountingTowardsMark();

            foreach (var question in questionsCountingTowardsMark)
            {
                var result = question.ValidateQuestion();
                if (!result.IsValid)
                {
                    validationErrors.AddRange(result.Errors);
                }
            }

            return validationErrors;
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
