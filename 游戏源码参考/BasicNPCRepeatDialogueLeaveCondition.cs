using UnityEngine;

public sealed class BasicNPCRepeatDialogueLeaveCondition : MonoBehaviour
{
	[SerializeField]
	private BasicNPC basicNpc;

	[SerializeField]
	private NPCEncounterStateController npcEcounter;

	private void Awake()
	{
		if (basicNpc == null)
		{
			basicNpc = GetComponent<BasicNPC>();
			if (basicNpc == null)
			{
				return;
			}
		}
		if (npcEcounter == null)
		{
			npcEcounter = GetComponent<NPCEncounterStateController>();
			if (npcEcounter == null)
			{
				return;
			}
		}
		basicNpc.OnEnd.AddListener(OnDialogueEnd);
	}

	private void OnValidate()
	{
		if (basicNpc == null)
		{
			basicNpc = GetComponent<BasicNPC>();
		}
		if (npcEcounter == null)
		{
			npcEcounter = GetComponent<NPCEncounterStateController>();
		}
	}

	private void OnDialogueEnd()
	{
		if (npcEcounter != null)
		{
			npcEcounter.GetCurrentState();
			npcEcounter.SetMet();
			if (basicNpc.HasRepeated)
			{
				npcEcounter.SetReadyToLeave();
			}
		}
	}
}
