using System.Collections.Generic;
using UnityEngine;

public class JitterEnemyInside : MonoBehaviour
{
	private struct EnemyInfo
	{
		public Transform Transform;

		public Vector3 InitialPos;

		public bool FreezePosition;

		public bool IsReady;

		public Vector3 PreCullPos;

		public Vector3 TargetPos;
	}

	[SerializeField]
	private float frequency;

	[SerializeField]
	private Vector3 amount;

	[SerializeField]
	private bool ignoreZ;

	private double nextJitterTime;

	private List<EnemyInfo> enemiesInside = new List<EnemyInfo>();

	private void Awake()
	{
		if (frequency <= 0f)
		{
			frequency = 60f;
		}
	}

	private void OnEnable()
	{
		CameraRenderHooks.CameraPreCull += OnCameraPreCull;
		CameraRenderHooks.CameraPostRender += OnCameraPostRender;
	}

	private void OnDisable()
	{
		enemiesInside.Clear();
		CameraRenderHooks.CameraPreCull -= OnCameraPreCull;
		CameraRenderHooks.CameraPostRender -= OnCameraPostRender;
	}

	private void Update()
	{
		if (Time.timeAsDouble < nextJitterTime)
		{
			return;
		}
		nextJitterTime = Time.timeAsDouble + (double)(1f / frequency);
		for (int i = 0; i < enemiesInside.Count; i++)
		{
			EnemyInfo value = enemiesInside[i];
			if (!value.IsReady)
			{
				value.IsReady = true;
			}
			if (value.FreezePosition)
			{
				Vector3 initialPos = value.InitialPos;
				if (ignoreZ)
				{
					initialPos.z = value.Transform.position.z;
				}
				value.Transform.position = initialPos;
			}
			value.TargetPos = value.Transform.position + amount.RandomInRange();
			enemiesInside[i] = value;
		}
	}

	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType != CameraRenderHooks.CameraSource.MainCamera || !base.isActiveAndEnabled || Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		for (int i = 0; i < enemiesInside.Count; i++)
		{
			EnemyInfo value = enemiesInside[i];
			if (value.IsReady)
			{
				value.PreCullPos = value.Transform.position;
				value.Transform.position = value.TargetPos;
				enemiesInside[i] = value;
			}
		}
	}

	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType != CameraRenderHooks.CameraSource.MainCamera || !base.isActiveAndEnabled || Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		for (int i = 0; i < enemiesInside.Count; i++)
		{
			EnemyInfo value = enemiesInside[i];
			if (value.IsReady)
			{
				value.Transform.position = value.PreCullPos;
				enemiesInside[i] = value;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Recoil component = collision.GetComponent<Recoil>();
		if (!component)
		{
			return;
		}
		float recoilSpeedBase = component.RecoilSpeedBase;
		foreach (EnemyInfo item in enemiesInside)
		{
			if (item.Transform == component.transform)
			{
				return;
			}
		}
		enemiesInside.Add(new EnemyInfo
		{
			Transform = component.transform,
			InitialPos = component.transform.position,
			FreezePosition = (recoilSpeedBase > 0f)
		});
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Recoil component = collision.GetComponent<Recoil>();
		if (!component)
		{
			return;
		}
		for (int num = enemiesInside.Count - 1; num >= 0; num--)
		{
			if (enemiesInside[num].Transform == component.transform)
			{
				enemiesInside.RemoveAt(num);
			}
		}
	}
}
