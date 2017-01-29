using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher
{
	[Serializable]
	public class WorldDress
	{
		public ColorSetter ColorSetter;
		public GameObject BackgroundPrefab;
	}

	public class WorldDresser : ScriptableObject
	{

		[SerializeField]
		WorldDress m_defaultDress;
		public WorldDress DefaultDress { get { return m_defaultDress; } }

		[SerializeField]
		List<WorldDress> m_worldsDresses = new List<WorldDress>();
		public List<WorldDress> WorldDresses { get { return m_worldsDresses; } }

		public WorldDress GetDressForWorld(int world)
		{
			if (world >= m_worldsDresses.Count)
				return m_defaultDress;
			return m_worldsDresses[world];
		}
	}
}
