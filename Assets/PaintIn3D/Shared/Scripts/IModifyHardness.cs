namespace PaintIn3D
{
	/// <summary>This interface allows you to create components that modify the paint hardness.</summary>
	public interface IModifyHardness
	{
		void ModifyHardness(float pressure, ref float hardness);
	}
}