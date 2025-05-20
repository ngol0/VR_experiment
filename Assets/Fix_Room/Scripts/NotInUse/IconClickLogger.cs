using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IconClickLogger : MonoBehaviour
{
    public Button startButton;
    public List<Button> iconButtons;

    private float startTime;
    private bool isTiming = false;

    // 紀錄資料：每次按下的 index 與經過時間
    private List<(int index, float time)> logData = new List<(int, float)>();
        public TMP_Text logText; // ⬅️ 把記錄顯示在這裡

    void Start()
    {
        // 綁定 Start 按鈕事件
        startButton.onClick.AddListener(StartTimer);

        // 綁定每個 Icon 按鈕事件
        for (int i = 0; i < iconButtons.Count; i++)
        {
            int index = i; // 本地變數避免閉包問題
            iconButtons[i].onClick.AddListener(() => OnIconClicked(index));
        }
    }

    void StartTimer()
    {
        startTime = Time.time;
        isTiming = true;
        logData.Clear();
        Debug.Log("🟢 Timer started!");
        for (int i = 0; i < iconButtons.Count; i++)
        {
            iconButtons[i].gameObject.SetActive(true);
        }
    }

    void OnIconClicked(int index)
    {
        if (!isTiming) return;

        float elapsedTime = Time.time - startTime;
        logData.Add((index, elapsedTime));

        // 即時更新 logText
        logText.text = $"🔘 Icon {index} clicked at {elapsedTime:F2} sec\n";
    }

    // 你可以額外提供一個方法導出 logData 或列印全部
    public void PrintLog()
    {
        foreach (var entry in logData)
        {
            Debug.Log($"Icon {entry.index} clicked at {entry.time:F2} seconds.");
        }
    }
}
