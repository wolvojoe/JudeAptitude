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
        public Guid ExamAttemptId { get; set; }

        public Exam Exam { get; set; }

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

            if (page.RandomiseQuestionOrder)
            {
                var randomGenerator = new Random();
                return page.Questions.OrderBy(x => randomGenerator.Next()).ToList();
            }

            return page.Questions;
        }



        #region Add Answers

        private Question GetQuestion(Guid questionId)
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
            var question = GetQuestion(questionId);

            Answers.RemoveAll(a => a.Question.Id == questionId);

            var givenAnswers = new List<string>
            {
                givenAnswer
            };

            Answers.Add(new Answer
            {
                Question = question,
                GivenAnswers = givenAnswers
            });
        }

        public void AddAnswer(Guid questionId, List<string> selectedAnswers)
        {
            var question = GetQuestion(questionId);

            Answers.RemoveAll(a => a.Question.Id == questionId);

            Answers.Add(new Answer
            {
                Question = question,
                GivenAnswers = selectedAnswers
            });
        }

        public void AddAnswer(Guid questionId, int sliderValue)
        {
            var question = GetQuestion(questionId);

            if (question is SliderQuestion sliderQuestion)
            {
                if (sliderValue < sliderQuestion.MinValue || sliderValue > sliderQuestion.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(sliderValue), $"Value must be between {sliderQuestion.MinValue} and {sliderQuestion.MaxValue}.");
            }
            else
            {
                throw new InvalidOperationException("Question is not a slider question.");
            }

            Answers.RemoveAll(a => a.Question.Id == questionId);
            Answers.Add(new Answer
            {
                Question = question,
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
                _result.Mark = null;

                return _result;
            }

            decimal totalMark = 0;

            var allQuestionsCountingTowardsMark = Exam.AllQuestionsCountingTowardsMark();

            foreach (var answer in Answers)
            {
                var question = allQuestionsCountingTowardsMark.First(q => q.Id == answer.Question.Id);

                decimal questionMark = 0m;

                questionMark = question.MarkingStrategy.Evaluate(mcq, answer);


                if (question is MultipleChoiceQuestion mcq && question.MarkingStrategy != null)
                {
                    questionMark = question.MarkingStrategy.Evaluate(mcq, answer);
                }
                else if (question is FreeTextQuestion ftq && question.MarkingStrategy is FreeTextMarkingStrategy ftStrategy)
                {
                    questionMark = ftStrategy.Evaluate(ftq, answer);
                }
                else if (question is SliderQuestion sq && question.MarkingStrategy is SliderThresholdStrategy sqStrategy)
                {
                    questionMark = sqStrategy.Evaluate(sq, answer);
                }

                answer.Mark = questionMark;
                totalMark += questionMark;
            }

            _result.Mark = (int)Math.Round(totalMark);
            _result.MaximumPossibleMark = Exam.MaximumPossibleMark();
            _result.Answers = Answers;

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

        public decimal? Mark { get; set; }
        public decimal? MaximumPossibleMark { get; set; }

        public List<Answer> Answers { get; set; }
    }
}
