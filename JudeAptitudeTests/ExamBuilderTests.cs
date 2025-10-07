using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudeAptitudeTests
{
    public class ExamBuilderTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Exam_WithPagesAndQuestions_ShouldBeValid()
        {
            // Arrange
            var exam = new Exam("Sample Exam", isMarked: true);

            var page1 = new Page("Page 1");
            var question1 = new DummyQuestion { Prompt = "What is 2+2?", CountsTowardsMarking = true };
            page1.Questions.Add(question1);

            var page2 = new Page("Page 2");
            var question2 = new DummyQuestion { Prompt = "What is the capital of France?", CountsTowardsMarking = true };
            page2.Questions.Add(question2);

            exam.Pages.Add(page1);
            exam.Pages.Add(page2);

            // Act
            var result = exam.ValidateExam();

            // Assert
            Assert.IsTrue(result.IsValid, $"Exam should be valid, but errors were: {string.Join(", ", result.Errors)}");
        }

        [Test]
        public void Exam_WithAllQuestionTypes_ShouldBeValid()
        {
            // Arrange
            var exam = new Exam("Complex Exam", isMarked: true);

            var page1 = new Page("MCQ Page");
            var mcq = new MultipleChoiceQuestion
            {
                Prompt = "Select all prime numbers",
                Options = new List<string> { "2", "3", "4" },
                CorrectAnswers = new List<string> { "2", "3" },
                CountsTowardsMarking = true
            };
            page1.Questions.Add(mcq);

            var page2 = new Page("Free Text Page");
            var ftq = new FreeTextQuestion
            {
                Prompt = "What is the answer to life, the universe and everything?",
                ExpectedAnswer = "42",
                UseExactMatch = true,
                CountsTowardsMarking = true
            };
            page2.Questions.Add(ftq);

            var page3 = new Page("Slider Page");
            var slider = new SliderQuestion
            {
                Prompt = "Rate your experience from 1 to 10",
                MinValue = 1,
                MaxValue = 10,
                CountsTowardsMarking = true
            };
            page3.Questions.Add(slider);

            exam.Pages.Add(page1);
            exam.Pages.Add(page2);
            exam.Pages.Add(page3);

            // Act
            var result = exam.ValidateExam();

            // Assert
            Assert.IsTrue(result.IsValid, $"Exam should be valid, but errors were: {string.Join(", ", result.Errors)}");
            Assert.IsTrue(mcq.ValidateQuestion().IsValid, $"MCQ should be valid, but errors were: {string.Join(", ", mcq.ValidateQuestion().Errors)}");
            Assert.IsTrue(ftq.ValidateQuestion().IsValid, $"FreeTextQuestion should be valid, but errors were: {string.Join(", ", ftq.ValidateQuestion().Errors)}");
            Assert.IsTrue(slider.ValidateQuestion().IsValid, $"SliderQuestion should be valid, but errors were: {string.Join(", ", slider.ValidateQuestion().Errors)}");
        }



        #region Valid Tests

        #region Valid MultipleChoiceQuestion

        [Test]
        public void MultipleChoiceQuestion_WithValidOptionsAndCorrectAnswers_ShouldBeValid()
        {
            var question = new MultipleChoiceQuestion
            {
                Options = new List<string> { "A", "B", "C" },
                CorrectAnswers = new List<string> { "A", "B" },
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsTrue(result.IsValid);
            Assert.That(result.Errors, Is.Empty);
        }

        #endregion

        #region Valid FreeTextQuestion

        [Test]
        public void FreeTextQuestion_ExactMatchWithExpectedAnswer_ShouldBeValid()
        {
            var question = new FreeTextQuestion
            {
                ExpectedAnswer = "42",
                UseExactMatch = true,
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsTrue(result.IsValid);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void FreeTextQuestion_KeywordMatchWithKeywords_ShouldBeValid()
        {
            var question = new FreeTextQuestion
            {
                ExpectedAnswer = "",
                UseExactMatch = false,
                Keywords = new List<string> { "life", "universe", "everything" },
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsTrue(result.IsValid);
            Assert.That(result.Errors, Is.Empty);
        }

        #endregion

        #region Valid SliderQuestion

        [Test]
        public void SliderQuestion_MinValueLessThanMaxValue_ShouldBeValid()
        {
            var question = new SliderQuestion
            {
                MinValue = 0,
                MaxValue = 10,
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsTrue(result.IsValid);
            Assert.That(result.Errors, Is.Empty);
        }

        #endregion

        #endregion




        #region Invalid Tests

        #region Invalid Exams

        [Test]
        public void Exam_WithNoPages_ShouldBeInvalid()
        {
            var exam = new Exam("No Pages Exam", isMarked: false);
            var result = exam.ValidateExam();
            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Exam has no Pages"));
        }

        [Test]
        public void Exam_WithPagesButNoQuestions_ShouldBeInvalid()
        {
            var exam = new Exam("No Questions Exam", isMarked: false);
            exam.Pages.Add(new Page("Page 1"));
            var result = exam.ValidateExam();
            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Exam has no Questions"));
        }

        [Test]
        public void MarkedExam_WithNoMarkedQuestions_ShouldBeInvalid()
        {
            var exam = new Exam("Marked Exam", isMarked: true);
            var page = new Page("Page 1");
            var question = new DummyQuestion { Prompt = "Unmarked Question", CountsTowardsMarking = false };
            page.Questions.Add(question);
            exam.Pages.Add(page);

            var result = exam.ValidateExam();
            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Exam is Marked but has no Questions that count towards Mark"));
        }

        #endregion

        #region Invalid Questions

        [Test]
        public void MarkedExam_WithInvalidQuestion_ShouldBeInvalid()
        {
            var exam = new Exam("Invalid Question Exam", isMarked: true);
            var page = new Page("Page 1");
            var invalidQuestion = new InvalidDummyQuestion { Prompt = "Invalid", CountsTowardsMarking = true };
            page.Questions.Add(invalidQuestion);
            exam.Pages.Add(page);

            var result = exam.ValidateExam();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Question is invalid"));
        }

        [Test]
        public void MarkedExam_WithMultipleInvalidQuestions_ShouldAggregateErrors()
        {
            var exam = new Exam("Multiple Invalid Questions", isMarked: true);
            var page = new Page("Page 1");
            var invalidQuestion1 = new InvalidDummyQuestion { Prompt = "Invalid 1", CountsTowardsMarking = true };
            var invalidQuestion2 = new InvalidDummyQuestion { Prompt = "Invalid 2", CountsTowardsMarking = true };
            page.Questions.Add(invalidQuestion1);
            page.Questions.Add(invalidQuestion2);
            exam.Pages.Add(page);

            var result = exam.ValidateExam();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(result.Errors, Does.Contain("Question is invalid"));
        }

        [Test]
        public void MarkedExam_WithValidAndInvalidQuestions_ShouldBeInvalid()
        {
            var exam = new Exam("Mixed Validity Questions", isMarked: true);
            var page = new Page("Page 1");
            var validQuestion = new DummyQuestion { Prompt = "Valid", CountsTowardsMarking = true };
            var invalidQuestion = new InvalidDummyQuestion { Prompt = "Invalid", CountsTowardsMarking = true };
            page.Questions.Add(validQuestion);
            page.Questions.Add(invalidQuestion);
            exam.Pages.Add(page);

            var result = exam.ValidateExam();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Question is invalid"));
        }

        #endregion


        #region Invalid MultipleChoiceQuestion

        [Test]
        public void MultipleChoiceQuestion_WithLessThanTwoOptions_ShouldBeInvalid()
        {
            var question = new MultipleChoiceQuestion
            {
                Options = new List<string> { "A" },
                CorrectAnswers = new List<string> { "A" },
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.StartWith("A multiple choice question must have at least 2 options"));
        }

        [Test]
        public void MultipleChoiceQuestion_WithNoCorrectAnswers_ShouldBeInvalid()
        {
            var question = new MultipleChoiceQuestion
            {
                Options = new List<string> { "A", "B" },
                CorrectAnswers = new List<string>(),
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.StartWith("A multiple choice question must have at least 1 correct answer"));
        }

        [Test]
        public void MultipleChoiceQuestion_WithCorrectAnswerNotInOptions_ShouldBeInvalid()
        {
            var question = new MultipleChoiceQuestion
            {
                Options = new List<string> { "A", "B" },
                CorrectAnswers = new List<string> { "C" },
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.StartWith("Correct answer 'C' is not in the list of options"));
        }

        #endregion

        #region Invalid FreeTextQuestion

        [Test]
        public void FreeTextQuestion_ExactMatchWithNoExpectedAnswer_ShouldBeInvalid()
        {
            var question = new FreeTextQuestion
            {
                ExpectedAnswer = "",
                UseExactMatch = true,
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.StartWith("A Free Text question needs an Expected Answer"));
        }

        [Test]
        public void FreeTextQuestion_KeywordMatchWithNoKeywords_ShouldBeInvalid()
        {
            var question = new FreeTextQuestion
            {
                ExpectedAnswer = "",
                UseExactMatch = false,
                Keywords = new List<string>(),
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.StartWith("A Free Text question needs Expected Keywords"));
        }

        #endregion

        #region Invalid SliderQuestion

        [Test]
        public void SliderQuestion_MinValueGreaterThanOrEqualToMaxValue_ShouldBeInvalid()
        {
            var question = new SliderQuestion
            {
                MinValue = 10,
                MaxValue = 5,
                CountsTowardsMarking = true
            };

            var result = question.ValidateQuestion();

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.StartWith("A Slider question needs a Min Value less than the Max Value"));
        }

        #endregion

        #endregion






        [Test]
        public void Exam_Constructor_InitializesProperties()
        {
            var exam = new Exam("Sample Exam", isMarked: true);
            Assert.AreEqual("Sample Exam", exam.Title);
            Assert.IsTrue(exam.IsMarked);
            Assert.IsNotNull(exam.Pages);
            Assert.IsNotNull(exam.Tags);
            Assert.AreEqual(DifficultyLevel.NotSpecified, exam.Difficulty);
        }

        [Test]
        public void ValidateExam_ReturnsValid_WhenExamIsValid()
        {
            var exam = new Exam("Valid Exam", isMarked: true);
            var page = new Page("Page 1");
            var question = new DummyQuestion { Prompt = "Q1", CountsTowardsMarking = true };
            page.Questions.Add(question);
            exam.Pages.Add(page);

            var result = exam.ValidateExam();
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ValidateExam_ReturnsInvalid_WhenNoPages()
        {
            var exam = new Exam("No Pages", isMarked: false);
            var result = exam.ValidateExam();
            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Exam has no Pages"));
        }

        [Test]
        public void ValidateExam_ReturnsInvalid_WhenNoQuestions()
        {
            var exam = new Exam("No Questions", isMarked: false);
            exam.Pages.Add(new Page("Page 1"));
            var result = exam.ValidateExam();
            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Exam has no Questions"));
        }

        [Test]
        public void AllQuestions_ReturnsAllQuestions()
        {
            var exam = new Exam("Exam", isMarked: false);
            var page1 = new Page("Page 1");
            var page2 = new Page("Page 2");
            var q1 = new DummyQuestion { Prompt = "Q1" };
            var q2 = new DummyQuestion { Prompt = "Q2" };
            page1.Questions.Add(q1);
            page2.Questions.Add(q2);
            exam.Pages.Add(page1);
            exam.Pages.Add(page2);

            var allQuestions = exam.AllQuestions();
            Assert.AreEqual(2, allQuestions.Count);
            Assert.Contains(q1, allQuestions);
            Assert.Contains(q2, allQuestions);
        }

        [Test]
        public void AllQuestionsCountingTowardsMark_ReturnsOnlyMarkedQuestions()
        {
            var exam = new Exam("Exam", isMarked: true);
            var page = new Page("Page 1");
            var q1 = new DummyQuestion { Prompt = "Q1", CountsTowardsMarking = true };
            var q2 = new DummyQuestion { Prompt = "Q2", CountsTowardsMarking = false };
            page.Questions.Add(q1);
            page.Questions.Add(q2);
            exam.Pages.Add(page);

            var markedQuestions = exam.AllQuestionsCountingTowardsMark();
            Assert.AreEqual(1, markedQuestions.Count);
            Assert.Contains(q1, markedQuestions);
            Assert.IsFalse(markedQuestions.Contains(q2));
        }

        [Test]
        public void SetPassingMarkPercentage_ValidAndInvalidValues()
        {
            var exam = new Exam("Exam", isMarked: true);
            Assert.IsTrue(exam.SetPassingMarkPercentage(0.5m));
            Assert.IsFalse(exam.SetPassingMarkPercentage(-0.1m));
            Assert.IsFalse(exam.SetPassingMarkPercentage(1.1m));
        }

        [Test]
        public void PassingMark_ReturnsExpectedValue()
        {
            var exam = new Exam("Exam", isMarked: true);
            var page = new Page("Page 1");
            var q1 = new DummyQuestion { Prompt = "Q1", CountsTowardsMarking = true };
            page.Questions.Add(q1);
            exam.Pages.Add(page);
            exam.SetPassingMarkPercentage(0.5m);

            var expected = exam.MaximumPossibleMark() * 0.5m;
            Assert.AreEqual(expected, exam.PassingMarkTotal());
        }

        [Test]
        public void MaximumPossibleMark_SumsAllPages()
        {
            var exam = new Exam("Exam", isMarked: true);
            var page1 = new Page("Page 1");
            var page2 = new Page("Page 2");
            var q1 = new DummyQuestion { Prompt = "Q1", CountsTowardsMarking = true };
            var q2 = new DummyQuestion { Prompt = "Q2", CountsTowardsMarking = true };
            page1.Questions.Add(q1);
            page2.Questions.Add(q2);
            exam.Pages.Add(page1);
            exam.Pages.Add(page2);

            var expected = q1.MaximumPossibleMark() + q2.MaximumPossibleMark();
            Assert.AreEqual(expected, exam.MaximumPossibleMark());
        }





        // DummyQuestion for testing purposes
        private class DummyQuestion : Question
        {
            public DummyQuestion() : base() { }

            public override ValidationResult ValidateQuestion()
            {
                return ValidationResult.Valid();
            }

            public override decimal MaximumPossibleMark()
            {
                return 1; // Dummy implementation
            }
        }

        private class InvalidDummyQuestion : Question
        {
            public InvalidDummyQuestion() : base() { }
            public override ValidationResult ValidateQuestion()
            {
                return ValidationResult.Invalid(new List<string> { "Question is invalid" });
            }

            public override decimal MaximumPossibleMark()
            {
                return 1; // Dummy implementation
            }
        }
    }
}
