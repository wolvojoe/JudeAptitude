using JudeAptitude.ExamBuilder;
using JudeAptitude.ExamBuilder.Marking.Interfaces;
using JudeAptitude.ExamBuilder.Marking.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JudeAptitude.Attempt
{
    public class ExamAttempt
    {
        public Guid ExamAttemptId { get; }


        private Exam _exam { get; set; }
        private List<Answer> _answers { get; }
        private ExamResult _result { get; set; }
        private int _currentPageIndex = 0;

        public ExamAttempt(Exam exam)
        {
            ValidateExam(exam);

            _exam = exam;

            ExamAttemptId = Guid.NewGuid();

            _answers = new List<Answer>();

            _result = new ExamResult
            {
                ExamId = exam.Id,
                ExamAttemptId = ExamAttemptId,
                StartedDate = DateTime.UtcNow,
                SubmittedDate = null
            };
        }

        private bool ValidateExam(Exam exam)
        {
            var examValidation = exam.ValidateExam();

            if (examValidation.IsValid == false)
            {
                throw new InvalidOperationException("Exam is not valid to attempt. " + string.Join(", ", examValidation.Errors));
            }

            return true;
        }



        public List<Question> GetQuestionsOnCurrentPage()
        {
            var page = GetCurrentPage();

            if (page.RandomiseQuestionOrder)
            {
                var randomGenerator = new Random();
                return page.Questions.OrderBy(x => randomGenerator.Next()).ToList();
            }

            return page.Questions;
        }


        // Add Answers need validation for question types
        #region Add Answers

        private Question GetQuestionForCurrentPage(Guid questionId)
        {
            var question = GetCurrentPage().Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
            {
                throw new InvalidOperationException($"Question {questionId} not found on Page.");
            }

            return question;
        }

        public void AddAnswer(Guid questionId, string givenAnswer)
        {
            var question = GetQuestionForCurrentPage(questionId);

            _answers.RemoveAll(a => a.Question.Id == questionId);

            var givenAnswers = new List<string>
            {
                givenAnswer
            };

            _answers.Add(new Answer
            {
                Question = question,
                GivenAnswers = givenAnswers
            });
        }

        public void AddAnswer(Guid questionId, List<string> selectedAnswers)
        {
            var question = GetQuestionForCurrentPage(questionId);

            _answers.RemoveAll(a => a.Question.Id == questionId);

            _answers.Add(new Answer
            {
                Question = question,
                GivenAnswers = selectedAnswers
            });
        }

        public void AddAnswer(Guid questionId, int sliderValue)
        {
            var question = GetQuestionForCurrentPage(questionId);

            if (question is SliderQuestion sliderQuestion)
            {
                if (sliderValue < sliderQuestion.MinValue || sliderValue > sliderQuestion.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(sliderValue), $"Value must be between {sliderQuestion.MinValue} and {sliderQuestion.MaxValue}.");
            }
            else
            {
                throw new InvalidOperationException("Question is not a slider question.");
            }

            _answers.RemoveAll(a => a.Question.Id == questionId);
            _answers.Add(new Answer
            {
                Question = question,
                GivenNumber = sliderValue
            });
        }

        #endregion



        #region Marking

        public ExamResult Submit()
        {
            _result.SubmittedDate = DateTime.UtcNow;

            if (!_exam.IsMarked)
            {
                _result.Mark = null;

                return _result;
            }

            decimal totalMark = 0;

            var allQuestionsCountingTowardsMark = _exam.AllQuestionsCountingTowardsMark();

            foreach (var answer in _answers)
            {
                var question = allQuestionsCountingTowardsMark.First(q => q.Id == answer.Question.Id);

                decimal questionMark = answer.Mark();

                totalMark += questionMark;
            }

            _result.Mark = (int)Math.Round(totalMark);
            _result.MaximumPossibleMark = _exam.MaximumPossibleMark();

            return _result;
        }

        private List<QuestionSummary> Generate

        #endregion


        #region Paging

        public Page GetCurrentPage()
        {
            if (_currentPageIndex < 0 || _currentPageIndex >= _exam.Pages.Count)
                throw new InvalidOperationException("Current page index is out of range.");

            return _exam.Pages[_currentPageIndex];
        }

        public Page NextPage()
        {
            if (_currentPageIndex < _exam.Pages.Count - 1)
            {
                _currentPageIndex++;
                return GetCurrentPage();
            }
            return null;
        }

        public Page PreviousPage()
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--;
                return GetCurrentPage();
            }
            return null;
        }

        #endregion


    }

    public class ExamResult
    {
        public Guid ExamId { get; set; }
        public Guid ExamAttemptId { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public decimal? Mark { get; set; }
        public decimal? MaximumPossibleMark { get; set; }

        public List<QuestionSummary> Answers { get; set; }
    }

    public class  ExamSummary
    {
        List<>
    }

    public class QuestionSummary
    {

    }
}
