using System.Collections;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class PromptMarker : MonoBehaviour
{
	[SerializeField]
	private TMP_Text label;

	[SerializeField]
	private NestedFadeGroup group;

	private tk2dSpriteAnimator anim;

	private GameObject owner;

	private Camera mainCam;

	private Camera hudCam;

	private Transform followTransform;

	private Vector3 followOffset;

	public bool IsVisible { get; private set; }

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	private void Start()
	{
		GameManager.instance.UnloadingLevel += RecycleOnLevelLoad;
		CameraRenderHooks.CameraPreCull += UpdatePosition;
	}

	private void OnDestroy()
	{
		if ((bool)GameManager.UnsafeInstance)
		{
			GameManager.UnsafeInstance.UnloadingLevel -= RecycleOnLevelLoad;
		}
		CameraRenderHooks.CameraPreCull -= UpdatePosition;
	}

	private void RecycleOnLevelLoad()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.Recycle();
		}
	}

	private void OnEnable()
	{
		anim.Play("Blank");
		group.AlphaSelf = 0f;
		GameCameras instance = GameCameras.instance;
		mainCam = instance.mainCamera;
		hudCam = instance.hudCamera;
		UpdatePosition();
	}

	private void OnDisable()
	{
		followTransform = null;
	}

	private void UpdatePosition(CameraRenderHooks.CameraSource cameraSource)
	{
		if (base.isActiveAndEnabled && cameraSource == CameraRenderHooks.CameraSource.HudCamera)
		{
			UpdatePosition();
		}
	}

	private void UpdatePosition()
	{
		if (IsVisible && (!owner || !owner.activeInHierarchy))
		{
			Hide();
		}
		if ((bool)followTransform)
		{
			Vector3 position = followTransform.position + followOffset;
			Vector3 position2 = mainCam.WorldToViewportPoint(position);
			Vector3 position3 = hudCam.ViewportToWorldPoint(position2);
			base.transform.position = position3;
		}
	}

	public void SetLabel(string labelName)
	{
		if ((bool)label)
		{
			label.text = new LocalisedString("Prompts", labelName.ToUpper());
		}
	}

	public void Show()
	{
		anim.Play("Up");
		base.transform.SetPositionZ(0f);
		group.FadeTo(1f, 0.4f, null, isRealtime: true);
		IsVisible = true;
	}

	public void Hide()
	{
		owner = null;
		IsVisible = false;
		if (base.gameObject.activeInHierarchy)
		{
			anim.Play("Down");
			group.FadeTo(0f, 0.233f, null, isRealtime: true);
			StartCoroutine(RecycleDelayed(0.233f));
		}
		else
		{
			base.gameObject.Recycle();
		}
	}

	private IEnumerator RecycleDelayed(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		base.gameObject.Recycle();
	}

	public void SetOwner(GameObject obj)
	{
		owner = obj;
	}

	public void SetFollowing(Transform t, Vector3 offset)
	{
		followTransform = t;
		followOffset = offset;
	}
}
