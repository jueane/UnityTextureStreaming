using UnityEngine;
using IngameDebugConsole;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TestScript : MonoBehaviour
{
    [ConsoleMethod("cube", "Creates a cube at specified position")]
    public static void CreateCubeAt(Vector3 position)
    {
        GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = position;
    }

    [ConsoleMethod("cq", "change quality")]
    public static void ChangeQuality(int q)
    {
        QualitySettings.SetQualityLevel(q);

        // 保存当前质量级别
        int currentQualityLevel = QualitySettings.GetQualityLevel();
        PlayerPrefs.SetInt("SavedQualityLevel", currentQualityLevel);
        PlayerPrefs.Save();
        Debug.Log("Saved Quality Level: " + currentQualityLevel);
    }

    [ConsoleMethod("load", "load asset")]
    public static void ChangeQuality(string name)
    {
        var path = $"Assets/Prefabs/{name}.prefab";
        Addressables.LoadAssetAsync<GameObject>(path).Completed += OnCompleted;
    }

    static void OnCompleted(AsyncOperationHandle<GameObject> obj)
    {
        var asset = obj.Result;
        var newObject = GameObject.Instantiate(asset);

        var x = Random.Range(-1f, 1f);
        var y = Random.Range(0.5f, 1.5f);

        newObject.transform.position = new Vector3(x, y, -8f);
        // newObject.transform.SetParent();
        // newObject.transform.localPosition = Vector3.zero;

        newObject.transform.localScale = Vector3.one;
        newObject.transform.rotation = Quaternion.identity;
    }

    [ConsoleMethod("m", "totalMemory")]
    public static void test11()
    {
        // 获取应用占用的总内存（包括资源）
        long totalMemory = Profiler.GetTotalAllocatedMemoryLong();

        // 将字节转换为更易读的单位（例如KB、MB）
        string formattedTotalMemory = FormatMemorySize(totalMemory);

        // 输出内存占用
        Debug.Log("Total Memory Usage: " + formattedTotalMemory);
    }

    // 辅助方法：将字节数转换为更易读的单位
    static string FormatMemorySize(long bytes)
    {
        const long KB = 1024;
        const long MB = 1024 * KB;
        const long GB = 1024 * MB;

        if (bytes > GB)
        {
            return string.Format("{0:F2} GB", (float)bytes / GB);
        }
        else if (bytes > MB)
        {
            return string.Format("{0:F2} MB", (float)bytes / MB);
        }
        else if (bytes > KB)
        {
            return string.Format("{0:F2} KB", (float)bytes / KB);
        }
        else
        {
            return bytes + " Bytes";
        }
    }
}
