//#define CREATE_FLOW
//#define CREATE_BUILD_DATA
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using Dasher;
using UnityEditorInternal;

namespace DasherTool
{

	public class BuildTools : EditorWindow
	{
		private const string DATA_PATH = "Assets/Data/";
		private const string MAIN_FLOW_NAME = "MainLevelFlow.Asset";
		private const string GAME_SCENES_PATH = "Assets/Scenes/Levels/";
		private const string GAME_SCENE_EXTENSION = ".unity";

		private const string INTRO_SCENE_PATH = "Assets/Scenes/MainMenu.unity";


		private LevelFlow mainLevelFlow;

		private bool m_IncrementPatch = true;
		private bool m_isDevelopementBuild = true;
		private bool m_androidAutoPlay = true;

		#region Create flow

		private const string DEFAULT_FLOW_NAME = "LevelFlow.Asset";
#if CREATE_FLOW
	[MenuItem("Dasher/Create level flow")]
	static void S_CreateLevelFlow()
	{
		Debug.Log("Creating a new level flow");
		LevelFlow lf = new LevelFlow();
		AssetDatabase.CreateAsset(lf, DATA_PATH + DEFAULT_FLOW_NAME);
		Debug.Log("Level flow created");
	}
#endif
		#endregion

		#region Create build data

		const string BUILD_DATA_NAME = "BuildData.Asset";
#if CREATE_BUILD_DATA

		[MenuItem("Dasher/Create build data")]
		static void S_CreateBuildData()
		{
			Debug.Log("Creating a new build data");
			BuildData lf = ScriptableObject.CreateInstance<BuildData>();
			AssetDatabase.CreateAsset(lf, DATA_PATH + BUILD_DATA_NAME);
			Debug.Log("Build Data created");
		}

#endif

		#endregion

		[MenuItem("Dasher/BuildTool")]
		public static void ShowWindow()
		{
			var window = EditorWindow.GetWindow(typeof(BuildTools));
			window.name = "Dasher Tool";
		}

		void OnGUI()
		{
			if (GUILayout.Button("Setup Scenes in Build"))
			{
				SetSceneInProjects();
			}

			if (GUILayout.Button("Go to intro scene"))
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorSceneManager.OpenScene(INTRO_SCENE_PATH,OpenSceneMode.Single);
				}
			}

			m_IncrementPatch		= GUILayout.Toggle(m_IncrementPatch			, "Auto Increment Patch");
			m_isDevelopementBuild	= GUILayout.Toggle(m_isDevelopementBuild	, "Is Developpement Build");
			m_androidAutoPlay		= GUILayout.Toggle(m_androidAutoPlay		, "Android Auto push");


			if (GUILayout.Button("Build Android"))
			{
				BuildForAndroid(m_isDevelopementBuild, m_IncrementPatch, m_androidAutoPlay);
			}
		}
		#region Set scenes in builds
		void SetSceneInProjects()
		{
			EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
			AssetDatabase.ImportAsset(DATA_PATH + MAIN_FLOW_NAME);
			LevelFlow lf = AssetDatabase.LoadAssetAtPath<LevelFlow>(DATA_PATH + MAIN_FLOW_NAME);
			Debug.Log("Setting up level in build");

			bool sceneAdded = false;
			bool sceneNameChanged = false;
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
				if (sceneObject.name != lf.levelList[i].sceneName)
				{
					levelItem.sceneName = sceneObject.name;
					lf.levelList[i] = levelItem;
					sceneNameChanged = true;
				}
			}
			if (sceneAdded)
			{
				EditorBuildSettings.scenes = buildScenes;
			}
			if (sceneNameChanged)
			{
				EditorUtility.SetDirty(lf);
				AssetDatabase.SaveAssets();
			}
		}
		#endregion

		#region Builds

		const string c_buildFolder = "../../DasherBuilds/";
		const string c_buildNameTemplate = "Dasher_{0:D}_{1:D2}_{2:D3}";

		string[] StandardSetup()
		{
			SetSceneInProjects();

			UnityEditor.PlayerSettings.runInBackground = false;
			EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
			string[] scenes = new string[buildScenes.Length];
			for (int i = 0; i < buildScenes.Length; ++i)
			{
				scenes[i] = buildScenes[i].path;
			}

			return scenes;
		}

		string GetVersionName()
		{
			BuildData bd = AssetDatabase.LoadAssetAtPath<BuildData>(DATA_PATH + BUILD_DATA_NAME);
			return string.Format(c_buildNameTemplate, bd.Version, bd.Revision, bd.Patch);
		}

		void IncrementPatch()
		{
			BuildData bd = AssetDatabase.LoadAssetAtPath<BuildData>(DATA_PATH + BUILD_DATA_NAME);
			bd.Patch += 1;
			AssetDatabase.SaveAssets();
		}

		BuildOptions GetOptions(bool isDevelopementBuild)
		{
			BuildOptions bo = BuildOptions.None;
			if (isDevelopementBuild)
			{
				bo = bo | BuildOptions.Development | BuildOptions.AllowDebugging;
			} 
			return bo;
		}

		#region Android
		const string c_androidFolder = "Android/";
		const string c_androidExtension = ".apk";

		public void BuildForAndroid(bool isDebug,bool increment, bool autoRun)
		{
			string buildPath = c_buildFolder + c_androidFolder + GetVersionName();

			if(!Directory.Exists(buildPath))
			{
				Directory.CreateDirectory(buildPath);
			}
			string buildName = buildPath + "/" + GetVersionName() + c_androidExtension;

			string[] scenes = StandardSetup();
			BuildOptions bo = GetOptions(isDebug);

			if (autoRun)
			{
				bo = bo | BuildOptions.AutoRunPlayer;
			}

			string buildResult = BuildPipeline.BuildPlayer(scenes, buildName, BuildTarget.Android,bo);

			if (string.IsNullOrEmpty(buildResult))
			{
				UnityEngine.Debug.Log("Android build complete" + buildName);
				if (increment)
				{
					IncrementPatch();
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Error building Android:\n" + buildResult);
			}
		}
		#endregion
		#endregion
	}
}