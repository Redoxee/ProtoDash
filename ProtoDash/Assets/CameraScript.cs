using UnityEngine;
using Assets;

public class CameraScript : MonoBehaviour {
	[SerializeField]
	private GameObject target;
	[SerializeField]
	private float dampingFactor = 0.5f;

	private Vector3 offset = new Vector3(0,0,0);

	private float originalZoom;

	void Start () {
		originalZoom = transform.position.z;
		offset = transform.position - target.transform.position;
	}

	private Vector3 _Damping(Vector3 p, Vector3 t, float d, float dt)
	{
		return p + (t - p) / d * dt;
	}

	void LateUpdate()
	{
		Vector3 tp = target.transform.position + offset;
		tp.z = originalZoom;
		transform.position = FuctionUtils.damping(dampingFactor, transform.position, tp, Time.deltaTime);
	}
}
