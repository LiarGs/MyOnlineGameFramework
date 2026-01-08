using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayFromStartup
{
    private const string MenuPath         = "Tools/Play From Start Scene";
    private const string SettingKey       = "PlayFromStartScene_Enabled";
    private const string StartupScenePath = "Assets/Scenes/Startup.unity"; 

    static PlayFromStartup()
    {
        // 编译完成后自动同步一次状态
        EditorApplication.delayCall += _UpdatePlayModeStartScene;
    }

    [MenuItem(MenuPath)]
    public static void ToggleAction()
    {
        // 切换布尔值状态
        bool isEnabled = !EditorPrefs.GetBool(SettingKey, false);
        EditorPrefs.SetBool(SettingKey, isEnabled);
        
        _UpdatePlayModeStartScene();
        
        Debug.Log(isEnabled ? "已开启：始终从主场景启动" : "已关闭：从当前场景启动");
    }

    [MenuItem(MenuPath, true)]
    public static bool ToggleActionValidate()
    {
        // 为菜单项添加打勾状态
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(SettingKey, false));
        return true;
    }

    private static void _UpdatePlayModeStartScene()
    {
        bool isEnabled = EditorPrefs.GetBool(SettingKey, false);

        if (isEnabled)
        {
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(StartupScenePath);
            if (scene != null)
                EditorSceneManager.playModeStartScene = scene;
            else
                Debug.LogWarning($"未找到启动场景：{StartupScenePath}");
        }
        else
        {
            // 设置为 null 则恢复 Unity 默认行为（运行当前场景）
            EditorSceneManager.playModeStartScene = null;
        }
    }
}