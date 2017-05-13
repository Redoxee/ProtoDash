using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dasher
{
	[CustomEditor(typeof(DecoratorHolder))]
	[CanEditMultipleObjects]
	public class DecoratorInspector : Editor
	{
		//int m_northLength, m_eastLength, m_southLength, m_westLength = 1;

		public override void OnInspectorGUI()
		{
			DecoratorHolder subject = target as DecoratorHolder;
			base.OnInspectorGUI();

			if (targets.Length < 2)
			{

				GUILink(subject.m_northLink, "N");
				GUILink(subject.m_eastLink, "E");
				GUILink(subject.m_southLink, "S");
				GUILink(subject.m_westLink, "W");
			}
			else
			{
				GUILinks((DecoratorHolder me) => { return me.m_northLink; }, "N");
				GUILinks((DecoratorHolder me) => { return me.m_eastLink; }, "E");
				GUILinks((DecoratorHolder me) => { return me.m_southLink; }, "S");
				GUILinks((DecoratorHolder me) => { return me.m_westLink; }, "W");
			}
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
				isActive = true;
			}
			if (length < 1)
			{
				length = 0;
				isActive = false;
			}
			else
			{
				isActive = true;
			}

			scale.x = length;
			link.SetActive(isActive);
			link.transform.localScale = scale;
			
			GUILayout.EndHorizontal();
		}
		delegate GameObject GetLink(DecoratorHolder deco);
		void GUILinks(GetLink glink, string name)
		{

			GUILayout.BeginHorizontal();

			GUILayout.Label(name, GUILayout.Width(40));

			Vector3 scale = glink((DecoratorHolder)targets[0]).transform.localScale;

			bool isChanged = false;
			bool isActive = true;
			int length = (int)scale.x;

			if (GUILayout.Button("-", GUILayout.Width(20)))
			{
				length -= 1;
				isChanged = true;
			}

			int nLength = EditorGUILayout.IntField(length, GUILayout.Width(30));
			if (nLength != length)
			{
				length = nLength;
				isChanged = true;
			}

			if (GUILayout.Button("+", GUILayout.Width(20)))
			{
				length += 1;
				isChanged = true;
			}


			if (length < 1)
			{
				length = 0;
				isActive = false;
			}

			if (isChanged)
			{
				scale.x = length;
				foreach (Object odeco in targets)
				{
					var deco = (DecoratorHolder)odeco;
					var link = glink(deco);
					link.SetActive(isActive);
					link.transform.localScale = scale;
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}