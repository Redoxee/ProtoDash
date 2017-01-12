//#define CREATE_FLOW
//#define CREATE_BUILD_DATA
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using Dasher;
using UnityEditorInternal;
using System;
using System.Diagnostics;

namespace DasherTool
{

	public class BuildTools : EditorWindow
	{
		private const string DATA_PATH = "Assets/Data/";
		private const string MAIN_FLOW_NAME = "MainLevelFlow.Asset";
		private const string GAME_SCENES_PATH = "Assets/Scenes/Levels/";
		private const string GAME_SCENE_EXTENSION = ".unity";

		private const string INTRO_SCENE_PATH = "Assets/Scenes/MainScene.unity";


		private LevelFlow mainLevelFlow;

		private bool m_IncrementPatch = true;
		private bool m_isDevelopementBuild = false;
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
		void Awake()
		{

			AssetDatabase.ImportAsset(DATA_PATH + MAIN_FLOW_NAME);
			mainLevelFlow = AssetDatabase.LoadAssetAtPath<LevelFlow>(DATA_PATH + MAIN_FLOW_NAME);
		}

		void OnGUI()
		{
			mainLevelFlow = EditorGUILayout.ObjectField("Level flow", mainLevelFlow, typeof(LevelFlow), false) as LevelFlow;
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

			if (GUILayout.Button("Push last build Android"))
			{
				PushLastAndroidBuild();
			}

			if (GUILayout.Button("Build Web"))
			{
				BuildForWeb(m_isDevelopementBuild, m_IncrementPatch);
			}
		}

		#region Set scenes in builds
		void SetSceneInProjects()
		{
			EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
			UnityEngine.Debug.Log("Setting up level in build");

			bool sceneAdded = false;
			bool sceneNameChanged = false;
			int levelCount = mainLevelFlow.GetLevelCount();
			for (int i = 0; i < levelCount; ++i)
			{
				LevelData levelItem = mainLevelFlow.LevelList[i];
				UnityEngine.Object sceneObject = levelItem.sceneObject;
				string scenePath = GAME_SCENES_PATH + sceneObject.name + GAME_SCENE_EXTENSION;
				int index = UnityEditor.ArrayUtility.FindIndex<EditorBuildSettingsScene>(buildScenes, (ebs) => { return ebs.path == scenePath; });
				if (index < 0)
				{
					EditorBuildSettingsScene newEBS = new EditorBuildSettingsScene(scenePath, true);
					UnityEditor.ArrayUtility.Add<EditorBuildSettingsScene>(ref buildScenes, newEBS);

					UnityEngine.Debug.Log("Adding scene : " + scenePath);
					sceneAdded = true;
				}
				if (sceneObject.name != levelItem.sceneName)
				{
					levelItem.sceneName = sceneObject.name;
					mainLevelFlow.LevelList[i] = levelItem;
					sceneNameChanged = true;
				}
			}
			if (sceneAdded)
			{
				EditorBuildSettings.scenes = buildScenes;
			}
			if (sceneNameChanged)
			{
				EditorUtility.SetDirty(mainLevelFlow);
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
			//Undo.RecordObject(bd, "patchIncrement");
			bd.Patch += 1;
			EditorUtility.SetDirty(bd);
			AssetDatabase.SaveAssets();
		}

		//[MenuItem("Dasher/Debug Increment patch")]
		//static void S_IncrementPatch()
		//{
		//	BuildData bd = AssetDatabase.LoadAssetAtPath<BuildData>(DATA_PATH + BUILD_DATA_NAME);
		//	bd.Patch += 1;
		//	EditorUtility.SetDirty(bd);
		//	AssetDatabase.SaveAssets();
		//}

		BuildOptions GetOptions(bool isDevelopementBuild)
		{
			BuildOptions bo = BuildOptions.None;
			if (isDevelopementBuild)
			{
				bo = bo | BuildOptions.Development | BuildOptions.AllowDebugging;
			} 
			return bo;
		}

		#region Platforms

		#region Android
		const string c_androidFolder = "Android/";
		const string c_androidExtension = ".apk";

		private string m_lastBuildName = null;

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

			UnityEngine.Debug.Log("Android building : " + buildName);

			string buildResult = BuildPipeline.BuildPlayer(scenes, buildName, BuildTarget.Android,bo);

			if (string.IsNullOrEmpty(buildResult))
			{
				UnityEngine.Debug.Log("Android build complete : " + buildName);
				m_lastBuildName = GetVersionName();

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

		public void PushLastAndroidBuild()
		{
			string dataPath = Application.dataPath;
			string projectpath = dataPath.Substring(0, dataPath.Length - ("ProtoDash/ProtoDash/Assets").Length);
			string apkLocation = projectpath + "DasherBuilds/Android/" + m_lastBuildName + "/" + m_lastBuildName + c_androidExtension;
			if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
			{
				UnityEngine.Debug.LogError("Cannot find .apk file : " + apkLocation );
				return;
			}
			PlayerPrefs.SetString("APK location", apkLocation);

			string adbLocation = PlayerPrefs.GetString("Android debug bridge location");
			if (string.IsNullOrEmpty(apkLocation) || !File.Exists(adbLocation))
				adbLocation = EditorUtility.OpenFilePanel("Android debug bridge", Environment.CurrentDirectory, "exe");
			if (string.IsNullOrEmpty(apkLocation) || !File.Exists(adbLocation))
			{
				UnityEngine.Debug.LogError("Cannot find adb.exe.");
				return;
			}
			PlayerPrefs.SetString("Android debug bridge location", adbLocation);

			ProcessStartInfo info = new ProcessStartInfo
			{
				FileName = adbLocation,
				Arguments = string.Format("install -r \"{0}\"", apkLocation),
				WorkingDirectory = Path.GetDirectoryName(adbLocation),
			};
			Process.Start(info);
		}

		#endregion

		#region Web
		const string c_webFolder = "Web/";

		public void BuildForWeb(bool isDebug, bool increment)
		{
			string buildPath = c_buildFolder + c_webFolder + GetVersionName();

			if (!Directory.Exists(buildPath))
			{
				Directory.CreateDirectory(buildPath);
			}
			string buildName = buildPath + "/" + GetVersionName();

			string[] scenes = StandardSetup();
			BuildOptions bo = GetOptions(isDebug);

			string buildResult = BuildPipeline.BuildPlayer(scenes, buildName, BuildTarget.WebGL, bo);

			if (string.IsNullOrEmpty(buildResult))
			{
				UnityEngine.Debug.Log("Web build complete" + buildName);
				if (increment)
				{
					IncrementPatch();
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Error building Web:\n" + buildResult);
			}
		}
		#endregion

		#endregion

		#endregion

		#region Show save folder

		[MenuItem("Dasher/Open save folder")]
		public static void OpenSaveFolder()
		{
			string itemPath = Application.persistentDataPath;
			itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
			System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
		}

		#endregion

		#region Colors

		[MenuItem("Dasher/Create color data")]
		static void S_CreateColorData()
		{
			UnityEngine.Debug.Log("Creating a new color data");
			ColorScheme lf = ScriptableObject.CreateInstance<ColorScheme>();
			AssetDatabase.CreateAsset(lf, DATA_PATH + "ColorScheme.Asset");
			UnityEngine.Debug.Log("Color Data created");
		}
		#endregion
	}
}