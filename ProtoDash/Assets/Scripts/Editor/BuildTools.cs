//#define CREATE_FLOW

using UnityEngine;
using UnityEditor;

public class BuildTools : EditorWindow
{
	private const string LEVEL_FLOW_PATH = "Assets/Data/";
	private const string DEFAULT_FLOW_NAME = "LevelFlow.Asset";
	private const string MAIN_FLOW_NAME = "MainLevelFlow.Asset";
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

	}
}
