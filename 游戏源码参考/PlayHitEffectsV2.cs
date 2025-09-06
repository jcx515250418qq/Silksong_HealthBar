using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class PlayHitEffectsV2 : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	[UIHint(UIHint.Variable)]
	public FsmFloat attackDirection;

	public FsmBool forceNotWeakHit;

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
		forceNotWeakHit = null;
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
					Direction = attackDirection.Value,
					ForceNotWeakHit = forceNotWeakHit.Value
				});
			}
			hitEffectRecievers.Clear();
		}
		Finish();
	}
}
