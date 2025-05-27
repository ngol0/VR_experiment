using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public enum UI_STATE
{
    //NO_SELECTION,
    ON_COMPLETE,
    ON_ICON_CLICKED,
    QUESTION_DISPLAY,
    DONE_QUESTION,
    ON_CONTINUE
}

public class UIHandler : MonoBehaviour
{
    [Header("Logic")]
    [SerializeField] FlowManager manager;
    [SerializeField] QuestionManager questionManager;

    [Header("Init icons")]
    [SerializeField] private Transform root;
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private Transform StartIcon;
    [SerializeField] private float radius = 2.0f;

    // Private fields
    [Header("Logging")]
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text countdownText;

    [Header("Buttons")]
    [SerializeField] private List<DisplayButton> iconButtons;
    [SerializeField] DisplayButton startButton;
    [SerializeField] DisplayButton resetButton;
    [SerializeField] Button continueButton;

    [Header("Question UI")]
    [SerializeField] Transform questionUI;

    void OnEnable()
    {
        manager.OnStart += StartGame;
        manager.OnButtonClicked += UpdateLogOnClick;
        manager.OnUIChanged += OnUIChanged;
        manager.UpdateCountdownText += OnCountdownUpdate;

        questionManager.OnQuestionDone += UIOnQuestionDone;
    }

    void Start()
    {
        continueButton.interactable = false;
        resetButton.gameObject.SetActive(false);

        InitIconPrefab();
    }

    // initialize 10 icon prefab
    private void InitIconPrefab()
    {
        for (int i = 0; i < IconFinder.MAX_RANDOM; i++)
        {
            // init prefab
            float angle = i * Mathf.PI * 2f / IconFinder.MAX_RANDOM;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Vector3 spawnPosition = StartIcon.position + offset;
            var icon = Instantiate(IconPrefab, spawnPosition, Quaternion.identity, root);
            icon.SetActive(false);

            // put into list
            DisplayButton iconButtonUI = icon.GetComponent<DisplayButton>();
            iconButtons.Add(iconButtonUI);
            iconButtons[i].GetComponent<Button>().onClick.AddListener(
                () => manager.OnIconClicked(iconButtonUI));
        }
    }

    // Set images for icons and turn random icon active
    void StartGame(ImageSO target, List<ImageSO> random)
    {
        startButton.SetImageData(target);
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(true);
            iconButtons[i].SetImageData(random[i]);
        }
    }

    void UpdateLogOnClick(DisplayButton button, float elapsedTime)
    {
        logText.text = $"Icon {button.imageData.iconName} clicked at {elapsedTime:F2} sec\n";
    }

    void OnUIChanged(UI_STATE state)
    {
        switch (state)
        {
            case UI_STATE.ON_COMPLETE:
                UIComplete();
                break;
            //case UI_STATE.NO_SELECTION:
                //UINotSelect();
                //break;
            case UI_STATE.ON_ICON_CLICKED:
                UIOnQuestionDisplay();
                break;
            case UI_STATE.DONE_QUESTION:
                UINewRound();
                break;
        }

    }

    // -- On Question Display: Random icons disappear and Main UI is hidden
    void UIOnQuestionDisplay()
    {
        // set all icons to inactive and set start button to a white image
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(false);
        }
        //startButton.SetToNullImage();
        root.gameObject.SetActive(false); // Hide the root object containing icons
        questionUI.gameObject.SetActive(true); // Show the question UI
        continueButton.interactable = false;
    }

    // -- Continue after choosing mark for questions
    public void UINewRound()
    {
        StartCoroutine(DeactivateAfterSound());
    }

    public IEnumerator DeactivateAfterSound()
    {
        yield return new WaitForSeconds(0.1f);
        root.gameObject.SetActive(true);
        questionUI.gameObject.SetActive(false);
        ResetStartButton();
    }

    void UIOnQuestionDone()
    {
        // activate continue button in the question ui
        continueButton.interactable = true;
    }

    // -- On Complete: Start Btn is now Reset Btn, all other UI disappear except "Complete" txt
    void UIComplete()
    {
        countdownText.text = "Experiment Complete!";
        startButton.gameObject.SetActive(false);
        logText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(true);
    }

    // -- No Selection: No Rest. All random disappear, Start Btn interactive again
    void UINotSelect()
    {
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(false);
        }

        // Reactivate the start button
        ResetStartButton();
    }

    void ResetStartButton()
    {
        // Reactivate the start button (image + interaction)
        startButton.ResetImage();
        startButton.ResetButton();
    }

    void OnCountdownUpdate(string txt)
    {
        countdownText.text = txt;
    }
}
