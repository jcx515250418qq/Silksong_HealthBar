using UnityEngine.Networking;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("WWW")]
	[Tooltip("Gets data from a url and store it in variables. See Unity WWW docs for more details.")]
	public class WWWObject : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Url to download data from.")]
		public FsmString url;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets text from the url.")]
		public FsmString storeText;

		[UIHint(UIHint.Variable)]
		[Tooltip("Gets a Texture from the url.")]
		public FsmTexture storeTexture;

		[UIHint(UIHint.Variable)]
		[Tooltip("Error message if there was an error during the download.")]
		public FsmString errorString;

		[UIHint(UIHint.Variable)]
		[Tooltip("How far the download progressed (0-1).")]
		public FsmFloat progress;

		[ActionSection("Events")]
		[Tooltip("Event to send when the data has finished loading (progress = 1).")]
		public FsmEvent isDone;

		[Tooltip("Event to send if there was an error.")]
		public FsmEvent isError;

		private UnityWebRequest uwr;

		private DownloadHandlerBuffer d;

		public override void Reset()
		{
			url = null;
			storeText = null;
			storeTexture = null;
			errorString = null;
			progress = null;
			isDone = null;
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(url.Value))
			{
				Finish();
				return;
			}
			if (!storeTexture.IsNone)
			{
				uwr = UnityWebRequestTexture.GetTexture(url.Value);
			}
			else
			{
				uwr = new UnityWebRequest(url.Value);
				d = new DownloadHandlerBuffer();
				uwr.downloadHandler = d;
			}
			uwr.SendWebRequest();
		}

		public override void OnUpdate()
		{
			if (uwr == null)
			{
				errorString.Value = "Unity Web Request is Null!";
				Finish();
				return;
			}
			errorString.Value = uwr.error;
			if (!string.IsNullOrEmpty(uwr.error))
			{
				uwr.Dispose();
				Finish();
				base.Fsm.Event(isError);
				return;
			}
			progress.Value = uwr.downloadProgress;
			if (progress.Value.Equals(1f) && uwr.isDone)
			{
				if (!storeText.IsNone)
				{
					storeText.Value = uwr.downloadHandler.text;
				}
				if (!storeTexture.IsNone)
				{
					storeTexture.Value = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
				}
				errorString.Value = uwr.error;
				uwr.Dispose();
				base.Fsm.Event(string.IsNullOrEmpty(errorString.Value) ? isDone : isError);
				Finish();
			}
		}
	}
}
