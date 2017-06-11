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
			loca[LocaLanguage.English] = LocaDictionary.New<LocaDictionary>();
			loca[LocaLanguage.ChineseSimplified] = LocaDictionary.New<LocaDictionary>();
			loca[LocaLanguage.ChineseTraditional] = LocaDictionary.New<LocaDictionary>();
			
			int index = 1;
			for (; index < lines.Length; ++index)
			{
				string line = lines[index];
				line = line.Replace("\\n", "\n");
				string[] parse = line.Split('\t');
				int locaKey = index + 1; //Linecounting in excel / google sheet start at 1
				loca[LocaLanguage.English][locaKey] = parse[1];
				loca[LocaLanguage.ChineseSimplified][locaKey] = parse[3];
				loca[LocaLanguage.ChineseTraditional][locaKey] = parse[4];
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

			Debug.LogFormat("Locca created successfully : {0}", lines.Length);
		}
	}
}
