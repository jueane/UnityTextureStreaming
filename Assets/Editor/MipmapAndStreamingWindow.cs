using UnityEngine;
using UnityEditor;
using System.IO;

public class MipmapAndStreamingWindow : EditorWindow
{
    private string folderPath = "Assets/Textures"; // 默认路径
    private int totalTextureCount;
    private int mipStreamingEnabledCount;

    [MenuItem("Window/Mipmap and Streaming Settings")]
    static void OpenWindow()
    {
        MipmapAndStreamingWindow window = GetWindow<MipmapAndStreamingWindow>();
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Mipmap and Streaming Settings", EditorStyles.boldLabel);

        // 将当前所选中的Project窗口的目录赋值给Folder path按钮
        if (GUILayout.Button("Use Selected Folder"))
        {
            UseSelectedFolder();
        }

        // 显示当前路径
        GUILayout.Label("Current Folder Path: " + folderPath);

        // 提供文本输入框来修改路径
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        // 应用设置按钮
        if (GUILayout.Button("Apply Mipmaps and Streaming"))
        {
            ApplyMipmapsAndStreaming();
        }

        // 刷新按钮
        if (GUILayout.Button("Refresh"))
        {
            RefreshCounts();
        }

        // 显示统计信息
        GUILayout.Label("Total Textures: " + totalTextureCount);
        GUILayout.Label("Mip Streaming Enabled: " + mipStreamingEnabledCount);

        // 关闭按钮
        if (GUILayout.Button("Disable Mipmaps and Streaming"))
        {
            DisableMipmapsAndStreaming();
        }
    }

    void ApplyMipmapsAndStreaming()
    {
        // 获取指定目录下的所有贴图文件
        string[] texturePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

        totalTextureCount = texturePaths.Length;
        mipStreamingEnabledCount = 0;

        foreach (string texturePath in texturePaths)
        {
            // 导入贴图
            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (textureImporter != null)
            {
                // 启用 Mipmaps
                textureImporter.mipmapEnabled = true;

                // 启用 Mip Streaming
                textureImporter.streamingMipmaps = true;

                // 记录启用了 Mip Streaming 的贴图数量
                if (textureImporter.streamingMipmaps)
                {
                    mipStreamingEnabledCount++;
                }

                // 应用设置
                AssetDatabase.ImportAsset(texturePath);
            }
        }

        // 保存更改
        AssetDatabase.SaveAssets();
        Debug.Log("Mipmaps and Streaming settings applied to all textures in the specified folder.");
    }

    void RefreshCounts()
    {
        // 获取指定目录下的所有贴图文件
        string[] texturePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

        totalTextureCount = texturePaths.Length;
        mipStreamingEnabledCount = 0;

        foreach (string texturePath in texturePaths)
        {
            // 导入贴图
            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            // 记录启用了 Mip Streaming 的贴图数量
            if (textureImporter != null && textureImporter.streamingMipmaps)
            {
                mipStreamingEnabledCount++;
            }
        }
    }

    void DisableMipmapsAndStreaming()
    {
        // 获取指定目录下的所有贴图文件
        string[] texturePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

        foreach (string texturePath in texturePaths)
        {
            // 导入贴图
            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (textureImporter != null)
            {
                // 禁用 Mipmaps 和 Mip Streaming
                textureImporter.mipmapEnabled = false;
                textureImporter.streamingMipmaps = false;

                // 记录禁用了 Mip Streaming 的贴图数量
                if (textureImporter.streamingMipmaps)
                {
                    mipStreamingEnabledCount--;
                }

                // 应用设置
                AssetDatabase.ImportAsset(texturePath);
            }
        }

        // 保存更改
        AssetDatabase.SaveAssets();
        Debug.Log("Mipmaps and Streaming settings disabled for all textures in the specified folder.");
    }

    // 将当前所选中的Project窗口的目录赋值给Folder path
    void UseSelectedFolder()
    {
        UnityEngine.Object[] selectedObjects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        if (selectedObjects.Length > 0)
        {
            string selectedPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
            if (Directory.Exists(selectedPath))
            {
                folderPath = selectedPath;
            }
            else
            {
                Debug.LogWarning("Please select a folder in the Project window.");
            }
        }
        else
        {
            Debug.LogWarning("Please select a folder in the Project window.");
        }
    }
}
