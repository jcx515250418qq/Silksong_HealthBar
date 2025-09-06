using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class PlayHitEffects : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	[UIHint(UIHint.Variable)]
	public FsmFloat attackDirection;

	private List<IHitEffectReciever> hitEffectRecievers;

	public override void Awake()
	{
		base.Awake();
		hitEffectRecievers = new List<IHitEffectReciever>();
	}

	public override void Reset()
	{
		target = new FsmOwnerDefault();
		attackDirection = new FsmFloat();
	}

	public override void OnEnter()
	{
		GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
		if (gameObject != null)
		{
			hitEffectRecievers.Clear();
			gameObject.GetComponents(hitEffectRecievers);
			for (int i = 0; i < hitEffectRecievers.Count; i++)
			{
				hitEffectRecievers[i].ReceiveHitEffect(new HitInstance
				{
					Direction = attackDirection.Value
				});
			}
			hitEffectRecievers.Clear();
		}
		Finish();
	}
}
