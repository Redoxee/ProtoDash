using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ColorSetter : MonoBehaviour {
	[Header("Elements")]
	[SerializeField]
	Material m_skyMaterial = null;
	const string c_skyCenter = "_Color";
	const string c_skyOuter = "_Color2";
	[SerializeField]
	Material m_backElementsMaterial = null;
	const string c_bgElementColor = "_Color";

	[Header("Colors")]
	[SerializeField]
	Color m_bgCenter;
	[SerializeField]
	Color m_bgOuter;
	[SerializeField]
	Color m_elementsColors;
	[SerializeField]
	Color m_fogColor;

	public void ApplyColors()
	{
		m_skyMaterial.SetColor(c_skyCenter, m_bgCenter);
		m_skyMaterial.SetColor(c_skyOuter, m_bgOuter);

		m_backElementsMaterial.SetColor(c_bgElementColor, m_elementsColors);

		RenderSettings.fog = true;
		RenderSettings.fogColor = m_fogColor;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogEndDistance = 1100;
	}
}
