using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to spawn a prefab at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dSpawner")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Spawner")]
	public class P3dSpawner : MonoBehaviour, IHit, IHitPoint
	{
		/// <summary>The prefab that will be spawned.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		/// <summary>The offset from the hit point based on the normal in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>Call this if you want to manually spawn the specified prefab.</summary>
		public void Spawn()
		{
			Spawn(transform.position, transform.rotation);
		}

		public void Spawn(Vector3 position, Vector3 normal)
		{
			Spawn(position, Quaternion.LookRotation(normal));
		}

		public void Spawn(Vector3 position, Quaternion rotation)
		{
			if (prefab != null)
			{
				var clone = Instantiate(prefab, position, transform.rotation, default(Transform));

				clone.SetActive(true);
			}
		}

		public void HandleHitPoint(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			Spawn(worldPosition + worldRotation * Vector3.forward * offset, worldRotation);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dSpawner))]
	public class P3dSpawner_Editor : P3dEditor<P3dSpawner>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				Draw("prefab", "The prefab that will be spawned.");
			EndError();
			Draw("offset", "The offset from the hit point based on the normal in world space.");
		}
	}
}
#endif