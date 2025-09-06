using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class ShowBossDoorChallengeUI : FsmStateAction
{
	private static BossDoorChallengeUI spawnedUI;

	public FsmOwnerDefault targetDoor;

	public FsmGameObject prefab;

	public FsmEvent cancelEvent;

	public FsmEvent challengeEvent;

	public override void Reset()
	{
		targetDoor = null;
		prefab = null;
		challengeEvent = null;
		cancelEvent = null;
	}

	public override void OnEnter()
	{
		if (spawnedUI == null && (bool)prefab.Value)
		{
			GameObject gameObject = Object.Instantiate(prefab.Value);
			spawnedUI = gameObject.GetComponent<BossDoorChallengeUI>();
			gameObject.SetActive(value: false);
		}
		if ((bool)spawnedUI)
		{
			GameObject safe = targetDoor.GetSafe(this);
			BossSequenceDoor door = (safe ? safe.GetComponent<BossSequenceDoor>() : null);
			spawnedUI.Setup(door);
			spawnedUI.Show();
			BossDoorChallengeUI.HideEvent temp = null;
			temp = delegate
			{
				base.Fsm.Event(cancelEvent);
				spawnedUI.OnHidden -= temp;
			};
			spawnedUI.OnHidden += temp;
			BossDoorChallengeUI.BeginEvent temp2 = null;
			temp2 = delegate
			{
				base.Fsm.Event(challengeEvent);
				spawnedUI.OnBegin -= temp2;
			};
			spawnedUI.OnBegin += temp2;
		}
	}
}
