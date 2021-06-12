using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This class stores lists of IHit__ instances, allowing components like P3dHit__ to easily invoke hit events.</summary>
	public class P3dHitCache
	{
		[System.NonSerialized]
		private bool cached;

		[System.NonSerialized]
		private List<IHitPoint> hitPoints = new List<IHitPoint>();

		[System.NonSerialized]
		private List<IHitLine> hitLines = new List<IHitLine>();

		[System.NonSerialized]
		private List<IHitRaycast> hitRaycasts = new List<IHitRaycast>();

		[System.NonSerialized]
		private static List<IHit> hits = new List<IHit>();

		public bool Cached
		{
			get
			{
				return cached;
			}
		}
#if UNITY_EDITOR
		private static HashSet<object> tempHits = new HashSet<object>();

		public void Inspector(GameObject gameObject, bool direct, bool point, bool line)
		{
			Cache(gameObject);

			tempHits.Clear();

			if (direct == true)
			{
				for (var i = 0; i < hitRaycasts.Count; i++)
				{
					tempHits.Add(hitRaycasts[i]);
				}
			}

			if (point == true)
			{
				for (var i = 0; i < hitPoints.Count; i++)
				{
					tempHits.Add(hitPoints[i]);
				}
			}

			if (line == true)
			{
				for (var i = 0; i < hitLines.Count; i++)
				{
					tempHits.Add(hitLines[i]);
				}
			}

			if (hits.Count == 0)
			{
				EditorGUILayout.HelpBox("This component isn't sending hit events to anything.", MessageType.Warning);
			}
			else
			{
				var output = "This component is sending hit events to:";

				foreach (var hit in tempHits)
				{
					output += "\n" + hit;
				}

				EditorGUILayout.HelpBox(output, MessageType.Info);
			}
		}
#endif
		public void InvokePoints(GameObject gameObject, List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			if (cached == false)
			{
				Cache(gameObject);
			}

			for (var i = 0; i < hitPoints.Count; i++)
			{
				hitPoints[i].HandleHitPoint(commands, repeaters, preview, collider, worldPosition, worldRotation, pressure);
			}
		}

		public void InvokeLines(GameObject gameObject, List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB)
		{
			if (cached == false)
			{
				Cache(gameObject);
			}

			for (var i = 0; i < hitLines.Count; i++)
			{
				hitLines[i].HandleHitLine(commands, repeaters, preview, worldPositionA, worldPositionB, pressureA, pressureB);
			}
		}

		public void InvokeRaycast(GameObject gameObject, List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, RaycastHit hit, float pressure)
		{
			if (cached == false)
			{
				Cache(gameObject);
			}

			for (var i = 0; i < hitRaycasts.Count; i++)
			{
				hitRaycasts[i].HandleHitRaycast(commands, repeaters, preview, hit, pressure);
			}
		}

		public void Clear()
		{
			cached = false;

			hitPoints.Clear();
			hitLines.Clear();
			hitRaycasts.Clear();
		}

		private void Cache(GameObject gameObject)
		{
			cached = true;

			gameObject.GetComponentsInChildren(hits);

			hitPoints.Clear();
			hitLines.Clear();
			hitRaycasts.Clear();

			for (var i = 0; i < hits.Count; i++)
			{
				var hit = hits[i];

				var hitPoint = hit as IHitPoint; if (hitPoint != null) { hitPoints.Add(hitPoint); }

				var hitLine = hit as IHitLine; if (hitLine != null) { hitLines.Add(hitLine); }

				var hitDirect = hit as IHitRaycast; if (hitDirect != null) { hitRaycasts.Add(hitDirect); }
			}
		}
	}
}