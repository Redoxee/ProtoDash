using UnityEngine;
using Assets;

public class CameraScript : MonoBehaviour {
	[SerializeField]
	private GameObject target;

    [SerializeField]
    MainScript mainScriptRef;

	[SerializeField]
	private float xDampingFactor = 0.5f;
	[SerializeField]
	private float yDampingFactor = 0.25f;

	[SerializeField]
	private float cameraYOffset = 6;
	[SerializeField]
	private float wantedXOffset = 6;


    [SerializeField]
    private float timeToPosX = 1.5f;
    [SerializeField]
    private AnimationCurve repositionXCurve;

	private float originalZoom;
    private float lastOffsetX = 0.0f;
    private float lastOrientation = 0.0f;
    private float timerPosX = 0.0f;


	void Start () {
		originalZoom = transform.position.z;
	}

    void Update()
    {
        if (lastOrientation != mainScriptRef.getFacingSign())
        {
            lastOrientation = mainScriptRef.getFacingSign();
            timerPosX = timeToPosX;
            lastOffsetX = transform.position.x - target.transform.position.x;
        }
        if (timerPosX > 0)
        {
            timerPosX = Mathf.Max(0, timerPosX - Time.deltaTime);
        }
    }

    private void _DrawCross(float x, float y, Color col)
    {
        Debug.DrawRay(new Vector3(x - .5f, y, 0.0f), new Vector3(1f, 0.0f, 0.0f), col);
        Debug.DrawRay(new Vector3(x, y - .5f, 0.0f), new Vector3(0f, 1f, 0.0f), col);
    }

	void LateUpdate()
	{
        float orientation =  mainScriptRef.getFacingSign();
        float currentOffsetX = wantedXOffset * orientation;
        if (timerPosX > 0)
        {
            float progression = 1.0f - repositionXCurve.Evaluate(1.0f - timerPosX / timeToPosX);

            currentOffsetX = currentOffsetX + (lastOffsetX - currentOffsetX) * progression;
            orientation = lastOrientation;

        }

        Vector3 p = target.transform.position;
        float tpx = p.x + currentOffsetX;
        float tpy = p.y + cameraYOffset;
        _DrawCross(tpx, tpy, Color.yellow);
        tpx = FuctionUtils.damping(xDampingFactor, transform.position.x, tpx, Time.deltaTime);
        tpy = FuctionUtils.damping(yDampingFactor, transform.position.y, tpy, Time.deltaTime);
        _DrawCross(p.x + wantedXOffset * orientation, p.y + cameraYOffset, Color.gray);
        _DrawCross(tpx, tpy, Color.cyan);

        transform.position = new Vector3(tpx, tpy, transform.position.z);
	}
}
