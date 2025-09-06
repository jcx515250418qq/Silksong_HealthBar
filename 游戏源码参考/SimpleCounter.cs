using GlobalSettings;
using TMProOld;
using UnityEngine;
using UnityEngine.Events;

public class SimpleCounter : MonoBehaviour
{
	[SerializeField]
	private EventRegister appearEvent;

	[SerializeField]
	private EventRegister disappearEvent;

	[SerializeField]
	private EventRegister incrementEvent;

	[SerializeField]
	private TMP_Text counterText;

	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	private GameObject hitTargetEffect;

	[Space]
	public UnityEvent OnIncrement;

	private int count;

	private int cap;

	private int target;

	private bool wasAboveTarget;

	private void Awake()
	{
		if ((bool)appearEvent)
		{
			appearEvent.ReceivedEvent += Appear;
		}
		if ((bool)disappearEvent)
		{
			disappearEvent.ReceivedEvent += Disappear;
		}
		if ((bool)incrementEvent)
		{
			incrementEvent.ReceivedEvent += Increment;
		}
		if ((bool)hitTargetEffect)
		{
			hitTargetEffect.SetActive(value: false);
		}
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Appear()
	{
		count = 0;
		UpdateText();
		base.gameObject.SetActive(value: true);
	}

	public void Disappear()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Increment()
	{
		count++;
		UpdateText();
		OnIncrement.Invoke();
	}

	public void SetCurrent(int value)
	{
		count = value;
		UpdateText();
	}

	public void SetCap(int value)
	{
		cap = value;
		UpdateText();
	}

	public void SetTarget(int value)
	{
		target = value;
		UpdateText();
	}

	private void UpdateText()
	{
		counterText.text = ((cap > 0) ? $"{count}/{cap}" : count.ToString());
		bool flag = target > 0 && count > target;
		Color color = (flag ? UI.MaxItemsTextColor : Color.white);
		counterText.color = color;
		if ((bool)icon)
		{
			icon.color = color;
		}
		if (flag && !wasAboveTarget && (bool)hitTargetEffect)
		{
			hitTargetEffect.SetActive(value: true);
		}
		wasAboveTarget = flag;
	}
}
