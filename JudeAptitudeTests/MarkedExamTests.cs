using JudeAptitude.Attempt.Marking;
using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudeAptitudeTests
{
    public class MarkedExamTests
    {
        private MultipleChoiceQuestion _mcq;
        private FreeTextQuestion _ftq;
        private Exam _exam;

        [SetUp]
        public void Setup()
        {
            _mcq = new MultipleChoiceQuestion
            {
                Prompt = "Select all prime numbers",
                Options = new List<string> { "2", "3", "4", "5" },
                CorrectAnswers = new List<string> { "2", "3", "5" }
            };

            _ftq = new FreeTextQuestion
            {
                Prompt = "What is the powerhouse of the cell?",
                ExpectedAnswer = "Mitochondria",
                UseExactMatch = true
            };

            _exam = new Exam("", true)
            {
                Pages = new List<Page>
                {
                    new Page("")
                    {
                        Questions = new List<Question> { _mcq, _ftq }
                    }
                }
            };
        }

        [Test]
        public void ExamAttempt_AllCorrectAnswers_ShouldScoreFull()
        {
            var attempt = new ExamAttempt(_exam);

            // Add all correct answers for MCQ
            attempt.AddAnswer(_mcq.Id, new List<string> { "2", "3", "5" });

            // Add correct answer for free text
            attempt.AddAnswer(_ftq.Id, "Mitochondria");

            attempt.Submit();

            Assert.AreEqual(2, attempt.Score, "Score should be 2 for both correct answers.");
        }

        [Test]
        public void ExamAttempt_PartialCorrectMCQ_FreeTextIncorrect_ShouldScoreOne()
        {
            var attempt = new ExamAttempt(_exam);

            // Partial MCQ answer: 2 correct (2,3), 1 incorrect (4)
            attempt.AddAnswer(_mcq.Id, new List<string> { "2", "3", "4" });

            // Incorrect free text answer
            attempt.AddAnswer(_ftq.Id, "Nucleus");

            // Combine marking strategies manually (example, or you can extend strategy)
            attempt.Submit();

            // Score calculation:
            // MCQ: (2 correct * 1) - (1 incorrect * 0.5) = 2 - 0.5 = 1.5
            // FreeText: 0
            // Rounded score:
            Assert.AreEqual(2, attempt.Score, "Score should be approximately 2 after rounding.");
        }

        [Test]
        public void ExamAttempt_AllWrongAnswers_ShouldScoreZero()
        {
            var attempt = new ExamAttempt(_exam);

            // All wrong MCQ answers
            attempt.AddAnswer(_mcq.Id, new List<string> { "1", "4" });

            // Wrong free text answer
            attempt.AddAnswer(_ftq.Id, "Chloroplast");

            attempt.Submit();

            Assert.AreEqual(0, attempt.Score, "Score should be zero for all wrong answers.");
        }
    }
}
