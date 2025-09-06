using UnityEngine;

public class BetaGateChanger : MonoBehaviour
{
	public TransitionPoint[] gates;

	public void SwitchToBetaExit()
	{
		TransitionPoint[] array = gates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetTargetScene("BetaEnd");
		}
	}
}
