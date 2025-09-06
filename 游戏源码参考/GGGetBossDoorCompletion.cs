using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class GGGetBossDoorCompletion : FsmStateAction
{
	public FsmString playerDataVariable;

	[Space]
	[UIHint(UIHint.Variable)]
	public FsmBool unlocked;

	[UIHint(UIHint.Variable)]
	public FsmBool completed;

	[UIHint(UIHint.Variable)]
	public FsmBool allBindings;

	[UIHint(UIHint.Variable)]
	public FsmBool noHits;

	[Space]
	[UIHint(UIHint.Variable)]
	public FsmBool boundNail;

	[UIHint(UIHint.Variable)]
	public FsmBool boundShell;

	[UIHint(UIHint.Variable)]
	public FsmBool boundCharms;

	[UIHint(UIHint.Variable)]
	public FsmBool boundSoul;

	public override void Reset()
	{
		unlocked = null;
		completed = null;
		allBindings = null;
		noHits = null;
		boundNail = null;
		boundShell = null;
		boundCharms = null;
		boundSoul = null;
	}

	public override void OnEnter()
	{
		if (!string.IsNullOrEmpty(playerDataVariable.Value))
		{
			BossSequenceDoor.Completion completion = GameManager.instance.GetPlayerDataVariable<BossSequenceDoor.Completion>(playerDataVariable.Value);
			unlocked.Value = completion.unlocked;
			completed.Value = completion.completed;
			allBindings.Value = completion.allBindings;
			noHits.Value = completion.noHits;
			boundNail.Value = completion.boundNail;
			boundShell.Value = completion.boundShell;
			boundCharms.Value = completion.boundCharms;
			boundSoul.Value = completion.boundSoul;
		}
		Finish();
	}
}
