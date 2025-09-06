using UnityEngine;

public class TitleLogo : MonoBehaviour
{
	public StartManager startManager;

	public void AnimationFinished()
	{
		startManager.SwitchToMenuScene();
	}
}
