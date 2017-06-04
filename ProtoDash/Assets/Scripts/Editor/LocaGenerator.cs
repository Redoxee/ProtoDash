using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace Dasher
{
	public class LocaGenerator : EditorWindow
	{
		const string c_locaAssetPath = "Assets/Data/Localization.asset";

		[MenuItem("Dasher/Localization")]
		public static void OpenWindow()
		{

			var window = EditorWindow.GetWindow(typeof(LocaGenerator));
			window.name = "Dasher Tool";
		}

		string sourceLocaPath = "";
		const string sourceExt = "tsv";


		public void OnGUI()
		{
			EditorGUILayout.LabelField("Localization generation");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Source Loca Path");
			EditorGUILayout.LabelField(sourceLocaPath);
			if (GUILayout.Button("..."))
			{
				sourceLocaPath = EditorUtility.OpenFilePanel("LocaSources","C:\\", sourceExt);
			}
			
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Create assets Localization"))
			{
				if(sourceLocaPath != null && sourceLocaPath != "")
					ImportLocalization();
			}
		}

		void ImportLocalization()
		{
			string[] lines = System.IO.File.ReadAllLines(sourceLocaPath);

			LocaObject loca = LocaObject.CreateInstance<LocaObject>();
			loca.m_loca = LocaCollection.New<LocaCollection>();
			loca.m_loca.dictionary[(int)LocaLanguage.English] = LocaDictionary.New<LocaDictionary>();
			loca.m_loca.dictionary[(int)LocaLanguage.Chinese] = LocaDictionary.New<LocaDictionary>();

			int index = 1;
			for (; index < lines.Length; ++index)
			{
				string line = lines[index];
				string[] parse = line.Split('\t');


				loca.m_loca.dictionary[(int)LocaLanguage.English].dictionary[index] = parse[1];
			}
			LocaObject old = AssetDatabase.LoadAssetAtPath<LocaObject>(c_locaAssetPath);
			if (old != null)
			{
				EditorUtility.CopySerialized(loca, old);
				EditorUtility.SetDirty(old);
				AssetDatabase.SaveAssets();
			}
			else
			{
				AssetDatabase.CreateAsset(loca, c_locaAssetPath);
			}
		}
	}
}
