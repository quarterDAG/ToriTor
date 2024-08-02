using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{
    public Image image;
    public AudioSource audioSource;
    public Draggable draggable;
    public RectTransform target;
    private bool isCorrect;
    private Vector3 initialScale;
    private Vector2 initialPosition;
    private Transform initialParent;
    private RectTransform rectTransform;

    private Fader fader;

    public ToriObject toriObject { get; private set; }
    public QuizManager quizManager { get; private set; }
    private AnswersManager answersManager;

    private void Awake ()
    {
        rectTransform = GetComponent<RectTransform>();
        fader = GetComponent<Fader>();

        initialParent = transform.parent;
        initialScale = rectTransform.localScale;
        initialPosition = rectTransform.anchoredPosition;
    }

    public void SetAudioClip ( AudioClip _clip )
    {
        audioSource.clip = _clip;
    }

    public void SetImage ( Sprite _sprite )
    {
        image.sprite = _sprite;
        image.SetNativeSize();
    }

    public void SetColor ( Color _color )
    {
        image.color = _color;
    }

    public void SetQuizManager ( QuizManager _quizManager )
    {
        if (quizManager == null)
            quizManager = _quizManager;

        answersManager = _quizManager.answersManager;
    }

    public void SetAsCorrect () { isCorrect = true; }

    public void SetTarget ( RectTransform target )
    {
        draggable.SetTarget(target);
    }

    public void OnClickAnswer ()
    {
        audioSource.Play();

        quizManager.AnswerClicked(isCorrect);

    }

    public void PlayerAnswerCorrect ()
    {
        //audioSource.Play();
        answersManager.SetCurrentAnswer(this);
        quizManager.CorrectAnswer();
    }

    public void ResetAnswer ()
    {
        isCorrect = false;
        transform.SetParent(initialParent);
        rectTransform.localScale = initialScale;
        rectTransform.anchoredPosition = initialPosition;

        if (draggable)
        {
            draggable.SetTarget(null);
            draggable.EnableDrag();
        }
    }

    public ToriObject GetToriObject ()
    {
        return toriObject;
    }

    public void FadeIn ()
    {
        fader?.FadeIn();
    }

    public void FadeOut ()
    {
        fader?.FadeOut();
    }

}
