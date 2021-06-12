namespace PaintIn3D
{
	/// <summary>This interface allows you to create components that modify the paint opacity.</summary>
	public interface IModifyOpacity
	{
		void ModifyOpacity(float pressure, ref float opacity);
	}
}