using System.IO;
using UnityEngine;

public class UserData
{
    int userId;
    int round;
    string targetIndex;
    string pickedIndex;
    float seconds;

    public override string ToString()
    {
        // Text format — change this as needed
        return $"Round {round + 1}:\n - Target Icon Index: {targetIndex}\n - Selected Icon Index: {pickedIndex}\n - Response Time: {seconds} seconds";
    }

    public string ToCSV()
{
    return $"{userId},{round},{targetIndex},{pickedIndex},{seconds}";
}

    public UserData(int id, int rnd, string tIndex, string pIndex, float secs)
    {
        userId = id;
        round = rnd;
        targetIndex = tIndex;
        pickedIndex = pIndex;
        seconds = secs;
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
        //txtPath = Path.Combine(Application.persistentDataPath, "UserDataLog.txt");
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

        // Append a header for this user/session in txt file
        //File.AppendAllText(txtPath, $"\n*User {userId}\n");
    }

    public void SaveUserData(int round, string targetName, string selectedName, float time)
    {
        UserData data = new UserData(userId, round, targetName, selectedName, time);
        SaveToCSV(data);
        //SaveToTxt(data);
    }

    void SaveToCSV(UserData data)
    {
        // Add header once
        if (!File.Exists(csvPath))
            File.WriteAllText(csvPath, "UserID,Round,TargetIndex,PickedIndex,Seconds\n");

        File.AppendAllText(csvPath, data.ToCSV() + "\n");
    }

    public void ResetUserId()
    {
        PlayerPrefs.SetInt("UserID", 0);
        PlayerPrefs.Save();
    }
}
