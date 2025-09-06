using UnityEngine;

public class ParticleCulling : MonoBehaviour
{
	private const float CAM_PADDING_X = 5f;

	private const float CAM_PADDING_Y = 5f;

	private ParticleSystem[] systems;

	private Camera camera;

	private Transform cameraTrans;

	private bool wasCulled;

	private void Awake()
	{
		systems = GetComponentsInChildren<ParticleSystem>();
	}

	private void Start()
	{
		camera = GameCameras.instance.mainCamera;
		cameraTrans = camera.transform;
	}

	private void LateUpdate()
	{
		float num = camera.aspect / 1.7777778f;
		Vector2 vector = new Vector2(14.6f * num + 5f, 13.3f);
		Vector2 vector2 = (Vector2)cameraTrans.position - (Vector2)base.transform.position;
		bool flag = false;
		if (vector.x > 0f && Mathf.Abs(vector2.x) > vector.x)
		{
			flag = true;
		}
		else if (vector.y > 0f && Mathf.Abs(vector2.y) > vector.y)
		{
			flag = true;
		}
		if (flag != wasCulled)
		{
			wasCulled = flag;
			ParticleSystem[] array = systems;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.EmissionModule emission = array[i].emission;
				emission.enabled = !flag;
			}
		}
	}
}
