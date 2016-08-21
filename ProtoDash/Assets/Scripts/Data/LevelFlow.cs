using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct LevelData
{
	public UnityEngine.Object sceneObject;
	public int world;
	public float parTime;
}

public class LevelFlow : ScriptableObject {
	[SerializeField]
	public List<LevelData> levelList;
}
