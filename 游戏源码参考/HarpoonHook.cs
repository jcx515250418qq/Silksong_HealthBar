using UnityEngine;
using UnityEngine.Events;

public class HarpoonHook : MonoBehaviour
{
	public UnityEvent OnHookStart;

	public UnityEvent OnHookEnd;

	public UnityEvent OnHookCancel;

	public void HookStart()
	{
		OnHookStart.Invoke();
	}

	public void HookEnd()
	{
		OnHookEnd.Invoke();
	}

	public void HookCancel()
	{
		OnHookCancel.Invoke();
	}
}
