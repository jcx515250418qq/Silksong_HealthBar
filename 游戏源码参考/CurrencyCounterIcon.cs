using UnityEngine;

public class CurrencyCounterIcon : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM fsm;

	[SerializeField]
	private Animator animator;

	private Vector3 scaleFrom;

	private Coroutine scaleRoutine;

	public void Appear()
	{
		if (scaleRoutine != null)
		{
			StopCoroutine(scaleRoutine);
		}
		if ((bool)fsm)
		{
			fsm.SendEvent("APPEAR");
			return;
		}
		if ((bool)animator)
		{
			animator.Play("Appear");
			return;
		}
		base.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		base.transform.ScaleTo(this, Vector3.one, 0.2f);
	}

	public void Disappear()
	{
		if (scaleRoutine != null)
		{
			StopCoroutine(scaleRoutine);
		}
		if ((bool)fsm)
		{
			fsm.SendEvent("DISAPPEAR");
			return;
		}
		if ((bool)animator)
		{
			animator.Play("Disappear");
			return;
		}
		scaleFrom = base.transform.localScale;
		scaleRoutine = this.StartTimerRoutine(0f, 0.2f, delegate(float t)
		{
			base.transform.localScale = Vector3.Lerp(scaleFrom, new Vector3(0.5f, 0.5f, 1f), t);
		}, null, delegate
		{
			base.transform.localScale = Vector3.zero;
		});
	}

	public void HideInstant()
	{
		if (scaleRoutine != null)
		{
			StopCoroutine(scaleRoutine);
		}
		if ((bool)fsm)
		{
			fsm.enabled = false;
			fsm.enabled = true;
		}
		else if ((bool)animator)
		{
			base.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.transform.localScale = Vector3.zero;
		}
	}

	public void Idle()
	{
		if ((bool)fsm)
		{
			fsm.SendEvent("IDLE");
		}
	}

	public void Get()
	{
		if ((bool)fsm)
		{
			fsm.SendEvent("GET");
		}
	}

	public void GetSingle()
	{
		if ((bool)fsm)
		{
			fsm.SendEvent("GET SINGLE");
		}
	}

	public void Take()
	{
		if ((bool)fsm)
		{
			fsm.SendEvent("TAKE");
		}
	}
}
