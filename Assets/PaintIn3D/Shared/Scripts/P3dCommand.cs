using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This is the base class for all paint commands. These commands (e.g. paint decal) are added to the command list for each P3dPaintableTexture, and are executed at the end of the frame to optimize state changes.</summary>
	public abstract class P3dCommand
	{
		public P3dModel     Model;
		public P3dGroupMask Groups;
		public bool         Preview;
		public P3dBlendMode Blend;
		public Material     Material;
		public Matrix4x4    Matrix;
		public Vector3      Position;
		public float        Radius;

		public abstract bool RequireMesh
		{
			get;
		}

		public virtual void SetLocation(Matrix4x4 matrix)
		{
			Matrix   = matrix;
			Position = matrix.MultiplyPoint(Vector3.zero);
		}

		public abstract void Apply();
		public abstract void Pool();
		public abstract P3dCommand SpawnCopy();

		public P3dCommand SpawnCopyLocal(Transform transform)
		{
			var copy = SpawnCopy();

			copy.Matrix = transform.worldToLocalMatrix * copy.Matrix;

			return copy;
		}

		public P3dCommand SpawnCopyWorld(Transform transform)
		{
			var copy = SpawnCopy();

			copy.SetLocation(transform.localToWorldMatrix * copy.Matrix);

			return copy;
		}

		protected T SpawnCopy<T>(Stack<T> pool)
			where T : P3dCommand, new()
		{
			var command = pool.Count > 0 ? pool.Pop() : new T();

			command.Model    = Model;
			command.Groups   = Groups;
			command.Preview  = Preview;
			command.Blend    = Blend;
			command.Material = Material;
			command.Matrix   = Matrix;
			command.Position = Position;
			command.Radius   = Radius;

			return command;
		}
	}
}