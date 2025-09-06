using System.Collections.Generic;
using UnityEngine;

public class SavedItemTrackerMarker : MonoBehaviour
{
	[SerializeField]
	private SavedItem[] items;

	public IReadOnlyCollection<SavedItem> Items => (IReadOnlyCollection<SavedItem>)(object)items;
}
