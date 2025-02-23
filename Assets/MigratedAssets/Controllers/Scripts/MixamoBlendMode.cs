using System;

public enum MixamoBlendMode
{
    Tree1D, Tree2D, Tree3D
}

public static class MixamoBlendModeExtension
{
    public static int ToDimension(this MixamoBlendMode mode)
    {
        return (int)mode + 1;
    }

    public static int ToNumOfParams(this MixamoBlendMode mode)
    {
        return (int)Math.Pow(2, mode.ToDimension());
    }

    public static MixamoBlendMode DimensionToMode(int dimension)
    {
        int maxDimension = Enum.GetNames(typeof(MixamoBlendMode)).Length;
        if (dimension > maxDimension) //dimension is restricted to 3 for now 
        {
            dimension = maxDimension;
        }
        return (MixamoBlendMode)(dimension - 1);
    }
}
