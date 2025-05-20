using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public enum UI_STATE
{
    MOVE_ON_UI,
    COMPLETE_UI,
    ICON_CLICKED,
    CONTINUE
}

public class UIHandler : MonoBehaviour
{
    [SerializeField] FlowManager manager;

    [Header("Init icons")]
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private Transform StartIcon;
    [SerializeField] private float radius = 2.0f;

    // Private fields
    [Header("Logging")]
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text countdownText;

    [Header("Button Functions binding")]
    [SerializeField] private List<DisplayButton> iconButtons;
    [SerializeField] DisplayButton startButton;
    [SerializeField] DisplayButton resetButton;

    const int ICON_COUNT = 10;

    void OnEnable()
    {
        manager.OnStart += StartGame;
        manager.OnButtonClicked += OnClicked;
        manager.OnUIChanged += OnUIChanged;
        manager.UpdateCountdownText += OnCountdownUpdate;
    }

    void Start()
    {
        resetButton.gameObject.SetActive(false);
        InitIconPrefab();
    }

    // initialize 10 icon prefab
    private void InitIconPrefab()
    {
        for (int i = 0; i < ICON_COUNT; i++)
        {
            // init prefab
            float angle = i * Mathf.PI * 2f / ICON_COUNT;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Vector3 spawnPosition = StartIcon.position + offset;
            var icon = Instantiate(IconPrefab, spawnPosition, Quaternion.identity, transform);
            icon.SetActive(false);

            // put into list
            DisplayButton iconButtonUI = icon.GetComponent<DisplayButton>();
            iconButtons.Add(iconButtonUI);
            iconButtons[i].GetComponent<Button>().onClick.AddListener(
                () => manager.OnIconClicked(startButton, iconButtonUI));
        }
    }

    void StartGame(ImageSO target, List<ImageSO> random)
    {
        startButton.SetImageData(target);
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(true);
            iconButtons[i].SetImageData(random[i]);
        }
    }

    void OnClicked(DisplayButton button, float elapsedTime)
    {
        logText.text = $"Icon {button.imageData.index} clicked at {elapsedTime:F2} sec\n";
    }

    void OnUIChanged(UI_STATE state)
    {
        switch (state)
        {
            case UI_STATE.COMPLETE_UI:
                UIComplete();
                break;
            case UI_STATE.MOVE_ON_UI:
                UIMoveOn();
                break;
            case UI_STATE.ICON_CLICKED:
                UIAfterClick();
                break;
            case UI_STATE.CONTINUE:
                UIContinue();
                break;
        }

    }

    void UIAfterClick()
    {
        // set all icons to inactive and set start button to a white image
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(false);
        }
        startButton.SetToNullImage();
    }

    void UIComplete()
    {
        countdownText.text = "Experiment Complete!";
        startButton.gameObject.SetActive(false);
        logText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(true);
    }

    void UIMoveOn()
    {
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(false);
        }

        // Reactivate the start button
        ResetStartButton();
    }

    void UIContinue()
    {
        ResetStartButton();
    }

    void ResetStartButton()
    {
        // Reactivate the start button
        startButton.ResetImage();
        startButton.ResetButton();
    }

    void OnCountdownUpdate(string txt)
    {
        countdownText.text = txt;
    }
}
