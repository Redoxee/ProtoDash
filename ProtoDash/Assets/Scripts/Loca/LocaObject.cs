using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dasher
{
	public enum LocaLanguage
	{
		English,
		ChineseSimplified,
		ChineseTraditional,
	}

	[Serializable]
	public class LocaDictionary : SerializableDictionary<int, string> {	}
	[Serializable]
	public class LocaCollection : SerializableDictionary<int, LocaDictionary> { }
	public class LocaObject : ScriptableObject
	{
		[SerializeField]
		public LocaCollection m_loca;

		public LocaLanguage CurrentLoca = LocaLanguage.English;

		public LocaDictionary this[LocaLanguage l]
		{
			get
			{
				return m_loca[(int)l];
			}
			set
			{
				m_loca[(int)l] = value;
			}
		}

		LocaDictionary CurrentLang { get { return m_loca[(int)CurrentLoca]; } }
		LocaDictionary DefaultLang { get { return m_loca[(int)LocaLanguage.English]; } }
		public string GetText(int index)
		{
			if (CurrentLang.dictionary.ContainsKey(index))
			{
				return CurrentLang[index];
			}
			if(DefaultLang.dictionary.ContainsKey(index))
				return DefaultLang[index];
			return null;
		}

		public string GetText(int index, LocaLanguage lang)
		{
			if(m_loca[(int)lang].dictionary.ContainsKey(index))
				return m_loca[(int)lang].dictionary[index];
			return null;
		}
	}
}