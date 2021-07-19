using UnityEditor;
using UnityEngine;
public class SettingsWindow : EditorWindow
{
    private static SettingsManager settings => SettingsManager._i;
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;


    // Add menu named "My Window" to the Window menu
    [MenuItem("Game/Settings", priority = -1)]
    static void Init()
    {

        // Get existing open window or if none, make a new one:
        SettingsWindow window
            = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow), false,
                "Game Settings");
        window.Show();
    }

    private void OnGUI()
    {
        if (settings == null)
        {
            GUILayout.Label("Could not load settings singleton", new GUIStyle{alignment = TextAnchor.MiddleCenter});
            return;
        }

        EditorGUILayout.BeginVertical(new GUIStyle
        {
            margin = new RectOffset(5, 15, 3, 3)
        });

        SerializedObject so = new SerializedObject(settings);
        for (int i = 0; i < settings.boxes.Count; i++)
        {
            SerializedProperty sp = so.FindProperty($"boxes.Array.data[{i}]");

            EditorGUILayout.PropertyField(sp, true);

            EditorGUILayout.Separator();
        }

        EditorGUILayout.EndVertical();

        so.ApplyModifiedProperties();

    }

}