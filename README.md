# JudeAptitude

![JudeAptitude Logo](JudeAptitude-128.png)

## Introduction

JudeAptitude is an Exam/ Quiz/ CPD/ Survey Engine, created in part because when I see this functionality in projects it's always spaghetti code.

## Exam Builder

To start you need to create an Exam.

An Exam can contain multiple Pages, each Page can contain both content and Questions.

Pages could be used to contain tutorial content, training content, HTML, images, videos etc.

Questions can consist of Multiple Choice, Free Text or a Slider/ Survey (1 - 10) question.

Exams can also be marked, if so Questions that count towards the mark need to have a correct answer specified.

**Simple Exam**
```
    var myFirstExam = new JudeExam("Hey Jude", isMarked: true);

    var myFirstPage = new ExamPage("Simple Page 1");

    var myFirstQuestion = new FreeTextQuestion
    {
        Prompt = "Simple Question 1",
        ExpectedAnswer = "Cannock",
        UseExactMatch = true,
        CountsTowardsMarking = true
    };


    myFirstPage.Questions.Add(myFirstQuestion);
    myFirstExam.Pages.Add(myFirstPage);

    var result = myFirstExam.ValidateExam();

```
You can create Exam objects and save them to a DB or some other storage, or you can create Exam objects on the fly from say a large Question Bank.

## Attempt

Once you have an Exam a user can now Attempt it

**Simple Attempt**
```
    var myFirstAttempt = new ExamAttempt(myFirstExam);

    var questionsOnCurrentPage = myFirstAttempt.GetAllQuestionsOnCurrentPage();

    var simpleQuestion = questionsOnCurrentPage[0];

    myFirstAttempt.SubmitAnswerForQuestionOnCurrentPage(new FreeTextAnswer
    {
        QuestionId = simpleQuestion.QuestionId,
        GivenText = "Cannock"
    });

    var myFirstResult = myFirstAttempt.SubmitExamAttempt();

```

Once the ExamAttempt has been completed, you can call SubmitExamAttempt() and the Exam Attempt will be marked and return a Result object.

In memory of Jude (2023 - 2024)