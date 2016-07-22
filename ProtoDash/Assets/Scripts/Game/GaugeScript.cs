using UnityEngine;
using UnityEngine.UI;

public class GaugeScript : MonoBehaviour {

	[SerializeField]
	private Character watchedScript;
	
	private Material gaugeMaterial;

	void Start()
	{
		gaugeMaterial = GetComponent<Image>().material;
	}
	
	void Update ()
	{
		gaugeMaterial.SetFloat("_GaugeProgression", watchedScript.currentEnergy / watchedScript.maxEnergyPoints);
	}
}
