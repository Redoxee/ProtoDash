using UnityEngine;
using System.Collections;
using Dasher;

public class AnimationSandbox : MonoBehaviour {

	public TimeDisplayCapsule m_timeCapsule;

	void Start()
	{
		m_timeCapsule.Initialize();
	}

	void Update () {
		m_timeCapsule.ManualUpdate();
	}

	public void StartFlash()
	{
		string randomMessage = "Time\n" + Random.Range(0, 99).ToString(TimeManager.c_timeDisplayFormat);

		m_timeCapsule.FlashInText(randomMessage);
	}

	public void StartSlide()
	{
		m_timeCapsule.StartSlide();
	}

	public void HideAdditional()
	{
		m_timeCapsule.HideBackPanel();
	}

	public void SetFrontCapsuleState(int st)
	{
		m_timeCapsule.SetFrontBorderState((TimeDisplayCapsule.CapsuleSuccessState)st);
	}

	public void SetBackCapsuleState(int st)
	{
		m_timeCapsule.SetBackBorderState((TimeDisplayCapsule.CapsuleSuccessState)st);
	}

}
