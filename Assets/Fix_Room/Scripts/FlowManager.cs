using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    [Header("Dependency on other classes")]
    [SerializeField] IconFinder iconFinder;

    private float startTime;

    //-----Event for UI-----
    public Action<ImageSO, List<ImageSO>> OnStart;
    public Action<DisplayButton, float> OnButtonClicked;
    public Action<UI_STATE> OnUIChanged;
    public Action<string> UpdateCountdownText;

    //-----Event for Saver-----
    public Action<int, string, string, float> OnSaveData;

    // when user choose start icon > all icons are shown and timer is started
    public void StartGame()
    {
        startTime = Time.time;
        Debug.Log("🟢 Timer started!");

        //------Find and set image for target button
        ImageSO iconTarget = iconFinder.ChooseTargetImg();

        //------Find and set image for pickable buttons
        List<ImageSO> imageList = iconFinder.ChooseRandomImg();

        OnStart?.Invoke(iconTarget, imageList);
        StartCoroutine(Countdown(15.0f, MoveOnAfterNoPick));
    }

    // when icon is clicked > save data and go to rest state for 10 seconds
    public void OnIconClicked(DisplayButton start, DisplayButton button)
    {
        float elapsedTime = Time.time - startTime;
        OnButtonClicked?.Invoke(button, elapsedTime);
        
        StartCoroutine(DeactiveRandomIconAfterSound());

        // ---Save data---
        OnSaveData?.Invoke(iconFinder.CurrentIndex + 1,
            start.imageData.iconName,
            button.imageData.iconName,
            elapsedTime);
    }

    // after 10 seconds > start icon is enabled for user to continue
    IEnumerator DeactiveRandomIconAfterSound()
    {
        yield return new WaitForSeconds(0.1f);
        OnUIChanged?.Invoke(UI_STATE.ICON_CLICKED);
        StopAllCoroutines();

        // Check if at the end of index
        if (iconFinder.IsFinish())
        {
            OnUIChanged?.Invoke(UI_STATE.COMPLETE_UI);
        }
        else
        {
            // rest 10 secs
            StartCoroutine(Countdown(10.0f, ContinueAfterRest));
        }
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

    void ContinueAfterRest()
    {
        // change UI
        OnUIChanged?.Invoke(UI_STATE.CONTINUE);
    }

    void MoveOnAfterNoPick()
    {
        // save data to null
        OnSaveData(iconFinder.CurrentIndex + 1, "null", "null", -1.0f);

        // change UI
        OnUIChanged?.Invoke(UI_STATE.MOVE_ON_UI);
    }

    public void ResetGame()
    {
        // Reload the scene after saving
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
