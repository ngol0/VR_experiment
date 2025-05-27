using System.IO;
using UnityEngine;

public class UserData
{
    int userId;
    int round;
    string targetIndex;
    string pickedIndex;
    float seconds;

    float question1;
    float question2;
    float question3;

    // public override string ToString()
    // {
    //     // Text format — change this as needed
    //     return $"Round {round + 1}:\n - Target Icon Index: {targetIndex}\n - Selected Icon Index: {pickedIndex}\n - Response Time: {seconds} seconds";
    // }

    public string ToCSV()
    {
        return $"{userId},{round},{targetIndex},{pickedIndex},{seconds},{question1},{question2},{question3}";
    }

    public UserData(int id, int rnd, string tIndex, string pIndex, float secs, float q1, float q2, float q3)
    {
        userId = id;
        round = rnd;
        targetIndex = tIndex;
        pickedIndex = pIndex;
        seconds = secs;
        question1 = q1;
        question2 = q2;
        question3 = q3;
    }
}

public class DataSaver : MonoBehaviour
{
    //private string txtPath;
    string csvPath;
    int userId;

    [SerializeField] FlowManager manager;

    void OnEnable()
    {
        manager.OnSaveData += SaveUserData;
    }

    void Start()
    {
        csvPath = Path.Combine(Application.persistentDataPath, "UserDataLog.csv");
        Debug.Log(csvPath);

        // If the log file does not exist, reset the user ID
        if (!File.Exists(csvPath))
        {
            Debug.Log("Log file not found — resetting UserID.");
            ResetUserId();
        }

        /// Get user/session number, increment and save it
        userId = PlayerPrefs.GetInt("UserID", 0) + 1;
        PlayerPrefs.SetInt("UserID", userId);
        PlayerPrefs.Save();
    }

    public void SaveUserData(
        int round, string targetName, string selectedName, float time, 
        float q1, float q2, float q3)
    {
        UserData data = new UserData(
            userId, round, targetName, selectedName, time, q1, q2, q3);
        SaveToCSV(data);
    }

    void SaveToCSV(UserData data)
    {
        // Add header once
        if (!File.Exists(csvPath))
            File.WriteAllText(csvPath, "UserID,Round,TargetIndex,PickedIndex,Seconds,Q1,Q2,Q3\n");

        File.AppendAllText(csvPath, data.ToCSV() + "\n");
    }

    public void ResetUserId()
    {
        PlayerPrefs.SetInt("UserID", 0);
        PlayerPrefs.Save();
    }
}
