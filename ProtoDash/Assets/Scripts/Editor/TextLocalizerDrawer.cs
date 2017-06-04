using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Dasher;

[CustomEditor(typeof(SimpleTextLocalizer))]
public class TextLocalizerDrawer : Editor
{
	[SerializeField]
	LocaObject LocaObj = null;


	int GetClosestLoca()
	{
		string current = ((SimpleTextLocalizer)target).GetComponent<Text>().text;
		string loca = null;
		int res = 1;
		do
		{
			loca = LocaObj.GetText(res);
			if (loca != null && loca == current)
			{
				break;
			}
			res++;
		} while (loca != null);

		return res;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		int key = ((SimpleTextLocalizer)target).LocaKey;

		if (key == -1)
		{
			key = GetClosestLoca();
			((SimpleTextLocalizer)target).LocaKey = key;
		}

		string loca = LocaObj.GetText(key);
		if (loca == null)
		{
			loca = "No Loca";
		}
		GUILayout.Label("Loca : " + loca);
	}
}