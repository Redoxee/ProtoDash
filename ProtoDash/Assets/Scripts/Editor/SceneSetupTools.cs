using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DasherTool
{

	public class SceneSetupTools : EditorWindow
	{
		Color m_fogColor = new Color(4f/255f,0f,29f/255f);
		float m_fogNear = 0;
		float m_fogFar = 1100;

		[MenuItem("Dasher/Scene setup Tool")]
		public static void ShowWindow()
		{
			var window = EditorWindow.GetWindow(typeof(SceneSetupTools));
			window.name = "Scene setup Tool";
		}


		void OnGUI()
		{
			m_fogColor = EditorGUILayout.ColorField("Fog Color", m_fogColor);
			m_fogNear = EditorGUILayout.FloatField("Fog Near", m_fogNear);
			m_fogFar = EditorGUILayout.FloatField("Fog far", m_fogFar);

			if (GUILayout.Button("Apply Fog"))
			{
				ApplyFog();
			}
		}

		void ApplyFog()
		{
			RenderSettings.fog = true;
			RenderSettings.fogColor = m_fogColor;
			RenderSettings.fogMode = FogMode.Linear;
			RenderSettings.fogStartDistance = m_fogNear;
			RenderSettings.fogEndDistance = m_fogFar;
		}

		[MenuItem("Dasher/Time")]
		static void TimeShortCut()
		{
			EditorApplication.ExecuteMenuItem("Edit/Project Settings/Time");
		}
	}
}