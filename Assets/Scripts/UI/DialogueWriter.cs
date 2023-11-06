using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class DialogueWriter : MonoBehaviour
{
    public static DialogueWriter instance;
    public float writingSpeed = 15;

    public TMP_Animated sourceText;
    public RectTransform dialogueContainer;
    public float firsMessageDelay = 0.5f;
    public float animDuration = 0.5f;
    public Ease animEase = Ease.InOutBack;
    public float delayToSkip = 0.5f;


    List<string> sentencesToRead;
    bool isReading, sentenceFinished;
    int sentenceIndex;
    Vector3 defaultScale;
    bool canSkip;

    private void Awake()
    {
        instance = this;

        sourceText.speed = writingSpeed;
        defaultScale = dialogueContainer.localScale;
        dialogueContainer.localScale = Vector3.zero;
        sentenceFinished = true;
        canSkip = false;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && sentenceIndex > -1)
        {
            PressedNext();
        }
    }

    [ContextMenu("Test Dialogue")]
    public void Test()
    {
        string[] sentences = { "Teste frase 1", "Teste frase 2 mas desta vez bem mais longa que a outra", "Teste frase 3" };
        ReadSentences(sentences);
    }

    public void ReadSentences(string[] sentences)
    {

        if (!sentenceFinished)
        {
            sourceText.StopText();
        }
        else
        {
            ShowDialogue();
        }

        sentencesToRead = sentences.ToList();
        sentenceIndex = -1;

        //Debug.Log("Read Sentece before");
        isReading = true;

        NextSentence();
        //Debug.Log("Read Sentece after");

        StartCoroutine(WaitToEnableSkip());
    }

    IEnumerator WaitToEnableSkip()
    {
        canSkip=false;
        yield return new WaitForSeconds(delayToSkip);
        canSkip=true;
    }

    void PressedNext()
    {
        if (!canSkip || !isReading) return;

        //Debug.Log("Pressed Next");
        NextSentence();
    }


    void NextSentence()
    {
        sourceText.StopText();

        sentenceIndex++;

        if (sentenceIndex > sentencesToRead.Count - 1)
        {
            HideDialogue();
            isReading = false;
            return;
        }

        //Acount for show animation and apply delay 
        if (sentenceIndex == 0)
            sourceText.ReadText(sentencesToRead[sentenceIndex], firsMessageDelay);
        else
            sourceText.ReadText(sentencesToRead[sentenceIndex]);


    }

    void ShowDialogue()
    {
        dialogueContainer.DOScale(defaultScale, animDuration).SetEase(animEase);
    }

    void HideDialogue()
    {
        dialogueContainer.DOScale(Vector3.zero, animDuration).SetEase(animEase);
    }

}
