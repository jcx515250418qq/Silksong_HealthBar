using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	public class TMP_UpdateManager
	{
		private static TMP_UpdateManager s_Instance;

		private readonly List<TMP_Text> m_LayoutRebuildQueue = new List<TMP_Text>();

		private Dictionary<int, int> m_LayoutQueueLookup = new Dictionary<int, int>();

		private readonly List<TMP_Text> m_GraphicRebuildQueue = new List<TMP_Text>();

		private Dictionary<int, int> m_GraphicQueueLookup = new Dictionary<int, int>();

		public static TMP_UpdateManager instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new TMP_UpdateManager();
				}
				return s_Instance;
			}
		}

		protected TMP_UpdateManager()
		{
			Canvas.willRenderCanvases += DoRebuild;
		}

		public static void RegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			instance.InternalRegisterTextElementForLayoutRebuild(element);
		}

		private bool InternalRegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			if (m_LayoutQueueLookup.ContainsKey(instanceID))
			{
				return false;
			}
			m_LayoutQueueLookup[instanceID] = instanceID;
			m_LayoutRebuildQueue.Add(element);
			return true;
		}

		public static void RegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			instance.InternalRegisterTextElementForGraphicRebuild(element);
		}

		private bool InternalRegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			if (m_GraphicQueueLookup.ContainsKey(instanceID))
			{
				return false;
			}
			m_GraphicQueueLookup[instanceID] = instanceID;
			m_GraphicRebuildQueue.Add(element);
			return true;
		}

		private void OnCameraPreRender(Camera cam)
		{
			DoRebuild();
		}

		private void DoRebuild()
		{
			for (int i = 0; i < m_LayoutRebuildQueue.Count; i++)
			{
				m_LayoutRebuildQueue[i].Rebuild(CanvasUpdate.Prelayout);
			}
			if (m_LayoutRebuildQueue.Count > 0)
			{
				m_LayoutRebuildQueue.Clear();
				m_LayoutQueueLookup.Clear();
			}
			for (int j = 0; j < m_GraphicRebuildQueue.Count; j++)
			{
				m_GraphicRebuildQueue[j].Rebuild(CanvasUpdate.PreRender);
			}
			if (m_GraphicRebuildQueue.Count > 0)
			{
				m_GraphicRebuildQueue.Clear();
				m_GraphicQueueLookup.Clear();
			}
		}

		public static void UnRegisterTextElementForRebuild(TMP_Text element)
		{
			instance.InternalUnRegisterTextElementForGraphicRebuild(element);
			instance.InternalUnRegisterTextElementForLayoutRebuild(element);
		}

		private void InternalUnRegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			instance.m_GraphicRebuildQueue.Remove(element);
			m_GraphicQueueLookup.Remove(instanceID);
		}

		private void InternalUnRegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			instance.m_LayoutRebuildQueue.Remove(element);
			m_LayoutQueueLookup.Remove(instanceID);
		}
	}
}
