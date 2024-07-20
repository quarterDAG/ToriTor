using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsBook : MonoBehaviour, IBook
{
    [SerializeField] private TMP_Text objectText;
    [SerializeField] private Image image;
    [SerializeField] private AudioSource objectWordAudioSource;
    [SerializeField] private AudioSource objectSoundAudioSource;
    [SerializeField] private Button objectImageButton;

    [SerializeField] private Fader fader;

    [SerializeField] private BookPagesController bookPagesController;

    [SerializeField] private Sprite checkIconSprite;

    [SerializeField] private QuizSummary quizSummary;

    public List<ToriObject> objects { get; set; }
    public List<Subject> allSubjects { get; set; }

    private void Start ()
    {
        objects = GameManager.Instance.selectedObjects;

        bookPagesController.SetBook(this);
        SetBookPage(0);
    }




    public void SetBookPage ( int objectNumber )
    {
        fader.FadeOut();

        ToriObject obj = objects[objectNumber];

        objectText.text = obj.objectName;
        image.sprite = obj.sprite;
        image.SetNativeSize();
        image.color = obj.color;
        objectWordAudioSource.clip = obj.clip;

        if (obj.objectSoundClip != null)
        {
            objectSoundAudioSource.clip = obj.objectSoundClip;
            objectImageButton.interactable = true;
        }
        else
            objectImageButton.interactable = false;

        fader.FadeIn();

        SayTheWord();
    }


    public void SayTheWord ()
    {
        objectWordAudioSource.Play();
    }

    public void PlaySound ()
    {
        objectSoundAudioSource.Play();
    }

    public void Complete ()
    {
        quizSummary.ShowSummary();
    }

    public void ResetBook ()
    {
        SetBookPage(0);
        bookPagesController.ResetPages();
        quizSummary.HideSummary();
    }

}
