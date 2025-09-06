using System;
using UnityEngine;

public class MapPinConditional : MonoBehaviour
{
	[Serializable]
	public struct PersistentBoolMatch
	{
		public string Id;

		public string SceneName;

		public bool ExpectedValue;
	}

	[Serializable]
	public class Condition
	{
		public PlayerDataTest PlayerDataTest;

		public PersistentBoolMatch PersistentBool = new PersistentBoolMatch
		{
			ExpectedValue = true
		};

		public bool IsFulfilled
		{
			get
			{
				if (!PlayerDataTest.IsFulfilled)
				{
					return false;
				}
				if (string.IsNullOrEmpty(PersistentBool.Id) || string.IsNullOrEmpty(PersistentBool.SceneName))
				{
					return true;
				}
				PersistentItemData<bool> value;
				return (SceneData.instance.PersistentBools.TryGetValue(PersistentBool.SceneName, PersistentBool.Id, out value) && value.Value) == PersistentBool.ExpectedValue;
			}
		}
	}

	[SerializeField]
	private Condition visibleCondition;

	[Space]
	[SerializeField]
	private Condition materialCondition;

	[SerializeField]
	private Material material;

	private SpriteRenderer spriteRenderer;

	private Material initialMaterial;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		initialMaterial = spriteRenderer.sharedMaterial;
	}

	private void OnEnable()
	{
		if (!visibleCondition.IsFulfilled)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			spriteRenderer.sharedMaterial = (materialCondition.IsFulfilled ? material : initialMaterial);
		}
	}
}
