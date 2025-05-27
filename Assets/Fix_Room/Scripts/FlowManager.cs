using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    [Header("Dependency on other classes")]
    [SerializeField] IconFinder iconFinder;

    //-----State variables-----
    private float startTime;
    private float selectedTime;
    private ImageSO iconTargetData;
    private ImageSO selectedTargetData;
    private float timeforRound = 15.0f;

    //-----Event for UI-----
    public Action<ImageSO, List<ImageSO>> OnStart;
    public Action<DisplayButton, float> OnButtonClicked;
    public Action<UI_STATE> OnUIChanged;
    public Action<string> UpdateCountdownText;

    //-----Event for Saver-----
    public Action<int, string, string, float, float, float, float> OnSaveData;

    // when user choose start icon > all icons are shown and timer is started
    public void StartGame()
    {
        startTime = Time.time;
        selectedTime = 0.0f;
        selectedTargetData = null; // reset selected target data

        Debug.Log("Timer started!");

        //------Find and set image for target button
        iconTargetData = iconFinder.ChooseTargetImg();

        //------Find and set image for pickable buttons
        List<ImageSO> imageList = iconFinder.ChooseRandomImg();

        OnStart?.Invoke(iconTargetData, imageList);
        //StartCoroutine(Countdown(15.0f, MoveOnAfterNoPick));

        StartCoroutine(Countdown(timeforRound, DisplayQuestion));
    }

    // when icon is clicked > save data and go to rest state for 10 seconds
    public void OnIconClicked(DisplayButton selectedIcon)
    {
        selectedTime = Time.time - startTime;
        selectedTargetData = selectedIcon.imageData;

        OnButtonClicked?.Invoke(selectedIcon, selectedTime); //update logger
        StartCoroutine(DeactiveRandomIconAfterSound());
    }

    // after 10 seconds > start icon is enabled for user to continue
    IEnumerator DeactiveRandomIconAfterSound()
    {
        yield return new WaitForSeconds(0.1f);
        StopAllCoroutines(); // stop the other countdown

        // Check if at the end of index
        if (iconFinder.IsFinish())
        {
            OnUIChanged?.Invoke(UI_STATE.ON_COMPLETE);
        }
        else
        {
            // rest 10 secs
            //OnUIChanged?.Invoke(UI_STATE.ON_ICON_CLICKED);
            //StartCoroutine(Countdown(10.0f, ContinueAfterRest));

            // display the questions
            DisplayQuestion();
        }
    }

    void DisplayQuestion()
    {
        OnUIChanged?.Invoke(UI_STATE.ON_ICON_CLICKED);
    }

    // coundown function - can be reused for different purposes
    IEnumerator Countdown(float seconds, Action DoSomething)
    {
        float counter = seconds;

        while (counter > 0)
        {
            //Debug.Log("Time left: " + Mathf.Ceil(counter));
            UpdateCountdownText?.Invoke("Time left: " + Mathf.Ceil(counter));
            yield return new WaitForSeconds(1f); // update every second
            counter -= 1f;
        }

        UpdateCountdownText?.Invoke("");
        DoSomething?.Invoke();
    }

    public void SaveData(float q1, float q2, float q3)
    {
        if (HavePicked())
        {
            // ---Save data---
            OnSaveData?.Invoke(iconFinder.CurrentIndex + 1,
                iconTargetData.iconName,
                selectedTargetData.iconName,
                selectedTime,
                q1, q2, q3);
        }
        else //when user don't pick any icon
        {
            // save data to null
            OnSaveData?.Invoke(iconFinder.CurrentIndex + 1,
                iconTargetData.iconName,
                "null",
                timeforRound,
                q1, q2, q3);
        }
        OnUIChanged?.Invoke(UI_STATE.DONE_QUESTION);
    }

    bool HavePicked()
    {
        return selectedTargetData != null;
    }

    public void ResetGame()
    {
        // Reload the scene after saving
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
