using UnityEngine;
using Assets;

public class CompanionsScript : MonoBehaviour {
	[SerializeField]
	private MonoBehaviour ScriptHolder;
	[SerializeField]
	private MonoBehaviour CharacterTarget;
	[SerializeField]
	private float dampingFacor;


	private MainScript targetScript;
	private Renderer rendererRef;

	private float depth = -10;
	private Vector3 targetOffset;

	// Use this for initialization
	void Start () {
		rendererRef = GetComponent<Renderer>();

		targetScript = ScriptHolder.GetComponent<MainScript>();
		targetOffset = transform.position - CharacterTarget.transform.position;

		depth = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 offset = targetOffset;
		offset.x *= targetScript.getFacingSign();
		Vector3 nextPos = CharacterTarget.transform.position + offset;
		nextPos.z = depth;
		transform.position = FuctionUtils.damping(dampingFacor, transform.position, nextPos, Time.deltaTime);

		rendererRef.material.SetFloat("_GaugeProgression", targetScript.currentEnergy / targetScript.maxEnergyPoints);
	}
}
