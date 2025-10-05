using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JudeAptitude.ExamBuilder
{
    public class Exam
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsMarked { get; }

        public List<Page> Pages { get; set; }


        public Exam(string title, bool isMarked)
        {
            Title = title;
            IsMarked = isMarked;

            Id = Guid.NewGuid();

            Pages = new List<Page>();
        }

        public bool ValidateExam()
        {
            ValidateQuestionTypes();

            return true;
        }


        public void ValidateQuestionTypes()
        {
            

            if (1 == 0)
            {
                throw new InvalidOperationException("Text questions are not allowed in marked exams.");
            }
        }


        public List<Question> GetAllQuestions()
        {
            return Pages.SelectMany(p => p.Questions).ToList();
        }

    }
}
