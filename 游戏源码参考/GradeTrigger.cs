using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class GradeTrigger : MonoBehaviour
{
	public GradeMarker gradeMarker;

	public bool instantActivate;

	[Range(0.5f, 2f)]
	public float easeTime = 0.8f;

	private void Start()
	{
		if ((bool)gradeMarker)
		{
			gradeMarker.SetStartSizeForTrigger();
			gradeMarker.EaseDuration = easeTime;
			gradeMarker.Deactivate();
		}
		else
		{
			Debug.LogError("No grade marker set for this grade trigger: " + base.name);
		}
	}

	private void OnTriggerEnter2D(Collider2D triggerObject)
	{
		if (triggerObject.tag == "Player")
		{
			if (instantActivate)
			{
				gradeMarker.Activate();
			}
			else
			{
				gradeMarker.ActivateGradual();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D triggerObject)
	{
		if (triggerObject.tag == "Player")
		{
			if (instantActivate)
			{
				gradeMarker.Deactivate();
			}
			else
			{
				gradeMarker.DeactivateGradual();
			}
		}
	}
}
