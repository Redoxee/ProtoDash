using UnityEngine;
using Assets;

public class CameraScript : MonoBehaviour {
	[SerializeField]
	private GameObject target;
	[SerializeField]
	private float xDampingFactor = 0.5f;
	[SerializeField]
	private float yDampingFactor = 0.25f;

	[SerializeField]
	private float cameraYOffset = 6;
	[SerializeField]
	private float wantedXOffset = 3;

	private float originalZoom;

	void Start () {
		originalZoom = transform.position.z;
	}

	private Vector3 _Damping(Vector3 p, Vector3 t, float d, float dt)
	{
		return p + (t - p) / Mathf.Max(d,float.Epsilon) * dt;
	}

	void LateUpdate()
	{
		Vector3 tp = target.transform.position + new Vector3(wantedXOffset,cameraYOffset,originalZoom );
		tp.z = originalZoom;
		transform.position = new Vector3(FuctionUtils.damping(xDampingFactor ,transform.position.x,tp.x,Time.deltaTime),FuctionUtils.damping(yDampingFactor, transform.position.y, tp.y, Time.deltaTime),transform.position.z);
	}
}
