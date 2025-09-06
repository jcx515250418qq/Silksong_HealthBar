using UnityEngine;
using UnityEngine.Events;

public class SuspendedPlatform : SuspendedPlatformBase
{
	[SerializeField]
	protected Collider2D collider;

	[Space]
	[SerializeField]
	private UnityEvent onBreak;

	[SerializeField]
	private UnityEvent onStartedTouching;

	[SerializeField]
	private UnityEvent onStoppedTouching;

	private int touchingCount;

	public override void CutDown()
	{
		base.CutDown();
		SetBroken();
	}

	protected override void OnStartActivated()
	{
		base.OnStartActivated();
		SetBroken();
		base.gameObject.SetActive(value: false);
	}

	private void SetBroken()
	{
		if ((bool)collider)
		{
			collider.enabled = false;
		}
		onBreak.Invoke();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == 9)
		{
			touchingCount++;
			if (touchingCount == 1)
			{
				onStartedTouching.Invoke();
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.layer == 9)
		{
			touchingCount--;
			if (touchingCount == 0)
			{
				onStoppedTouching.Invoke();
			}
		}
	}
}
