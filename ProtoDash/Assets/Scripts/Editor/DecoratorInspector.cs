using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dasher
{
	[CustomEditor(typeof(DecoratorHolder))]
	public class DecoratorInspector : Editor
	{
		int m_northLength, m_eastLength, m_southLength, m_westLength = 1;

		public override void OnInspectorGUI()
		{
			DecoratorHolder subject = target as DecoratorHolder;
			base.OnInspectorGUI();

			GUILink(subject.m_northLink, "N");
			GUILink(subject.m_eastLink, "E");
			GUILink(subject.m_southLink, "S");
			GUILink(subject.m_westLink, "W");
		}

		void GUILink(GameObject link, string name)
		{

			bool isActive;
			int length;
			Vector3 scale;

			GUILayout.BeginHorizontal();
			isActive = link.activeSelf;

			isActive = GUILayout.Toggle(isActive, name, GUILayout.Width(40));

			scale = link.transform.localScale;
			length = (int)scale.x;
			if (GUILayout.Button("-", GUILayout.Width(20)))
			{
				length -= 1;
			}
			length = EditorGUILayout.IntField(length, GUILayout.Width(30));
			if (GUILayout.Button("+", GUILayout.Width(20)))
			{
				length += 1;
			}
			if (length < 1)
			{
				length = 1;
				isActive = false;
			}

			scale.x = length;
			link.SetActive(isActive);
			link.transform.localScale = scale;
			
			GUILayout.EndHorizontal();
		}
	}
}