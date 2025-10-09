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
        private List<PageOrder> _pageOrder { get; set; }
        private Dictionary<Guid, int> _questionOrder { get; set; }


        public ExamAttempt(Exam exam)
        {
            ValidateExam(exam);

            _exam = exam;

            ExamAttemptId = Guid.NewGuid();

            _answers = new List<Answer>();
            _pageOrder = new List<PageOrder>();
            _questionOrder = new Dictionary<Guid, int>();

            _result = new ExamResult
            {
                ExamId = exam.Id,
                ExamAttemptId = ExamAttemptId,
                StartedDate = DateTime.UtcNow,
                SubmittedDate = null
            };

            SetPageOrder();
            SetQuestionOrder();
        }

        #region Public

        /// <summary>
        /// Submits an Answer for a Question on the current Page
        /// </summary>
        /// <param name="selectedAnswer"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SubmitAnswerForQuestionOnCurrentPage(Answer selectedAnswer)
        {
            if (ValidateAnswer(selectedAnswer) == false)
            {
                throw new InvalidOperationException("Answer is invalid.");
            }

            var question = GetQuestionOnCurrentPage(selectedAnswer.QuestionId);

            _answers.RemoveAll(a => a.QuestionId == selectedAnswer.QuestionId);

            _answers.Add(selectedAnswer);
        }

        /// <summary>
        /// Submits the Exam for Marking and returns an Exam Result
        /// </summary>
        /// <returns></returns>
        public ExamResult SubmitExamAttempt()
        {
            if (_result.SubmittedDate != null)
            {
                throw new InvalidOperationException("Exam Attempt has already been Submitted.");
            }

            if (!_exam.IsMarked)
            {
                _result.Mark = null;

                return _result;
            }

            var totalMark = 0m;

            var allQuestionsCountingTowardsMark = _exam.AllQuestionsCountingTowardsMark();

            foreach (var answer in _answers)
            {
                var question = allQuestionsCountingTowardsMark.First(q => q.Id == answer.QuestionId);

                var questionMark = GetAnswerMark(question, answer);

                answer.Mark = questionMark;
                totalMark += questionMark;
            }

            _result.SubmittedDate = DateTime.UtcNow;
            _result.Mark = (decimal)Math.Round(totalMark);
            _result.MaximumPossibleMark = _exam.MaximumPossibleMark();
            _result.PassingMark = _exam.PassingMarkTotal();
            _result.Answers = _answers;
            _result.ExamStatus = (_result.Mark >= _result.PassingMark) ? ExamStatus.Passed : ExamStatus.Failed;
            return _result;
        }

        /// <summary>
        /// Gets all Questions on the Exam Attempts current Page, ordered as specified by the Exam
        /// </summary>
        /// <returns></returns>
        public List<QuestionView> GetAllQuestionsOnCurrentPage()
        {
            var page = GetCurrentPageObject();

            return page.Questions.OrderBy(x => _questionOrder[x.Id]).Select(x => ToQuestionView(x)).ToList();
        }

        /// <summary>
        /// Gets the currently submitted Answer for a Question
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Answer GetCurrentAnswerForQuestion(Guid questionId)
        {
            return _answers.FirstOrDefault(a => a.QuestionId == questionId);
        }

        /// <summary>
        /// Gets the correct Answer for a Question
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Answer GetCorrectAnswerForQuestion(Guid questionId)
        {
            var question = _exam.AllQuestions().FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                return null;

            if (question is MultipleChoiceQuestion mcq)
            {
                return new MultipleChoiceAnswer
                {
                    QuestionId = questionId,
                    GivenAnswers = mcq.CorrectAnswers != null ? new List<string>(mcq.CorrectAnswers) : new List<string>()
                };
            }
            else if (question is FreeTextQuestion ftq)
            {
                return new FreeTextAnswer
                {
                    QuestionId = questionId,
                    GivenText = ftq.ExpectedAnswer
                };
            }
            else if (question is SliderQuestion sq)
            {
                return new SliderAnswer
                {
                    QuestionId = questionId,
                    GivenNumber = sq.PassingThresholdValue
                };
            }

            return null;
        }

        /// <summary>
        /// Gets the current Page details
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public PageView GetCurrentPage()
        {
            return ConvertPageToPageView(GetCurrentPageObject());
        }

        /// <summary>
        /// Moves the Exam Attempt to the next Page
        /// </summary>
        public void NavigateToNextPage()
        {
            var currentPage = GetCurrentPageObject();

            if (currentPage.Order < _pageOrder.Count)
            {
                _pageOrder.Where(x => x.PageId == currentPage.Id).First().IsCurrentPage = false;
                _pageOrder.Where(x => x.Order == currentPage.Order + 1).First().IsCurrentPage = true;
            }
        }

        /// <summary>
        /// Moves the Exam Attempt to the previous Page
        /// </summary>
        public void NavigateToPreviousPage()
        {
            var currentPage = GetCurrentPageObject();

            if (currentPage.Order > 1)
            {
                _pageOrder.Where(x => x.PageId == currentPage.Id).First().IsCurrentPage = false;
                _pageOrder.Where(x => x.Order == currentPage.Order - 1).First().IsCurrentPage = true;
            }
        }

        #endregion


        #region Private Methods

        private Page GetCurrentPageObject()
        {
            var currentPage = _exam.Pages.FirstOrDefault(x => x.Id == _pageOrder.FirstOrDefault(z => z.IsCurrentPage).PageId);

            if (currentPage is null)
                throw new InvalidOperationException("Current page is invalid.");

            return currentPage;
        }

        private PageView ConvertPageToPageView(Page selectedPage)
        {
            return new PageView(selectedPage.Title, selectedPage.Description, selectedPage.Questions.Count);
        }

        private void SetQuestionOrder()
        {
            foreach (var page in _exam.Pages)
            {
                if (page.RandomiseQuestionOrder)
                {
                    var randomGenerator = new Random();
                    var pageQuestionsRandomOrder = page.Questions.OrderBy(x => randomGenerator.Next()).ToList();
                    var orderCount = 1;

                    foreach (var question in pageQuestionsRandomOrder)
                    {
                        _questionOrder.Add(question.Id, orderCount);
                    }
                }
                else
                {
                    foreach (var question in page.Questions)
                    {
                        _questionOrder.Add(question.Id, question.Order);
                    }
                }
            }
        }

        private void SetPageOrder()
        {
            var pagesOrdered = new List<Page>();
            var orderCount = 1;

            if (_exam.RandomisePageOrder)
            {
                var randomGenerator = new Random();
                pagesOrdered = _exam.Pages.OrderBy(x => randomGenerator.Next(1, 5000)).ToList();
            }
            else
            {
                pagesOrdered = _exam.Pages.OrderBy(x => x.Order).ToList();
            }

            foreach (var page in pagesOrdered)
            {
                _pageOrder.Add(new PageOrder()
                {
                    PageId = page.Id,
                    Order = orderCount,
                    IsCurrentPage = _pageOrder.Count == 0 ? true : false
                });

                orderCount += 1;
            }
        }

        private decimal GetAnswerMark(Question question, Answer answer)
        {
            if (question == null || question.MarkingStrategy == null)
                return 0m;

            switch (question)
            {
                case MultipleChoiceQuestion mcq:
                    return question.MarkingStrategy.Evaluate(mcq, (MultipleChoiceAnswer)answer);
                case FreeTextQuestion ftq:
                    return question.MarkingStrategy.Evaluate(ftq, (FreeTextAnswer)answer);
                case SliderQuestion sq:
                    return question.MarkingStrategy.Evaluate(sq, (SliderAnswer)answer);
                default:
                    return 0m;
            }
        }

        private Question GetQuestionOnCurrentPage(Guid questionId)
        {
            var question = GetCurrentPageObject().Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
            {
                throw new InvalidOperationException($"Question {questionId} not found on Page.");
            }

            return question;
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

        private bool ValidateAnswer(Answer selectedAnswer)
        {
            var question = GetQuestionOnCurrentPage(selectedAnswer.QuestionId);

            if (question == null || selectedAnswer == null)
                return false;

            if (question is MultipleChoiceQuestion)
            {
                if (!(selectedAnswer is MultipleChoiceAnswer mcAnswer))
                    return false;

                if (mcAnswer.GivenAnswers == null || mcAnswer.GivenAnswers.Count == 0)
                    return false;

                var validOptions = ((MultipleChoiceQuestion)question).Options;
                if (mcAnswer.GivenAnswers.Any(a => !validOptions.Contains(a)))
                    return false;
            }
            else if (question is FreeTextQuestion)
            {
                if (!(selectedAnswer is FreeTextAnswer ftAnswer))
                    return false;

                if (string.IsNullOrWhiteSpace(ftAnswer.GivenText))
                    return false;
            }
            else if (question is SliderQuestion sliderQuestion)
            {
                if (!(selectedAnswer is SliderAnswer sAnswer))
                    return false;

                if (sAnswer.GivenNumber < sliderQuestion.MinValue || sAnswer.GivenNumber > sliderQuestion.MaxValue)
                    return false;
            }
            else
            {
                return false;
            }

            return true;
        }

        private QuestionView ToQuestionView(Question question)
        {
            if (question == null)
                return null;

            QuestionView view = null;

            if (question is MultipleChoiceQuestion mcq)
            {
                view = new MultipleChoiceQuestionView
                {
                    CorrectAnswers = mcq.CorrectAnswers != null ? new List<string>(mcq.CorrectAnswers) : new List<string>(),
                    Options = mcq.Options != null ? new List<string>(mcq.Options) : new List<string>()
                };
            }
            else if (question is FreeTextQuestion ftq)
            {
                view = new FreeTextQuestionView
                {
                    ExpectedAnswer = ftq.ExpectedAnswer,
                    Keywords = ftq.Keywords != null ? new List<string>(ftq.Keywords) : new List<string>()
                };
            }
            else if (question is SliderQuestion sq)
            {
                view = new SliderQuestionView
                {
                    MinValue = sq.MinValue,
                    MaxValue = sq.MaxValue,

                    PassingThresholdValue = sq.GetType().GetProperty("PassingThresholdValue") != null
                        ? (int)sq.GetType().GetProperty("PassingThresholdValue").GetValue(sq)
                        : 0,
                    ReversePassingThreshold = sq.GetType().GetProperty("ReversePassingThreshold") != null
                        ? (bool)sq.GetType().GetProperty("ReversePassingThreshold").GetValue(sq)
                        : false
                };
            }
            else
            {
                return null;
            }

            view.QuestionId = question.Id;
            view.Prompt = question.Prompt;
            view.Description = question.Description;
            view.Hint = question.Hint;
            view.Feedback = question.Feedback;
            view.CountsTowardsMarking = question.CountsTowardsMarking;

            return view;
        }

        #endregion

    }

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

    public enum ExamStatus
    {
        Passed,
        Failed
    }


    public class PageView
    {
        public string Title { get; }
        public string Description { get; }
        public int QuestionsCount { get; }

        public PageView(string title, string description, int questionCount)
        {
            Title = title;
            Description = description;
            QuestionsCount = questionCount;
        }
    }

    public abstract class QuestionView
    {
        public Guid QuestionId { get; set; }
        public string Prompt { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
        public string Feedback { get; set; }
        public bool CountsTowardsMarking { get; set; }

    }

    public class MultipleChoiceQuestionView : QuestionView
    {
        public List<string> CorrectAnswers { get; set; }

        public List<string> Options { get; set; }
    }

    public class FreeTextQuestionView : QuestionView
    {
        public string ExpectedAnswer { get; set; }

        public List<string> Keywords { get; set; }
    }

    public class SliderQuestionView : QuestionView
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public bool ReversePassingThreshold { get; set; }
        public int PassingThresholdValue { get; set; }
    }


    public class PageOrder
    {
        public Guid PageId { get; set; }
        public int Order { get; set; }
        public bool IsCurrentPage { get; set; }
    }
}
