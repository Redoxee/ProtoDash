using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dasher
{
	public enum LocaLanguage
	{
		English,
		Chinese,
	}

	[Serializable]
	public class LocaDictionary : SerializableDictionary<int, string> { }
	[Serializable]
	public class LocaCollection : SerializableDictionary<int, LocaDictionary> { }
	public class LocaObject : ScriptableObject
	{
		[SerializeField]
		public LocaCollection m_loca;

		public LocaLanguage CurrentLoca = LocaLanguage.English;

		LocaDictionary CurrentLang { get { return m_loca.dictionary[(int)CurrentLoca]; } }
		LocaDictionary DefaultLang { get { return m_loca.dictionary[(int)LocaLanguage.English]; } }
		public string GetText(int index)
		{
			if (CurrentLang.dictionary.ContainsKey(index))
			{
				return CurrentLang.dictionary[index];
			}
			return DefaultLang.dictionary[index];
		}

		public string GetText(int index, LocaLanguage lang)
		{
			return m_loca.dictionary[(int)lang].dictionary[index];
		}
	}
}