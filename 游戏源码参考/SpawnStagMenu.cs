using UnityEngine;

public class SpawnStagMenu : MonoBehaviour
{
	private PlayMakerFSM fsm;

	private void Start()
	{
		if ((bool)HeroController.instance)
		{
			HeroController.HeroInPosition temp = null;
			temp = delegate
			{
				SendEvent();
				HeroController.instance.heroInPosition -= temp;
			};
			HeroController.instance.heroInPosition += temp;
		}
		else
		{
			SendEvent();
		}
	}

	private void SendEvent()
	{
		if ((bool)GameCameras.instance)
		{
			fsm = GameCameras.instance.openStagFSM;
		}
		if ((bool)fsm)
		{
			fsm.SendEvent("SPAWN");
		}
	}

	private void OnDestroy()
	{
		if ((bool)fsm)
		{
			fsm.SendEvent("DESPAWN");
		}
	}
}
