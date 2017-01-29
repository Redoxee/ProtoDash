﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorSetter))]
public class ColorSetterDrawer : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Apply Colors"))
		{
			((ColorSetter)target).ApplyColors();
		}
	}
}