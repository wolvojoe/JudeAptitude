using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudeAptitudeTests
{
    public class ExamAttemptTests
    {
        [SetUp]
        public void Setup()
        {
        }


        private JudeExam BuildExam()
        {
            var mcq = new MultipleChoiceQuestion
            {
                Prompt = "Select all even numbers",
                Options = new List<string> { "1", "2", "3", "4" },
                CorrectAnswers = new List<string> { "2", "4" },
                CountsTowardsMarking = true,
                Order = 1
            };
            mcq.SetToAllOrNothingMarking();

            var ftq = new FreeTextQuestion
            {
                Prompt = "What is the answer to life, the universe and everything?",
                ExpectedAnswer = "42",
                UseExactMatch = true,
                CountsTowardsMarking = true,
                Order = 2
            };

            var slider = new SliderQuestion
            {
                Prompt = "Rate from 1 to 5",
                MinValue = 1,
                MaxValue = 5,
                PassingThresholdValue = 3,
                CountsTowardsMarking = true,
                Order = 3
            };

            var page1 = new ExamPage("Page 1") { Order = 1 };
            page1.Questions.Add(mcq);
            page1.Questions.Add(ftq);
            page1.Questions.Add(slider);

            var page2 = new ExamPage("Page 2") { Order = 2 };
            var ftq2 = new FreeTextQuestion
            {
                Prompt = "Say hello",
                ExpectedAnswer = "hello",
                UseExactMatch = true,
                CountsTowardsMarking = true,
                Order = 1
            };
            page2.Questions.Add(ftq2);

            var exam = new JudeExam("Test Exam", isMarked: true);
            exam.Pages.Add(page1);
            exam.Pages.Add(page2);
            exam.SetPassingMarkPercentage(0.5m);
            exam.ValidateExam();
            return exam;
        }

        [Test]
        public void ExamAttempt_AllPublicFunctions_AreTested()
        {
            var exam = BuildExam();
            var attempt = new ExamAttempt(exam);

            // GetAllQuestionsOnCurrentPage
            var questionsPage1 = attempt.GetAllQuestionsOnCurrentPage();
            Assert.AreEqual(3, questionsPage1.Count);
            Assert.That(questionsPage1.Any(q => q.Prompt == "Select all even numbers"));

            // SubmitAnswerForQuestionOnCurrentPage (MCQ)
            var mcq = exam.Pages[0].Questions.OfType<MultipleChoiceQuestion>().First();
            var mcqAnswer = new MultipleChoiceAnswer
            {
                QuestionId = mcq.Id,
                GivenAnswers = new List<string> { "2", "4" }
            };
            attempt.SubmitAnswerForQuestionOnCurrentPage(mcqAnswer);
            Assert.IsNotNull(attempt.GetCurrentAnswerForQuestion(mcq.Id));

            // SubmitAnswerForQuestionOnCurrentPage (FreeText)
            var ftq = exam.Pages[0].Questions.OfType<FreeTextQuestion>().First();
            var ftqAnswer = new FreeTextAnswer
            {
                QuestionId = ftq.Id,
                GivenText = "42"
            };
            attempt.SubmitAnswerForQuestionOnCurrentPage(ftqAnswer);
            Assert.IsNotNull(attempt.GetCurrentAnswerForQuestion(ftq.Id));

            // SubmitAnswerForQuestionOnCurrentPage (Slider)
            var slider = exam.Pages[0].Questions.OfType<SliderQuestion>().First();
            var sliderAnswer = new SliderAnswer
            {
                QuestionId = slider.Id,
                GivenNumber = 3
            };
            attempt.SubmitAnswerForQuestionOnCurrentPage(sliderAnswer);
            Assert.IsNotNull(attempt.GetCurrentAnswerForQuestion(slider.Id));

            // GetCorrectAnswerForQuestion
            var correctMcq = attempt.GetCorrectAnswerForQuestion(mcq.Id) as MultipleChoiceAnswerView;
            Assert.IsNotNull(correctMcq);
            Assert.That(correctMcq.GivenAnswers, Is.EquivalentTo(mcq.CorrectAnswers));

            var correctFtq = attempt.GetCorrectAnswerForQuestion(ftq.Id) as FreeTextAnswerView;
            Assert.IsNotNull(correctFtq);
            Assert.AreEqual(ftq.ExpectedAnswer, correctFtq.GivenText);

            var correctSlider = attempt.GetCorrectAnswerForQuestion(slider.Id) as SliderAnswerView;
            Assert.IsNotNull(correctSlider);
            Assert.That(correctSlider.GivenNumber, Is.InRange(slider.MinValue, slider.MaxValue));

            // GetCurrentPage
            var currentPageView = attempt.GetCurrentPage();
            Assert.AreEqual("Page 1", currentPageView.Title);

            // NavigateToNextPage and GetAllQuestionsOnCurrentPage
            attempt.NavigateToNextPage();
            var questionsPage2 = attempt.GetAllQuestionsOnCurrentPage();
            Assert.AreEqual(1, questionsPage2.Count);
            Assert.AreEqual("Say hello", questionsPage2[0].Prompt);

            // SubmitAnswerForQuestionOnCurrentPage (Page 2)
            var ftq2 = exam.Pages[1].Questions.OfType<FreeTextQuestion>().First();
            var ftq2Answer = new FreeTextAnswer
            {
                QuestionId = ftq2.Id,
                GivenText = "hello"
            };
            attempt.SubmitAnswerForQuestionOnCurrentPage(ftq2Answer);
            Assert.IsNotNull(attempt.GetCurrentAnswerForQuestion(ftq2.Id));

            // NavigateToPreviousPage
            attempt.NavigateToPreviousPage();
            var backToPage1 = attempt.GetCurrentPage();
            Assert.AreEqual("Page 1", backToPage1.Title);

            // SubmitExamAttempt
            var result = attempt.SubmitExamAttempt();
            Assert.IsNotNull(result);
            Assert.AreEqual(exam.Id, result.ExamId);
            Assert.AreEqual(attempt.ExamAttemptId, result.ExamAttemptId);
            Assert.IsNotNull(result.SubmittedDate);
            Assert.AreEqual(exam.MaximumPossibleMark(), result.MaximumPossibleMark);
            Assert.IsNotNull(result.Answers);
            Assert.AreEqual(4, result.Answers.Count);
            Assert.That(result.Mark, Is.EqualTo(result.MaximumPossibleMark));
            Assert.AreEqual(ExamStatus.Passed, result.ExamStatus);
        }


    }

}
