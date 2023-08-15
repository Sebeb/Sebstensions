using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;


public class GameSettingsManager : OdinMenuEditorWindow
{
	[MenuItem("Tools/Game Settings", priority = -999999)]
	private static void OpenEditor()
	{
		GameSettingsManager window = GetWindow<GameSettingsManager>("Game Settings");
		window.titleContent = new UnityEngine.GUIContent("Game Settings", EditorIcons.SettingsCog.Active);

	}

	protected override OdinMenuTree BuildMenuTree()
	{
		var tree = new OdinMenuTree();

		//Recurively add Scriptable Objects from the Assets/Data folder, ensuring that the path is correct
		tree.AddAllAssetsAtPath("", "Assets/Data/Settings", typeof(SettingsScriptable), true, false);

		return tree;
	}
}