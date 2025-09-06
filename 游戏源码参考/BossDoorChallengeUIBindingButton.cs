using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BossDoorChallengeUIBindingButton : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ICancelHandler, IPointerClickHandler
{
	public delegate void OnSelectionEvent();

	public delegate void OnCancelEvent();

	public Image iconImage;

	public Animator iconAnimator;

	private Sprite defaultSprite;

	public Sprite selectedSprite;

	public Sprite allSelectedSprite;

	public Animator chainAnimator;

	public GameObject bindAllEffect;

	public AudioSource audioSourcePrefab;

	public AudioEvent selectedSound;

	public AudioEvent deselectedSound;

	public float pitchShiftMin;

	public float pitchShiftMax = 0.5f;

	public int maxAmount = 4;

	private static int currentAmount;

	private bool isCounted;

	private bool selected;

	public bool Selected => selected;

	public event OnSelectionEvent OnButtonSelected;

	public event OnCancelEvent OnButtonCancelled;

	private void Awake()
	{
		if ((bool)iconImage)
		{
			defaultSprite = iconImage.sprite;
		}
	}

	public void Reset()
	{
		selected = false;
		if ((bool)chainAnimator)
		{
			chainAnimator.Play("Unbind", 0, 1f);
		}
		if ((bool)iconImage)
		{
			iconImage.sprite = defaultSprite;
			iconImage.SetNativeSize();
		}
		StartCoroutine(SetAnimSizeDelayed("Unbind", 1f));
		if ((bool)bindAllEffect)
		{
			bindAllEffect.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		if (isCounted)
		{
			currentAmount--;
			isCounted = false;
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		selected = !selected;
		if ((bool)iconImage)
		{
			iconImage.sprite = (selected ? selectedSprite : defaultSprite);
			iconImage.SetNativeSize();
		}
		if ((bool)iconAnimator)
		{
			iconAnimator.Play("Select");
		}
		if ((bool)chainAnimator)
		{
			chainAnimator.Play(selected ? "Bind" : "Unbind");
		}
		if (selected && !isCounted)
		{
			isCounted = true;
			currentAmount++;
		}
		else if (!selected && isCounted)
		{
			currentAmount--;
			isCounted = false;
		}
		AudioEvent audioEvent = (selected ? selectedSound : deselectedSound);
		float num = Mathf.Lerp(pitchShiftMin, pitchShiftMax, (float)currentAmount / (float)maxAmount);
		audioEvent.PitchMin += num;
		audioEvent.PitchMax += num;
		audioEvent.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		if (this.OnButtonSelected != null)
		{
			this.OnButtonSelected();
		}
	}

	public void SetAllSelected(bool value)
	{
		if ((bool)iconImage)
		{
			if (value)
			{
				iconImage.sprite = (allSelectedSprite ? allSelectedSprite : selectedSprite);
			}
			else
			{
				iconImage.sprite = (selected ? selectedSprite : defaultSprite);
			}
			iconImage.SetNativeSize();
		}
		if ((bool)iconAnimator)
		{
			iconAnimator.Play("Select");
		}
		StartCoroutine(SetAnimSizeDelayed(value ? "Bind All" : (selected ? "Bind" : "Unbind"), (!value && selected) ? 1 : 0));
		if ((bool)bindAllEffect)
		{
			bindAllEffect.SetActive(value);
		}
	}

	private IEnumerator SetAnimSizeDelayed(string anim, float normalizedTime)
	{
		if ((bool)chainAnimator)
		{
			chainAnimator.Play(anim, 0, normalizedTime);
		}
		float scale = chainAnimator.transform.localScale.x;
		chainAnimator.transform.SetScaleX(0f);
		yield return null;
		Image component = chainAnimator.GetComponent<Image>();
		if ((bool)component)
		{
			component.SetNativeSize();
		}
		chainAnimator.transform.SetScaleX(scale);
	}

	public void OnCancel(BaseEventData eventData)
	{
		if (this.OnButtonCancelled != null)
		{
			this.OnButtonCancelled();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnSubmit(eventData);
	}
}
