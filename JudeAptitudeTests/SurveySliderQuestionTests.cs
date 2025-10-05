using JudeAptitude.Attempt;
using JudeAptitude.ExamBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudeAptitudeTests
{
    public class SurveySliderQuestionTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void SurveySliderQuestion_ShouldStoreAnswer_AndNotScore()
        {
            // Arrange
            var sliderQuestion = new SliderQuestion
            {
                Prompt = "Rate your satisfaction from 0 to 10",
                MinValue = 0,
                MaxValue = 10
            };

            var unmarkedSurvey = new Exam("", false)
            {
                Pages = new List<Page>
                {
                    new Page("") { Questions = new List<Question> { sliderQuestion } }
                }
            };

            var attempt = new ExamAttempt(unmarkedSurvey);

            // Act
            attempt.AddAnswer(sliderQuestion.Id, 8);
            attempt.Submit();

            // Assert
            Assert.IsNull(attempt.Score, "Survey attempts should not have a score.");
            Assert.AreEqual(1, attempt.Answers.Count, "There should be one answer recorded.");

            var answer = attempt.Answers[0];
            Assert.AreEqual(sliderQuestion.Id, answer.QuestionId, "Answer QuestionId should match slider question.");
            Assert.AreEqual(8, answer.GivenNumber, "Answer GivenNumber should be the slider value entered.");
        }

        [Test]
        public void AddAnswer_ShouldThrow_WhenValueOutsideRange()
        {
            // Arrange
            var sliderQuestion = new SliderQuestion
            {
                Prompt = "Rate your satisfaction from 0 to 10",
                MinValue = 0,
                MaxValue = 10
            };

            var unmarkedSurvey = new Exam("", false)
            {
                Pages = new List<Page>
                {
                    new Page("") { Questions = new List<Question> { sliderQuestion } }
                }
            };

            var attempt = new ExamAttempt(unmarkedSurvey);

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => attempt.AddAnswer(sliderQuestion.Id, 15));
            StringAssert.Contains("Value must be between 0 and 10", ex.Message);
        }
    }
}
