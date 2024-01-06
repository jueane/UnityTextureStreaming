using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class TextureStreamingEditor : EditorWindow
{
    int totalTextureCount;
    int mipStreamingEnabledCount;

    GUILayoutOption buttionHeight = GUILayout.Height(40);

    List<string> folderPaths = new List<string>(); // 新增一个数组用于存储目录列表
    int selectedFolderIndex = 0; // 选中的目录索引
    string selectedFolder => (folderPaths == null || folderPaths.Count < 1) ? null : folderPaths[selectedFolderIndex];

    Vector2 scrollPosition = Vector2.zero;

    int count;

    string filePath = "Assets/Editor/TextureStreamingList.txt";

    [MenuItem("Window/Texture Streaming Editor")]
    static void OpenWindow()
    {
        TextureStreamingEditor editor = GetWindow<TextureStreamingEditor>();
        editor.Show();
    }

    // 在OnGUI之前调用，初始化目录列表
    void OnEnable()
    {
        Load();
    }

    void OnGUI()
    {
        GUILayout.Label("Texture Streaming Settings", EditorStyles.boldLabel);

        // 添加文本框
        GUILayout.Label($"File Path: {filePath}");

        GUILayout.BeginHorizontal();

        // 新增按钮，用于增加目录
        if (GUILayout.Button("Add Folder", buttionHeight))
        {
            // 使用 "Use Selected Folder" 的点击选中逻辑
            AddFolder();
            Save();
        }
        // 新增按钮，用于删除目录
        if (GUILayout.Button("Remove Folder", buttionHeight))
        {
            RemoveFolder();
            Save();
        }

        // 刷新按钮
        if (GUILayout.Button("Refresh", buttionHeight))
        {
            RefreshCounts(selectedFolder);
        }

        // 应用设置按钮
        if (GUILayout.Button("Enable", buttionHeight))
        {
            ApplyMipmapsAndStreaming(selectedFolder);
        }
        // 关闭按钮
        if (GUILayout.Button("Disable", buttionHeight))
        {
            DisableMipmapsAndStreaming(selectedFolder);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // 显示统计信息
        GUILayout.Label("Total Textures: " + totalTextureCount);
        GUILayout.Label("Mip Streaming Enabled: " + mipStreamingEnabledCount);

        GUILayout.Space(20);
        GUILayout.Label("Folder list", EditorStyles.boldLabel);
        GUILayout.Label($"Folder selected: {selectedFolder}");
        // 显示目录列表
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
        GUILayout.Space(10);
        if (folderPaths != null)
        {
            for (int i = 0; i < folderPaths.Count; i++)
            {
                GUILayout.BeginHorizontal();
                bool isSelected = i == selectedFolderIndex;
                isSelected = GUILayout.Toggle(isSelected, folderPaths[i], "Button");
                if (isSelected)
                {
                    selectedFolderIndex = i;
                    // folderPath = folderPaths[selectedFolderIndex];
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
        GUILayout.Space(10);
    }

    // 将当前所选中的Project窗口的目录赋值给Folder path
    void AddFolder()
    {
        UnityEngine.Object[] selectedObjects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        if (selectedObjects.Length > 0)
        {
            string selectedPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
            if (Directory.Exists(selectedPath))
            {
                folderPaths.Add(selectedPath);
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

    void RemoveFolder()
    {
        if (folderPaths.Count > 0)
        {
            // 移除选中的目录
            folderPaths = folderPaths.Where((path, index) => index != selectedFolderIndex).ToList();

            // 更新选中的目录索引
            if (selectedFolderIndex >= folderPaths.Count)
            {
                selectedFolderIndex = folderPaths.Count - 1;
            }
        }
    }

    void RefreshCounts(string folderPath)
    {
        var texturePaths = FindTextures(folderPath);

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

    string[] FindTextures(string folderPath)
    {
        List<string> texList = new List<string>();

        // 获取指定目录下的所有贴图文件（包括子目录）
        string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { folderPath });

        foreach (string guid in guids)
        {
            // 转换Asset路径为实际文件系统路径
            string fullPath = AssetDatabase.GUIDToAssetPath(guid);
            texList.Add(fullPath);
        }

        totalTextureCount = texList.Count;
        return texList.ToArray();
    }

    void ApplyMipmapsAndStreaming(string folderPath)
    {
        // 获取指定目录下的所有贴图文件
        string[] texturePaths = FindTextures(folderPath);

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
        Debug.Log($"{mipStreamingEnabledCount} Textures Mipmaps and Streaming settings applied to all textures in {folderPath}.");
    }

    void DisableMipmapsAndStreaming(string folderPath)
    {
        // 获取指定目录下的所有贴图文件
        string[] texturePaths = FindTextures(folderPath);

        var disableStreamingCount = 0;

        foreach (string texturePath in texturePaths)
        {
            // 导入贴图
            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (textureImporter != null)
            {
                // 禁用 Mipmaps 和 Mip Streaming
                textureImporter.mipmapEnabled = false;
                textureImporter.streamingMipmaps = false;

                // 应用设置
                AssetDatabase.ImportAsset(texturePath);

                disableStreamingCount++;
            }
        }

        // 保存更改
        AssetDatabase.SaveAssets();
        Debug.Log($"{disableStreamingCount} Textures Mipmaps and Streaming settings disabled in {folderPath}.");
    }

    void Load()
    {
        if (File.Exists(filePath))
            folderPaths = File.ReadAllLines(filePath).ToList();
    }

    void Save()
    {
        File.WriteAllLines(filePath, folderPaths);
        AssetDatabase.Refresh();
    }
}
