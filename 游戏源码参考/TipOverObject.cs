using UnityEngine;

public class TipOverObject : MonoBehaviour
{
	public float tipChance;

	public EventBase cameraReceiver;

	public bool active = true;

	private bool fallRight;

	private bool didFall;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		Transform transform = base.transform.Find("Sprite");
		if (transform != null && transform.transform.localPosition.x < 0f)
		{
			fallRight = true;
		}
		cameraReceiver.ReceivedEvent += ShakeEvent;
	}

	private void ShakeEvent()
	{
		if (!didFall && active && (float)Random.Range(1, 100) <= tipChance)
		{
			DoTipOver();
		}
	}

	public void DoTipOver()
	{
		if (!didFall)
		{
			if (fallRight)
			{
				animator.Play("Tip Over Right");
			}
			else
			{
				animator.Play("Tip Over Left");
			}
			didFall = true;
		}
	}

	public void SetActive(bool set_active)
	{
		active = set_active;
	}
}
