using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IconManager : MonoBehaviour
{
    [Header("Dependency on other classes")]
    [SerializeField] IconFinder iconFinder;
    [SerializeField] DataSaver dataSaver;

    [Header("Init icons")]
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private Transform StartIcon;
    [SerializeField] private float radius = 2.0f;

    [Header("Button Functions binding")]
    //[SerializeField] private Button startButton;
    [SerializeField] private List<DisplayButton> iconButtons;
    [SerializeField] DisplayButton startButton;
    [SerializeField] DisplayButton resetButton;

    // Private fields
    [Header("Logging")]
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text countdownText;

    private float startTime;
    const int ICON_COUNT = 10;

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
            iconButtons[i].GetComponent<Button>().onClick.AddListener(() => OnIconClicked(iconButtonUI));
        }
    }

    // when user choose start icon > all icons are shown and timer is started
    public void StartGame()
    {
        startTime = Time.time;
        Debug.Log("🟢 Timer started!");

        //------Find and set image for target button
        ImageSO iconTarget = iconFinder.ChooseTargetImg();
        startButton.SetImageData(iconTarget);

        //------Find and set image for pickable buttons
        // Note: pass the images of desired same group and whether to sample from other group
        List<ImageSO> imageList = iconFinder.ChooseRandomImg();
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(true);
            iconButtons[i].SetImageData(imageList[i]);
        }

        StartCoroutine(Countdown(15.0f, MoveOnAfterNoPick));
    }

    // when icon is clicked > save data and go to rest state for 10 seconds
    void OnIconClicked(DisplayButton button)
    {
        float elapsedTime = Time.time - startTime;

        // logText.text = $"🔘 Icon {index} clicked at {elapsedTime:F2} sec\n";
        logText.text = $"Icon {button.imageData.index} clicked at {elapsedTime:F2} sec\n";
        StartCoroutine(DeactiveRandomIconAfterSound());

        // ---Save data---
        dataSaver.SaveUserData(
            iconFinder.CurrentIndex + 1,
            startButton.imageData.iconName,
            button.imageData.iconName,
            elapsedTime);
    }

    // after 10 seconds > start icon is enabled for user to continue
    IEnumerator DeactiveRandomIconAfterSound()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(false);
        }
        startButton.SetToNullImage();
        StopAllCoroutines();

        // Check if at the end of index
        if (iconFinder.IsFinish())
        {
            countdownText.text = "Experiment Complete!";
            startButton.gameObject.SetActive(false);
            logText.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(true);
        }
        else
        {
            // rest 10 secs
            StartCoroutine(Countdown(10.0f, ResetStartButton));
        }
    }

    // coundown function - can be reused for different purposes
    IEnumerator Countdown(float seconds, Action DoSomething)
    {
        float counter = seconds;

        while (counter > 0)
        {
            //Debug.Log("Time left: " + Mathf.Ceil(counter));
            countdownText.text = "Time left: " + Mathf.Ceil(counter);
            yield return new WaitForSeconds(1f); // update every second
            counter -= 1f;
        }

        countdownText.text = "";
        //Debug.Log("Countdown complete!");

        DoSomething?.Invoke();
    }

    void ResetStartButton()
    {
        // Reactivate the start button
        startButton.ResetImage();
        startButton.ResetButton();
    }

    void MoveOnAfterNoPick()
    {
        dataSaver.SaveNullData(iconFinder.CurrentIndex + 1);

        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(false);
        }

        // Reactivate the start button
        ResetStartButton();
    }

    public void ResetGame()
    {
        // Reload the scene after saving
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
