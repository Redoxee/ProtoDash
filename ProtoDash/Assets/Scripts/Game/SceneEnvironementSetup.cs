using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher
{
	public class SceneEnvironementSetup : MonoBehaviour
	{
		[SerializeField]
		bool m_dontAddBackGround = false;
		[SerializeField]
		ColorSetter m_customColorSetter = null;

		void Start()
		{
			if (MainProcess.Instance == null)
				return;
			if (MainProcess.Instance.CurrentLevel == null)
				return;

			var mp = MainProcess.Instance;
			var levelFlow = mp.levelFlow;
			var ranks = levelFlow.GetWorldAndRankPosition(mp.CurrentLevel);
			var worldIndex = ranks.Key;
			var dresser = mp.WorldDresser;
			var dress = dresser.GetDressForWorld(worldIndex - 1);
			if (dress != null)
			{
				var colors = dress.ColorSetter;
				if (m_customColorSetter != null)
					colors = m_customColorSetter;
				if (colors != null)
					colors.ApplyColors();

				if (dress.BackgroundPrefab != null && !m_dontAddBackGround)
				{
					var background = Instantiate(dress.BackgroundPrefab);
					background.transform.SetParent(transform);
					background.transform.position = Vector3.zero;
				}
			}
		}
	}
}
