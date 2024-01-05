using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InitQuality();

        // 获取当前的质量级别
        int currentQualityLevel = QualitySettings.GetQualityLevel();
        Debug.Log("Current Quality Level: " + currentQualityLevel);

        // 获取所有可用的质量级别
        int totalQualityLevels = QualitySettings.names.Length;
        Debug.Log("Total Quality Levels: " + totalQualityLevels);

        for (int i = 0; i < totalQualityLevels; i++)
        {
            string qualityLevelName = QualitySettings.names[i];
            Debug.Log("Quality Level " + i + ": " + qualityLevelName);
        }

        SceneManager.LoadScene("Scenes/Logic");
    }

    public static void SetQuality()
    {
    }

    void InitQuality()
    {
        // 加载保存的质量级别
        int savedQualityLevel = PlayerPrefs.GetInt("SavedQualityLevel", -1);

        // 如果找到保存的质量级别，则应用它
        if (savedQualityLevel != -1)
        {
            QualitySettings.SetQualityLevel(savedQualityLevel);
            Debug.Log("Applied saved Quality Level: " + savedQualityLevel);
        }
        else
        {
            Debug.Log("No saved Quality Level found.");
        }
    }

    // 在退出应用程序或需要保存时调用此方法
    void OnApplicationQuit()
    {
        // 保存当前质量级别
        int currentQualityLevel = QualitySettings.GetQualityLevel();
        PlayerPrefs.SetInt("SavedQualityLevel", currentQualityLevel);
        PlayerPrefs.Save();
        Debug.Log("Saved Quality Level: " + currentQualityLevel);
    }
}
