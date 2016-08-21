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
					serializedObject.FindProperty("levelList"),
					true, true, true, true);

			float elementOffset = EditorGUIUtility.singleLineHeight + 1f;

			list.elementHeightCallback = (index) =>
			{
				return elementOffset * 3f + 6f;
			};

			list.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2f;

				float fieldWidth = EditorGUIUtility.currentViewWidth - 60;

				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("sceneObject"), GUIContent.none);
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y + elementOffset, fieldWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("world"), GUIContent.none);
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y + elementOffset * 2f, fieldWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("parTime"), GUIContent.none);
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