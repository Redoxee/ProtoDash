using UnityEngine;
using System;
using System.Collections.Generic;
namespace Dasher
{
	[Serializable]
	public struct LevelData
	{
		public UnityEngine.Object sceneObject;
		public string sceneName;
		public int world;
		public float parTime;
	}

	public class LevelFlow : ScriptableObject
	{
		[SerializeField]
		public List<LevelData> levelList;
	}
}