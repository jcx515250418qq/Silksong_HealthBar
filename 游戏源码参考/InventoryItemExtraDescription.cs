using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemExtraDescription : MonoBehaviour
{
	[SerializeField]
	private Transform descSectionParent;

	[SerializeField]
	private GameObject extraDescPrefab;

	[SerializeField]
	private PlayerDataTest condition;

	private InventoryItemSelectable selectable;

	private bool isSelected;

	private static readonly Dictionary<GameObject, GameObject> _spawnedExtraDescriptions = new Dictionary<GameObject, GameObject>();

	public bool WillDisplay => condition.IsFulfilled;

	public GameObject ExtraDescPrefab
	{
		get
		{
			return extraDescPrefab;
		}
		set
		{
			if (!(extraDescPrefab == value))
			{
				if (isSelected && (bool)extraDescPrefab && _spawnedExtraDescriptions.TryGetValue(extraDescPrefab, out var value2))
				{
					value2.SetActive(value: false);
				}
				extraDescPrefab = value;
				OnUpdateDisplay(selectable);
			}
		}
	}

	public event Action<GameObject> ActivatedDesc;

	private void Awake()
	{
		selectable = GetComponent<InventoryItemSelectable>();
		selectable.OnSelected += OnSelected;
		selectable.OnDeselected += OnDeselected;
		selectable.OnUpdateDisplay += OnUpdateDisplay;
	}

	private void OnSelected(InventoryItemSelectable self)
	{
		if (!condition.IsFulfilled)
		{
			return;
		}
		isSelected = true;
		if ((bool)extraDescPrefab && _spawnedExtraDescriptions.ContainsKey(extraDescPrefab))
		{
			GameObject gameObject = _spawnedExtraDescriptions[extraDescPrefab];
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: true);
				this.ActivatedDesc?.Invoke(gameObject);
			}
		}
	}

	private void OnDeselected(InventoryItemSelectable self)
	{
		isSelected = false;
		if ((bool)extraDescPrefab && _spawnedExtraDescriptions.TryGetValue(extraDescPrefab, out var value))
		{
			value.SetActive(value: false);
		}
	}

	private void OnUpdateDisplay(InventoryItemSelectable self)
	{
		if (!extraDescPrefab || !descSectionParent)
		{
			return;
		}
		if (_spawnedExtraDescriptions.TryGetValue(extraDescPrefab, out var value) && (bool)value)
		{
			if (isSelected)
			{
				this.ActivatedDesc?.Invoke(value);
			}
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(extraDescPrefab, descSectionParent, worldPositionStays: false);
		_spawnedExtraDescriptions[extraDescPrefab] = gameObject;
		gameObject.SetActive(isSelected);
		if (isSelected)
		{
			this.ActivatedDesc?.Invoke(gameObject);
		}
	}
}
