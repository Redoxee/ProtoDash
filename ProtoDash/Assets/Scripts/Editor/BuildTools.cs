//#define CREATE_FLOW

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class BuildTools : EditorWindow
{
	private const string LEVEL_FLOW_PATH = "Assets/Data/";
	private const string DEFAULT_FLOW_NAME = "LevelFlow.Asset";
	private const string MAIN_FLOW_NAME = "MainLevelFlow.Asset";
	private const string GAME_SCENES_PATH = "Assets/Scenes/Levels/";
	private const string GAME_SCENE_EXTENSION = ".unity";


	private LevelFlow mainLevelFlow;

	#region Create flow
#if CREATE_FLOW
	[MenuItem("Dasher/Create level flow")]
	static void DoSomething()
	{
		Debug.Log("Creating a new level flow");
		LevelFlow lf = new LevelFlow();
		AssetDatabase.CreateAsset(lf, LEVEL_FLOW_PATH + DEFAULT_FLOW_NAME);
		Debug.Log("Level flow created");
	}
#endif
	#endregion

	[MenuItem("Dasher/BuildTool")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(BuildTools));
	}

	void OnGUI()
	{
		if (GUILayout.Button("Set Scenes"))
		{
			SetSceneInProjects();
		}
	}

	void SetSceneInProjects()
	{
		EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
		AssetDatabase.ImportAsset(LEVEL_FLOW_PATH + MAIN_FLOW_NAME);
		LevelFlow lf = AssetDatabase.LoadAssetAtPath<LevelFlow>(LEVEL_FLOW_PATH + MAIN_FLOW_NAME);
		Debug.Log("Setting up level in build");

		bool sceneAdded = false;
		for (int i = 0; i < lf.levelList.Count; ++i)
		{
			LevelData levelItem = lf.levelList[i];
			Object sceneObject = levelItem.sceneObject;
			string scenePath = GAME_SCENES_PATH + sceneObject.name + GAME_SCENE_EXTENSION;
			int index = UnityEditor.ArrayUtility.FindIndex<EditorBuildSettingsScene>(buildScenes, (ebs) => { return ebs.path == scenePath; });
			if (index < 0)
			{
				EditorBuildSettingsScene newEBS = new EditorBuildSettingsScene(scenePath, true);
				UnityEditor.ArrayUtility.Add<EditorBuildSettingsScene>(ref buildScenes, newEBS);

				Debug.Log("Adding scene : " + scenePath);
				sceneAdded = true;
			}
		}
		if (sceneAdded)
		{
			EditorBuildSettings.scenes = buildScenes;
		}
	}
}