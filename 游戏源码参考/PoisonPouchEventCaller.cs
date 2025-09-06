using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

public class PoisonPouchEventCaller : MonoBehaviour
{
	public UnityEvent OnPoisonEquipped;

	public UnityEvent OnPoisonNotEquipped;

	private void OnEnable()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped)
		{
			OnPoisonEquipped.Invoke();
		}
		else
		{
			OnPoisonNotEquipped.Invoke();
		}
	}
}
