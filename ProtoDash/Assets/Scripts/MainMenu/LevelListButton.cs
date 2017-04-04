using System;
using UnityEngine;
using UnityEngine.UI;
namespace Dasher
{
	public class LevelListButton : MonoBehaviour
	{
		public Text MainLabel = null;
		public Text ChampLabel = null;
		public GameObject DisableImage = null;
		[NonSerialized]
		public LevelData bindedLevel = null;
	}
}
