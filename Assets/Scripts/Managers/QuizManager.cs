using static GameManager;
using static SubjectsManager;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    private IQuizFactory quizFactory;
    private IQuiz quiz;

    public enum QuestionState
    {
        Pending,
        Correct,
        Wrong
    }

    public QuestionState currentQuestionState;

    public FeedbackManager feedbackManager;
    public DialogManager dialogManager;
    public Stepper stepper;
    public QuizSummary quizSummary;

    [SerializeField] private GameType gameType;
    private Subject subject;
    [SerializeField] private Question question;

    public AnswersManager answersManager;

    public List<ToriObject> selectedObjects { get; private set; }
    private List<ToriObject> usedObjects;
    private int currentObjectIndex;

    public Animator chestLidAnimator;
    public Animator parallelObjectAnimator;

    [Header("Test")]
    public bool isTest;
    public QuizTester quizTester;

    void Start ()
    {
        quizFactory = new QuizFactory();
        quiz = quizFactory.CreateQuiz(gameType);

        LoadObjectsAndSubject();

        usedObjects = new List<ToriObject>();

        InitiateQuiz();
        SetAnswersQuizManager();
    }

    private void InitiateQuiz ()
    {
        quiz.SetQuizManager(this);
        quiz.SetSubject(subject);
        quiz.SetQuestion(question);

        InitializeAnswers();

        quiz.InitiateQuiz();
    }

    private void InitializeAnswers ()
    {
        /*List<Answer> activeAnswers = */
        answersManager.InitializeAnswers();

        //quiz.SetAnswers(activeAnswers);
    }

    private void LoadObjectsAndSubject ()
    {
        if (isTest)
        {
            selectedObjects = quizTester.subject.toriObjects;
            subject = quizTester.subject;
        }
        else
        {
            selectedObjects = GameManager.Instance.selectedObjects;
            subject = GameManager.Instance.currentSubject;
        }
    }

    private void SetAnswersQuizManager ()
    {
        foreach (var answer in answersManager.GetActiveAnswers())
        {
            answer.SetQuizManager(this);
        }
    }

    public ToriObject GetCurrentObject ()
    {
        ToriObject obj = selectedObjects[currentObjectIndex];
        AddObjectToUsuedObjectList(obj);
        return obj;
    }

    public void MoveToNextObject ()
    {
        ResetQuestionState();
        ResetToriEmoji();
        currentObjectIndex++;
    }

    private void AddObjectToUsuedObjectList ( ToriObject usedObject )
    {
        if (usedObjects.Contains(usedObject)) return;

        usedObjects.Add(usedObject);
    }

    public Answer GetUnusedAnswer ()
    {
        return answersManager.GetUnusedAnswer();
    }

    public void ResetUnusedAnswersList ()
    {
        answersManager.ResetUnusedAnswersList();
    }


    public List<ToriObject> GetRandomObjects ( int numberOfObjects, ToriObject exceptThisObject )
    {
        // Create a copy of the selectedObjects list
        List<ToriObject> tempObjects = new List<ToriObject>(selectedObjects);

        // Remove the specified object from the temp list
        tempObjects.Remove(exceptThisObject);

        // Create a new list to store the random objects
        List<ToriObject> objList = new List<ToriObject>();

        for (int i = 0; i < numberOfObjects; i++)
        {
            int randomIndex = Random.Range(0, tempObjects.Count);
            objList.Add(tempObjects[randomIndex]);
            tempObjects.RemoveAt(randomIndex);  // Remove the selected object to avoid duplicates
        }

        return objList;
    }

    public void CorrectAnswer ()
    {
        currentQuestionState = QuestionState.Correct;
        quiz.CorrectAnswer();
    }

    public void WrongAnswer ()
    {
        currentQuestionState = QuestionState.Wrong;
        quiz.WrongAnswer();
    }

    public void ResetQuestionState ()
    {
        currentQuestionState = QuestionState.Pending;
    }

    public void AnswerClicked ( bool isCorrect )
    {
        quiz.AnswerClicked(isCorrect);
    }

    public void OnFeedbackClicked ()
    {
        switch (currentQuestionState)
        {
            case QuestionState.Correct:
                if (usedObjects.Count < selectedObjects.Count)
                    quiz.CorrectFeedbackClicked();
                else
                    CompleteQuiz();
                break;

            case QuestionState.Wrong:
                quiz.WrongFeedbackClicked();
                break;

            case QuestionState.Pending:
                dialogManager.StartTalking();
                break;

        }
    }

    private void CompleteQuiz ()
    {
        quizSummary.ShowSummary();
        quiz.CompleteQuiz();
    }

    private void ResetToriEmoji ()
    {
        dialogManager.toriTheCat.SetEmotion("Default");
    }

    public void ResetQuiz ()
    {
        usedObjects.Clear();
        answersManager.ResetUnusedAnswersList();
        currentObjectIndex = 0;
        ResetQuestionState();
        quiz.InitiateQuiz();
        quizSummary.ResetStickers();
        quizSummary.HideSummary();
    }


}
