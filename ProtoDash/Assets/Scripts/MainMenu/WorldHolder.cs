using UnityEngine;
using System.Collections;

public class WorldHolder : MonoBehaviour {
	private const int c_numberCollumn = 3;
	[SerializeField]
	private GameObject m_rowPrefab = null;
	[SerializeField]
	private GameObject m_levelButtonPrefab = null;
	[SerializeField]
	private Transform m_rowParent = null;

	private GameObject m_currentRow;

	public GameObject AddLevelButton()
	{
		if (m_currentRow == null ||
			m_currentRow.transform.childCount >= c_numberCollumn)
		{
			m_currentRow = Instantiate<GameObject>(m_rowPrefab);
			m_currentRow.transform.SetParent(m_rowParent,false);
		}
		GameObject newLevelButton = Instantiate<GameObject>(m_levelButtonPrefab);
		newLevelButton.transform.SetParent(m_currentRow.transform,false);
		return newLevelButton;
	}

}
