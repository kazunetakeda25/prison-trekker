namespace PaintIn3D
{
	/// <summary>This interface allows you to create components that modify the paint angle.</summary>
	public interface IModifyAngle
	{
		void ModifyAngle(float pressure, ref float angle);
	}
}