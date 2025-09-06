using HutongGames.PlayMaker;
using UnityEngine;

public class SetRigidbodySimulated2D : FsmStateAction
{
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	public FsmBool isSimulated;

	public override void Reset()
	{
		gameObject = null;
		isSimulated = true;
	}

	public override void OnEnter()
	{
		DoSetIsKinematic();
		Finish();
	}

	private void DoSetIsKinematic()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if ((bool)ownerDefaultTarget)
		{
			Rigidbody2D component = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				component.simulated = isSimulated.Value;
			}
		}
	}
}
