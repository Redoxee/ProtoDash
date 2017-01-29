using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Dasher;

namespace DasherTool
{
	[CustomEditor(typeof(LevelFlow))]
	public class LevelDataEditor : Editor
	{
		private ReorderableList list;

		private void OnEnable()
		{
			list = new ReorderableList(serializedObject,
					serializedObject.FindProperty("m_levelList"),
					true, true, true, true);

			float elementOffset = EditorGUIUtility.singleLineHeight + 1f;

			list.elementHeightCallback = (index) =>
			{
				float nbFields = 3f;
				if (index % LevelFlow.c_nbLevelInWorld == 0)
				{
					nbFields += 1f;
				}

				return elementOffset * nbFields + 6f;
			};

			list.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2f;
				int nbLine = 0;
				if (index % LevelFlow.c_nbLevelInWorld == 0)
				{
					GUIStyle worldStyle = new GUIStyle();
					worldStyle.fontStyle = FontStyle.Bold;
					EditorGUI.LabelField(rect, string.Format("world : {0}", index / LevelFlow.c_nbLevelInWorld + 1), worldStyle);
					nbLine += 1;
				}

				float fieldWidth = EditorGUIUtility.currentViewWidth - 60;
				EditorGUI.LabelField(
					new Rect(rect.x, rect.y + elementOffset * nbLine, fieldWidth, EditorGUIUtility.singleLineHeight),
					"Level : " + index.ToString("00")
				);
				nbLine += 1;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y + elementOffset * nbLine, fieldWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("sceneObject"), GUIContent.none);
				nbLine += 1;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y + elementOffset * nbLine, fieldWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("parTime"), GUIContent.none);
				nbLine += 1;
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			list.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}