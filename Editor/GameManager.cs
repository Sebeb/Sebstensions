using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;


public class GameManager : OdinMenuEditorWindow
{
	[MenuItem("Tools/Game Manager %&g", priority = -999999)]
	private static void OpenEditor()
	{
		window = GetWindow<GameManager>("Game Manager", true);
		window.minSize = new Vector2(300, 300);
		window.titleContent = new GUIContent("Game Manager", EditorIcons.SettingsCog.Active);
	}

	protected override void DrawEditors()
	{
		base.DrawEditors();
		if (window != null)
		{
			window.titleContent = new GUIContent("Game Manager", EditorIcons.SettingsCog.Active);
		}
	}

	private static GameManager window;
	private static OdinMenuTree tree;
	protected override OdinMenuTree BuildMenuTree()
	{
		tree = new OdinMenuTree(false,
			config: new OdinMenuTreeDrawingConfig
			{
				AutoHandleKeyboardNavigation = false,
				SelectMenuItemsOnMouseDown = true,
				DefaultMenuStyle = OdinMenuStyle.TreeViewStyle
			});

		//Recursively add Scriptable Objects from the Assets/Data folder, ensuring that the path is correct
		tree.AddAllAssetsAtPath("", "Assets/Resources/Managers", typeof(ScriptableMonoObject), true, false);
		tree.AddAllAssetsAtPath("", "Assets/Resources (Editor)/Managers", typeof(ScriptableMonoObject), true,
			false);
		tree.SortMenuItemsByName();

		return tree;
	}
}