using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroCorpseMarker : MonoBehaviour
{
	[SerializeField]
	private GuidComponent guidComponent;

	[SerializeField]
	[FormerlySerializedAs("corpseOffset")]
	private Vector2 cocoonOffset;

	private static readonly List<HeroCorpseMarker> _activeMarkers = new List<HeroCorpseMarker>();

	public Vector2 Position => (Vector2)base.transform.position + cocoonOffset;

	public Guid Guid
	{
		get
		{
			if (!guidComponent)
			{
				return Guid.Empty;
			}
			return guidComponent.GetGuid();
		}
	}

	private void OnDrawGizmos()
	{
		if (!(cocoonOffset.magnitude < 0.01f))
		{
			Gizmos.color = Color.magenta;
			Vector2 vector = base.transform.position;
			Vector2 vector2 = vector + cocoonOffset;
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawWireSphere(vector2, 0.5f);
		}
	}

	private void OnEnable()
	{
		_activeMarkers.AddIfNotPresent(this);
	}

	private void OnDisable()
	{
		_activeMarkers.Remove(this);
	}

	public static HeroCorpseMarker GetClosest(Vector2 toPosition)
	{
		HeroCorpseMarker result = null;
		float num = float.MaxValue;
		foreach (HeroCorpseMarker activeMarker in _activeMarkers)
		{
			float num2 = Vector2.Distance(activeMarker.transform.position, toPosition);
			if (num2 < num)
			{
				num = num2;
				result = activeMarker;
			}
		}
		return result;
	}

	public static HeroCorpseMarker GetByGuid(byte[] guid)
	{
		if (guid == null || guid.Length == 0)
		{
			return null;
		}
		return GetByGuid(new Guid(guid));
	}

	public static HeroCorpseMarker GetByGuid(Guid guid)
	{
		if (guid == Guid.Empty)
		{
			return null;
		}
		foreach (HeroCorpseMarker activeMarker in _activeMarkers)
		{
			if ((bool)activeMarker.guidComponent && activeMarker.guidComponent.GetGuid() == guid)
			{
				return activeMarker;
			}
		}
		return null;
	}

	public static HeroCorpseMarker GetRandom()
	{
		if (_activeMarkers.Count != 0)
		{
			return _activeMarkers[UnityEngine.Random.Range(0, _activeMarkers.Count)];
		}
		return null;
	}
}
