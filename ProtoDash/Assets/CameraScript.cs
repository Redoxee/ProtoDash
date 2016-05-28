using UnityEngine;
using Assets;

public class CameraScript : MonoBehaviour {
	[SerializeField]
	private GameObject target;
	[SerializeField]
	private float xDampingFactor = 0.5f;
	[SerializeField]
	private float yDampingFactor = 0.25f;
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
		transform.position = new Vector3(FuctionUtils.damping(xDampingFactor ,transform.position.x,tp.x,Time.deltaTime),FuctionUtils.damping(yDampingFactor, transform.position.y, tp.y, Time.deltaTime),transform.position.z);
	}
}
