using System.Collections;
using UnityEngine;

public class BlackThreadImpacter : MonoBehaviour
{
	private bool activated;

	private GameObject silk_strand_impact;

	private GameObject black_thread_strand;

	private GameObject appearRange;

	private PersistentBoolItem persistent;

	private void Awake()
	{
		silk_strand_impact = base.transform.Find("silk_strand_impact").gameObject;
		black_thread_strand = base.transform.Find("black_thread_strand").gameObject;
		appearRange = base.transform.Find("Appear Range").gameObject;
		persistent = GetComponent<PersistentBoolItem>();
		if (persistent != null)
		{
			persistent.OnGetSaveState += delegate(out bool val)
			{
				val = activated;
			};
			persistent.OnSetSaveState += delegate(bool val)
			{
				activated = val;
				if (activated)
				{
					SetAlreadyActivated();
				}
			};
		}
		if (activated)
		{
			silk_strand_impact.SetActive(value: false);
			black_thread_strand.SetActive(value: false);
		}
		else
		{
			silk_strand_impact.SetActive(value: false);
			black_thread_strand.SetActive(value: true);
			appearRange.SetActive(value: false);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!activated)
		{
			appearRange.SetActive(value: false);
			StartCoroutine(DoAppear());
			activated = true;
		}
	}

	public IEnumerator DoAppear()
	{
		yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));
		silk_strand_impact.SetActive(value: true);
		yield return new WaitForSeconds(0.59f);
		silk_strand_impact.SetActive(value: false);
		black_thread_strand.SetActive(value: true);
	}

	private void SetAlreadyActivated()
	{
	}
}
