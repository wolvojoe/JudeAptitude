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


        [Test]
        public void ExamAttempt_Submit_ReturnsCorrectExamResult()
        {
            // Arrange: Build a simple marked exam with all question types
            var exam = new Exam("Test Exam", isMarked: true);

            var mcq = new MultipleChoiceQuestion
            {
                Prompt = "Select all even numbers",
                Options = new List<string> { "1", "2", "3", "4" },
                CorrectAnswers = new List<string> { "2", "4" },
                CountsTowardsMarking = true
            };
            mcq.SetToAllOrNothingMarking();

            var ftq = new FreeTextQuestion
            {
                Prompt = "What is the answer to life, the universe and everything?",
                ExpectedAnswer = "42",
                UseExactMatch = true,
                CountsTowardsMarking = true
            };

            var slider = new SliderQuestion
            {
                Prompt = "Rate from 1 to 5",
                MinValue = 1,
                MaxValue = 5,
                PassingThresholdValue = 3,
                CountsTowardsMarking = true
            };

            var page = new Page("Page 1");
            page.Questions.Add(mcq);
            page.Questions.Add(ftq);
            page.Questions.Add(slider);
            exam.Pages.Add(page);

            var attempt = new ExamAttempt(exam);

            // Act: Add correct answers for all questions
            attempt.AddAnswer(new MultipleChoiceAnswer
            {
                QuestionId = mcq.Id,
                GivenAnswers = new List<string> { "2", "4" }
            });
            attempt.AddAnswer(new FreeTextAnswer
            {
                QuestionId = ftq.Id,
                GivenText = "42"
            });
            attempt.AddAnswer(new SliderAnswer
            {
                QuestionId = slider.Id,
                GivenNumber = 3 // Assume 3 is the "correct" value for this test
            });

            var result = attempt.Submit();

            // Assert: ExamResult is correct
            Assert.IsNotNull(result);
            Assert.AreEqual(exam.Id, result.ExamId);
            Assert.AreEqual(attempt.ExamAttemptId, result.ExamAttemptId);
            Assert.IsNotNull(result.SubmittedDate);
            Assert.AreEqual(exam.MaximumPossibleMark(), result.MaximumPossibleMark);
            Assert.IsNotNull(result.Answers);
            Assert.AreEqual(3, result.Answers.Count);

            // Each answer should be fully marked (assuming all-or-nothing and correct input)
            foreach (var answer in result.Answers)
            {
                Assert.That(answer.Mark, Is.GreaterThanOrEqualTo(1));
            }
            Assert.That(result.Mark, Is.EqualTo(result.MaximumPossibleMark));
        }












        private Exam BuildSimpleExam()
        {
            var mcq = new MultipleChoiceQuestion
            {
                Prompt = "Select all even numbers",
                Options = new List<string> { "1", "2", "3", "4" },
                CorrectAnswers = new List<string> { "2", "4" },
                CountsTowardsMarking = true
            };
            mcq.SetToAllOrNothingMarking();

            var ftq = new FreeTextQuestion
            {
                Prompt = "What is the answer to life, the universe and everything?",
                ExpectedAnswer = "42",
                UseExactMatch = true,
                CountsTowardsMarking = true
            };

            var slider = new SliderQuestion
            {
                Prompt = "Rate from 1 to 5",
                MinValue = 1,
                MaxValue = 5,
                PassingThresholdValue = 3,
                CountsTowardsMarking = true
            };

            var page = new Page("Page 1");
            page.Questions.Add(mcq);
            page.Questions.Add(ftq);
            page.Questions.Add(slider);

            var exam = new Exam("Test Exam", isMarked: true);
            exam.Pages.Add(page);
            return exam;
        }

        [Test]
        public void GetQuestionsOnCurrentPage_ReturnsQuestions()
        {
            var exam = BuildSimpleExam();
            var attempt = new ExamAttempt(exam);
            var questions = attempt.GetQuestionsOnCurrentPage();
            Assert.AreEqual(3, questions.Count);
        }

        [Test]
        public void AddAnswer_And_GetCurrentAnswerForQuestion_Works()
        {
            var exam = BuildSimpleExam();
            var attempt = new ExamAttempt(exam);
            var mcq = (MultipleChoiceQuestion)exam.Pages[0].Questions[0];

            var answer = new MultipleChoiceAnswer
            {
                QuestionId = mcq.Id,
                GivenAnswers = new List<string> { "2", "4" }
            };

            attempt.AddAnswer(answer);
            var retrieved = attempt.GetCurrentAnswerForQuestion(mcq.Id);
            Assert.IsNotNull(retrieved);
            Assert.That(retrieved, Is.InstanceOf<MultipleChoiceAnswer>());
            Assert.That(((MultipleChoiceAnswer)retrieved).GivenAnswers, Is.EquivalentTo(new[] { "2", "4" }));
        }

        [Test]
        public void GetCorrectAnswerForQuestion_ReturnsExpectedAnswer()
        {
            var exam = BuildSimpleExam();
            var attempt = new ExamAttempt(exam);
            var mcq = (MultipleChoiceQuestion)exam.Pages[0].Questions[0];
            var ftq = (FreeTextQuestion)exam.Pages[0].Questions[1];
            var slider = (SliderQuestion)exam.Pages[0].Questions[2];

            var mcqCorrect = attempt.GetCorrectAnswerForQuestion(mcq.Id) as MultipleChoiceAnswer;
            Assert.IsNotNull(mcqCorrect);
            Assert.That(mcqCorrect.GivenAnswers, Is.EquivalentTo(mcq.CorrectAnswers));

            var ftqCorrect = attempt.GetCorrectAnswerForQuestion(ftq.Id) as FreeTextAnswer;
            Assert.IsNotNull(ftqCorrect);
            Assert.AreEqual(ftq.ExpectedAnswer, ftqCorrect.GivenText);

            var sliderCorrect = attempt.GetCorrectAnswerForQuestion(slider.Id) as SliderAnswer;
            Assert.IsNotNull(sliderCorrect);
            Assert.That(sliderCorrect.GivenNumber, Is.InRange(slider.MinValue, slider.MaxValue));
        }

        [Test]
        public void Submit_ReturnsCorrectExamResult()
        {
            var exam = BuildSimpleExam();
            var attempt = new ExamAttempt(exam);
            var mcq = (MultipleChoiceQuestion)exam.Pages[0].Questions[0];
            var ftq = (FreeTextQuestion)exam.Pages[0].Questions[1];
            var slider = (SliderQuestion)exam.Pages[0].Questions[2];

            attempt.AddAnswer(new MultipleChoiceAnswer
            {
                QuestionId = mcq.Id,
                GivenAnswers = new List<string> { "2", "4" }
            });
            attempt.AddAnswer(new FreeTextAnswer
            {
                QuestionId = ftq.Id,
                GivenText = "42"
            });
            attempt.AddAnswer(new SliderAnswer
            {
                QuestionId = slider.Id,
                GivenNumber = 3
            });

            var result = attempt.Submit();

            Assert.IsNotNull(result);
            Assert.AreEqual(exam.Id, result.ExamId);
            Assert.AreEqual(attempt.ExamAttemptId, result.ExamAttemptId);
            Assert.IsNotNull(result.SubmittedDate);
            Assert.AreEqual(exam.MaximumPossibleMark(), result.MaximumPossibleMark);
            Assert.IsNotNull(result.Answers);
            Assert.AreEqual(3, result.Answers.Count);
            Assert.That(result.Mark, Is.EqualTo(result.MaximumPossibleMark));
        }

        [Test]
        public void Paging_NextAndPreviousPage_Works()
        {
            var exam = BuildSimpleExam();
            // Add a second page
            var page2 = new Page("Page 2");
            exam.Pages.Add(page2);
            var attempt = new ExamAttempt(exam);

            // Start at page 0
            Assert.AreEqual("Page 1", attempt.GetCurrentPage().Title);

            // Move to next page
            var next = attempt.NextPage();
            Assert.IsNotNull(next);
            Assert.AreEqual("Page 2", next.Title);

            // Move back to previous page
            var prev = attempt.PreviousPage();
            Assert.IsNotNull(prev);
            Assert.AreEqual("Page 1", prev.Title);
        }
    }

}
