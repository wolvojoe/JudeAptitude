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


## Documentation

| Exam Builder |  |
| --- | --- |
| **JudeExam**	| The main Exam object	| 
| Title	| Title of the Exam	| 
| Description	| Description of the Exam	| 
| Subject	| Subject of the Exam	| 
| Tags	| Tags filter/ search Exams	| 
| Difficulty	| Difficulty to help filter/ search Exams	| 
| IsMarked	| True = All Questions need expected answers, when an ExamAttempt is submitted it will be automatically marked. False = Questions don’t need correct answers being set, submitting the ExamAttempt will not mark the exam.	| 
| RandomisePageOrder	| True = Pages will be sorted randomly for each ExamAttempt	| 
| Pages	| Pages that will be navigated in the ExamAttempt	| 
| PassingMarkPercentage	| A percentage passing rate, between 0 – 1	| 
| SetPassingMarkPercentage()	| Sets the percentage passing rate	| 
| ValidateExam()	| Validate that the Exam you have created is ready to be Attempted	| 
| AllQuestionsCountingTowardsMark()	| Returns all Questions that count towards the mark	| 
| AllQuestions()	| Returns all Questions	| 
| PassingMarkTotal()	| Total passing mark	| 
| MaximumPossibleMark()	| Maximum passing mark	| 
| **ExamPage**	| Page for an Exam	| 
| Title	| Title of the Page	| 
| Description	| Description of the Page, could be used to display HTML content	| 
| Questions	| Questions for the Page	| 
| RandomiseQuestionOrder	| True = Questions on the Page will have their order randomised on each ExamAttempt, False = The Order attribute on a Question object will be used for ordering Questions	| 
| Order	| Used to Order Pages	| 
| MaximumPossibleMark()	| Maximum possible Mark for the current Page	| 
| **Questions**	| Functionality common to all Question types	| 
| Prompt	| The text used for the Question Prompt	| 
| Description	| Description for the Question, could be used for HTML content	| 
| Hint	| Value that could be shown against the Question	| 
| Feedback	| Value that could be displayed upon submitting an Answer	| 
| Order	| Order of Questions	| 
| CountsTowardsMarking	| True = Question will be counted towards the final Mark	| 
| ValidateQuestion()	| Validate that you have created a valid Question	| 
| MaximumPossibleMark()	| Maximum possible Mark for the Question	| 
| **MultipleChoiceQuestion**	| Question type for True/ False, Checkbox or Radio Buttons	| 
| Options	| The options for the Multiple Choice	| 
| CorrectAnswers	| The correct options	| 
| SetToAllOrNothingMarking()	| Get all options correct to get a Mark	| 
| SetToPartialMarking()	| Get some options correct to get a Mark	| 
| **FreeTextQuestion**	| Question type for Textboxes	|
| ExpectedAnswer	| Expected text Answer	| 
| Keywords	| Keywords for Partial Matching	| 
| UseExactMatch	| False = Keywords will be checked	| 
| **SliderQuestion**	| Question type for a Survey or Feedback form	| 
| MinValue	| Min value of say 1	| 
| MaxValue	| Max value of say 10	| 
| ReversePassingThreshold	| If a correct answer it positive of the Threshold of negative of it	| 
| PassingThresholdValue	| Passing value of say 7	| 


***

| Exam Attempt |  |
| --- | --- |
| **ExamAttempt**	| Accepts a JudeExam object and provides functionality to Attempt the Exam	| 
| SubmitAnswerForQuestionOnCurrentPage	| Submit an Answer for the current Page	| 
| SubmitExamAttempt()	| Submits and finalises the Exam Attempt and potentially Marking	| 
| GetAllQuestionsOnCurrentPage()	| Gets all the Questions on the current Page, ordered as defined on the Exam	| 
| GetCurrentAnswerForQuestion()	| Gets a Questions current Answer Submitted for the ExamAttempt	| 
| GetCorrectAnswerForQuestion()	| Gets a Questions correct Answer	| 
| GetCurrentPage()	| Gets details for the current Page	| 
| NavigateToNextPage()	| Moves the Exam to the next Page defined on the Exam	| 
| NavigateToPreviousPage()	| Moves the Exam to the Previous Page defined on the Exam	| 


***


**In memory of Jude (2023 - 2024)**