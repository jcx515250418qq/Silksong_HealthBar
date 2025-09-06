using HutongGames.PlayMaker;
using UnityEngine;

public class SetRigidbody2DConstraints : FsmStateAction
{
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	public FsmBool freezePositionX;

	public FsmBool freezePositionY;

	public FsmBool freezeRotation;

	public override void Reset()
	{
		freezePositionX = new FsmBool
		{
			UseVariable = true
		};
		freezePositionY = new FsmBool
		{
			UseVariable = true
		};
		freezeRotation = new FsmBool
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		DoSetConstraints();
		Finish();
	}

	private void DoSetConstraints()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (!ownerDefaultTarget)
		{
			return;
		}
		Rigidbody2D component = ownerDefaultTarget.GetComponent<Rigidbody2D>();
		if ((bool)component)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (!freezePositionX.IsNone)
			{
				flag = freezePositionX.Value;
			}
			if (!freezePositionY.IsNone)
			{
				flag2 = freezePositionY.Value;
			}
			if (!freezeRotation.IsNone)
			{
				flag3 = freezeRotation.Value;
			}
			if (flag && !flag2 && !flag3)
			{
				component.constraints = RigidbodyConstraints2D.FreezePositionX;
			}
			else if (!flag && flag2 && !flag3)
			{
				component.constraints = RigidbodyConstraints2D.FreezePositionY;
			}
			else if (flag && flag2 && !flag3)
			{
				component.constraints = RigidbodyConstraints2D.FreezePosition;
			}
			else if (!flag && !flag2 && flag3)
			{
				component.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
			else if (flag && flag2 && flag3)
			{
				component.constraints = RigidbodyConstraints2D.FreezeAll;
			}
			else
			{
				component.constraints = RigidbodyConstraints2D.None;
			}
		}
	}
}
