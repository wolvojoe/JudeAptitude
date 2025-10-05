using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JudeAptitude.ExamBuilder;
using JudeAptitude.ExamBuilder.Marking;
using JudeAptitude.ExamBuilder.Marking.Interfaces;
using JudeAptitude.ExamBuilder.Marking.Strategies;

namespace JudeAptitude.Attempt
{
    public class ExamAttempt
    {
        public Guid ExamAttemptId { get; set; }
        public Exam Exam { get; set; }

        public List<Guid> ExamPageIds;
        public Guid CurrentPageId;
        public bool IsCompleted { get; set; }
        public List<Answer> Answers { get; }



        private ExamResult _result { get; set; }

        private int _currentPageIndex = 0;

        public ExamAttempt(Exam exam)
        {
            Exam = exam;

            if (exam.ValidateExam().IsValid == false)
            {
                throw new InvalidOperationException("Exam is not valid to attempt. " + string.Join(", ", exam.ValidateExam().Errors));
            }

            ExamAttemptId = Guid.NewGuid();
            Answers = new List<Answer>();

            IsCompleted = false;

            _result = new ExamResult
            {
                ExamId = exam.Id,
                ExamAttemptId = ExamAttemptId,
                StartedDate = DateTime.UtcNow,
                Answers = Answers
            };
        }

        public List<Question> GetQuestionsOnCurrentPage()
        {
            var page = GetCurrentPage();
            return page.Questions;
        }


        #region Add Answers

        public void AddAnswer(Guid questionId, string givenAnswer)
        {
            var question = Exam.AllQuestions().FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                throw new InvalidOperationException("Question not found in exam.");

            Answers.RemoveAll(a => a.QuestionId == questionId);

            var givenAnswers = new List<string>
            {
                givenAnswer
            };

            Answers.Add(new Answer
            {
                QuestionId = questionId,
                GivenAnswers = givenAnswers
            });
        }

        public void AddAnswer(Guid questionId, List<string> selectedAnswers)
        {
            var question = Exam.AllQuestions().FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                throw new InvalidOperationException("Question not found.");

            Answers.RemoveAll(a => a.QuestionId == questionId);

            Answers.Add(new Answer
            {
                QuestionId = questionId,
                GivenAnswers = selectedAnswers
            });
        }

        public void AddAnswer(Guid questionId, int sliderValue)
        {
            var question = Exam.AllQuestions().FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                throw new InvalidOperationException("Question not found.");

            if (question is SliderQuestion sliderQuestion)
            {
                if (sliderValue < sliderQuestion.MinValue || sliderValue > sliderQuestion.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(sliderValue), $"Value must be between {sliderQuestion.MinValue} and {sliderQuestion.MaxValue}.");
            }
            else
            {
                throw new InvalidOperationException("Question is not a slider question.");
            }

            Answers.RemoveAll(a => a.QuestionId == questionId);
            Answers.Add(new Answer
            {
                QuestionId = questionId,
                GivenNumber = sliderValue
            });
        }

        #endregion



        public ExamResult Submit()
        {
            IsCompleted = true;
            _result.SubmittedDate = DateTime.UtcNow;

            if (!Exam.IsMarked)
            {
                _result.Score = null;

                return _result;
            }

            decimal totalScore = 0;

            foreach (var answer in Answers)
            {
                var question = Exam.GetAllQuestions()
                                   .FirstOrDefault(q => q.Id == answer.QuestionId);

                decimal questionScore = 0m;

                if (question is MultipleChoiceQuestion mcq && question.MarkingStrategy != null)
                {
                    questionScore = question.MarkingStrategy.Evaluate(mcq, answer);
                }
                else if (question is FreeTextQuestion ftq && question.MarkingStrategy is FreeTextMarkingStrategy ftStrategy)
                {
                    questionScore = ftStrategy.Evaluate(ftq, answer);
                }

                answer.Score = questionScore;
                totalScore += questionScore;
            }

            _result.Score = (int)Math.Round(totalScore);

            return _result;
        }



        #region Paging

        public Page GetCurrentPage()
        {
            if (_currentPageIndex < 0 || _currentPageIndex >= Exam.Pages.Count)
                throw new InvalidOperationException("Current page index is out of range.");

            return Exam.Pages[_currentPageIndex];
        }

        public Page NextPage()
        {
            if (_currentPageIndex < Exam.Pages.Count - 1)
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
        public DateTime SubmittedDate { get; set; }

        public decimal? Score { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
