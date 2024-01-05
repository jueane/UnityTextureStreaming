using UnityEngine;
using UnityEngine.Profiling;

public class TextureMemoryUsage : MonoBehaviour
{
    public Texture2D yourTexture; // 替换为您的贴图

    void Start()
    {
        yourTexture = GetComponent<Renderer>().material.mainTexture as Texture2D;


        if (yourTexture != null)
        {
            // 获取贴图的内存占用
            long memorySize = Profiler.GetRuntimeMemorySizeLong(yourTexture);

            // 将字节转换为更易读的单位（例如KB、MB）
            string formattedMemorySize = FormatMemorySize(memorySize);

            // 输出内存占用
            Debug.Log("Texture Memory Usage: " + formattedMemorySize);
        }
        else
        {
            Debug.LogWarning("Texture is null. Assign a valid texture to yourTexture.");
        }
    }

    // 辅助方法：将字节数转换为更易读的单位
    private string FormatMemorySize(long bytes)
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
