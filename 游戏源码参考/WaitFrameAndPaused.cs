using UnityEngine;

public class WaitFrameAndPaused : CustomYieldInstruction
{
	public override bool keepWaiting => Time.deltaTime <= Mathf.Epsilon;
}
