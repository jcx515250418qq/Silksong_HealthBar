using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class EnemyWallRange : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM targetFSM;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateTargetBoolExists")]
	private string targetBoolName;

	private FsmBool targetBool;

	private List<Collider2D> collidersInside = new List<Collider2D>();

	public PlayMakerFSM TargetFSM
	{
		get
		{
			return targetFSM;
		}
		set
		{
			targetFSM = value;
			UpdateFSMVariables();
		}
	}

	public string TargetBoolName
	{
		get
		{
			return targetBoolName;
		}
		set
		{
			targetBoolName = value;
			UpdateFSMVariables();
		}
	}

	public bool ValidateTargetBoolExists()
	{
		UpdateFSMVariables();
		return targetBool != null;
	}

	private void Start()
	{
		UpdateFSMVariables();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		collidersInside.AddIfNotPresent(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		collidersInside.Remove(collision);
	}

	private void FixedUpdate()
	{
		if (targetBool != null)
		{
			bool flag = collidersInside.Count > 0;
			if (targetBool.Value != flag)
			{
				targetBool.Value = flag;
			}
		}
	}

	private void UpdateFSMVariables()
	{
		targetBool = null;
		if (!(TargetFSM == null) && !string.IsNullOrEmpty(TargetBoolName))
		{
			targetBool = TargetFSM.FsmVariables.FindFsmBool(TargetBoolName);
		}
	}
}
