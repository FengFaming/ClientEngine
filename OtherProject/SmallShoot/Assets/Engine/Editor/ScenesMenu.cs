using UnityEditor;
using UnityEditor.SceneManagement;
public static class ScenesMenu
{
[MenuItem("Tools/Update Scene List/GameStart", priority = 10)]
public static void Assets_Scenes_GameStart_unity() { ScenesMenuBuild.OpenScene("Assets/Scenes/GameStart.unity"); }
[MenuItem("Tools/Update Scene List/t1", priority = 10)]
public static void Assets_Scenes_t1_unity() { ScenesMenuBuild.OpenScene("Assets/Scenes/t1.unity"); }
}
