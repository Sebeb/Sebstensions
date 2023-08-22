using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;


public class GameManager : OdinMenuEditorWindow
{
	[MenuItem("Tools/Game Manager", priority = -999999)]
	private static void OpenEditor()
	{
		GameManager window = GetWindow<GameManager>("Game Manager");
		window.titleContent = new UnityEngine.GUIContent("Game Manager", EditorIcons.SettingsCog.Active);
	}
	

	protected override OdinMenuTree BuildMenuTree()
	{
		OdinMenuTree tree = new(false,
			config: new OdinMenuTreeDrawingConfig
			{
				DefaultMenuStyle = new OdinMenuStyle
					{ Borders = true, BorderPadding = 200, BorderAlpha = 1 }
			});

		//Recursively add Scriptable Objects from the Assets/Data folder, ensuring that the path is correct
		tree.AddAllAssetsAtPath("", "Assets/Resources/Managers", typeof(ScriptableMonoObject), true, false);

		return tree;
	}
}