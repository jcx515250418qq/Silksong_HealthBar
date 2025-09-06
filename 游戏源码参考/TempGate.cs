using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class TempGate : MonoBehaviour
{
	[SerializeField]
	private string openAnim;

	[SerializeField]
	private string openedAnim;

	[SerializeField]
	private string closeAnim;

	[SerializeField]
	private string closedAnim;

	[SerializeField]
	private float openDelay;

	[SerializeField]
	private float closeDelay;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private TempPressurePlate plate;

	[SerializeField]
	private TrackTriggerObjects keepOpenRange;

	[SerializeField]
	private float keepOpenMinTime;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string brokenPDBool;

	[SerializeField]
	private PersistentBoolItem readPersistent;

	private Animator animator;

	private AudioSource source;

	private bool queueOpen;

	private Coroutine updateRoutine;

	private void OnDrawGizmos()
	{
		if ((bool)plate)
		{
			Gizmos.DrawLine(base.transform.position, plate.transform.position);
		}
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		if ((bool)plate)
		{
			plate.Activated += OnPlateActivated;
		}
		HeroController hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			SetStartingState();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate
		{
			SetStartingState();
			hc.heroInPosition -= temp;
		};
		hc.heroInPosition += temp;
	}

	private void SetStartingState()
	{
		if (!animator)
		{
			Debug.LogError("No animator found!", this);
			return;
		}
		bool flag = !string.IsNullOrEmpty(brokenPDBool) && PlayerData.instance.GetVariable<bool>(brokenPDBool);
		bool flag2 = flag || keepOpenRange.IsInside;
		string text = ((!string.IsNullOrEmpty(openedAnim)) ? openedAnim : openAnim);
		string text2 = ((!string.IsNullOrEmpty(closedAnim)) ? closedAnim : closeAnim);
		animator.Play(flag2 ? text : text2, 0, 1f);
		animator.Update(0f);
		IBeginStopper[] componentsInChildren = GetComponentsInChildren<IBeginStopper>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DoBeginStop();
		}
		if ((bool)readPersistent && !readPersistent.GetCurrentValue())
		{
			plate.ActivateSilent();
		}
		else if (flag2)
		{
			plate.ActivateSilent();
		}
		if (!flag)
		{
			updateRoutine = StartCoroutine(UpdateState(flag2));
		}
	}

	private void OnPlateActivated()
	{
		queueOpen = true;
	}

	private IEnumerator UpdateState(bool isOpen)
	{
		while (true)
		{
			if (isOpen)
			{
				float closeDelayLeft = closeDelay + keepOpenMinTime;
				while (closeDelayLeft > 0f)
				{
					closeDelayLeft = ((!keepOpenRange.IsInside || (!(keepOpenMinTime <= 0f) && !(closeDelayLeft < keepOpenMinTime))) ? (closeDelayLeft - Time.deltaTime) : closeDelay);
					yield return null;
				}
				animator.Play(closeAnim, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
				isOpen = false;
				plate.Deactivate();
				continue;
			}
			while (!queueOpen)
			{
				yield return null;
			}
			queueOpen = false;
			if (openDelay > 0f)
			{
				yield return new WaitForSeconds(openDelay);
			}
			animator.Play(openAnim, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			isOpen = true;
		}
	}

	public void SetBroken()
	{
		if (updateRoutine != null)
		{
			StopCoroutine(updateRoutine);
		}
		if (!string.IsNullOrEmpty(brokenPDBool))
		{
			PlayerData.instance.SetVariable(brokenPDBool, value: true);
		}
		animator.Play(openAnim, 0, 1f);
	}
}
