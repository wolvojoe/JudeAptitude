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

        public bool IsMarked { get; }

        public List<Page> Pages { get; set; }


        public Exam(string title, bool isMarked)
        {
            Id = Guid.NewGuid();
            Title = title;
            IsMarked = isMarked;

            Pages = new List<Page>();
        }

        public ValidationResult ValidateExam()
        {
            var validationErrors = new List<string>();

            var examHasPages = Pages != null && Pages.Count > 0;
            var examHasQuestions = Questions().Count > 0;

            var examHasMarkedQuestions = false;

            if (IsMarked)
            {
                examHasMarkedQuestions = QuestionsCountingTowardsMark().Count > 0;
            }

            if (examHasPages == false)
            {
                validationErrors.Add("Exam has no Pages");           
            }

            if (examHasQuestions == false)
            {
                validationErrors.Add("Exam has no Questions");
            }

            if (IsMarked && examHasMarkedQuestions == false)
            {
                validationErrors.Add("Exam is Marked but has no Questions that count towards Mark");
            }

            return ValidationResult.Valid();
        }


        public List<Question> AllQuestionsCountingTowardsMark()
        {
            return Pages.SelectMany(p => p.Questions).Where(x => x.CountsTowardsMarking).ToList();
        }

        public List<Question> AllQuestions()
        {
            return Pages.SelectMany(p => p.Questions).ToList();
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
}
