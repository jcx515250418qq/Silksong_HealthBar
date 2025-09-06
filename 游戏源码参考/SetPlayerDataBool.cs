using TeamCherry.SharedUtils;
using UnityEngine;

public class SetPlayerDataBool : TriggerEnterEvent
{
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string boolName;

	[SerializeField]
	private bool value;

	[SerializeField]
	private bool setOnLoad;

	[SerializeField]
	private bool setOnTriggerEnter;

	protected override void Awake()
	{
		base.Awake();
		if (setOnTriggerEnter)
		{
			base.OnTriggerEntered += OnTriggerEnteredEvent;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (setOnLoad)
		{
			SetBool();
		}
	}

	private void OnTriggerEnteredEvent(Collider2D collision, GameObject sender)
	{
		if (setOnTriggerEnter)
		{
			SetBool();
		}
	}

	public void SetBool()
	{
		PlayerData.instance.SetVariable(boolName, value);
	}
}
