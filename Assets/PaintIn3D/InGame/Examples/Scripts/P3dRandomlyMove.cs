using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to define a team with a specified color. Put it in its own GameObject so you can give it a unique name.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dRandomlyMove")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Randomly Move")]
	public class P3dRandomlyMove : MonoBehaviour
	{
		/// <summary>The minimum position that can be moved to.</summary>
		public Vector3 Minimum { set { minimum = value; } get { return minimum; } } [SerializeField] private Vector3 minimum = -Vector3.one;

		/// <summary>The maximum position that can be moved to.</summary>
		public Vector3 Maximum { set { maximum = value; } get { return maximum; } } [SerializeField] private Vector3 maximum = Vector3.one;

		/// <summary>The interval between movements in seconds.</summary>
		public float Interval { set { interval = value; } get { return interval; } } [SerializeField] private float interval = 5.0f;

		/// <summary>The interval between movements in seconds.</summary>
		public float Dampening { set { dampening = value; } get { return dampening; } } [SerializeField] private float dampening = 0.1f;

		[SerializeField]
		private float age;

		[SerializeField]
		private Vector3 target;

		[ContextMenu("Move Now")]
		public void MoveNow()
		{
			age = 0.0f;

			target.x = Mathf.Lerp(minimum.x, maximum.x, Random.value);
			target.y = Mathf.Lerp(minimum.y, maximum.y, Random.value);
			target.z = Mathf.Lerp(minimum.z, maximum.z, Random.value);
		}

		protected virtual void Start()
		{
			MoveNow();
		}

		protected virtual void Update()
		{
			var position = transform.localPosition;
			var factor   = P3dHelper.DampenFactor(dampening, Time.deltaTime);

			age += Time.deltaTime;

			if (age >= interval)
			{
				age %= interval;

				MoveNow();
			}

			position = Vector3.Lerp(position, target, factor);

			transform.localPosition = position;
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			if (transform.parent != null)
			{
				Gizmos.matrix = transform.parent.localToWorldMatrix;
			}

			var center = (minimum + maximum) * 0.5f;
			var size   = maximum - minimum;

			Gizmos.DrawWireCube(center, size);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomEditor(typeof(P3dRandomlyMove))]
	public class P3dRandomlyMove_Editor : P3dEditor<P3dRandomlyMove>
	{
		protected override void OnInspector()
		{
			Draw("minimum", "The minimum position that can be moved to.");
			Draw("maximum", "The maximum position that can be moved to.");
			Draw("interval", "The interval between movements in seconds.");
			Draw("dampening", "The interval between movements in seconds.");
		}
	}
}
#endif