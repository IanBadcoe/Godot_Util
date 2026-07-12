using System;
using System.Runtime.CompilerServices;

public static class ArrayExtensions
{
    public enum SwapMode3
    {
        XYZ,
        YXZ,
        YZX,
        ZYX,
        ZXY,
        XZY
    }

    public static T SwappedIndexAccess3<T>(this T[,,] array, int x, int y, int z, SwapMode3 mode)
    {
        (int sx, int sy, int sz) = IndexSwap3(x, y, z, mode);

        return array[sx, sy, sz];
    }

    public static (int sx, int sy, int sz) IndexSwap3(int x, int y, int z, SwapMode3 mode)
    {
        switch(mode)
        {
            case SwapMode3.XYZ:
                return (x, y, z);
            case SwapMode3.YXZ:
                return (y, x, z);
            case SwapMode3.YZX:
                return (y, z, x);
            case SwapMode3.ZYX:
                return (z, y, x);
            case SwapMode3.ZXY:
                return (z, x, y);
            case SwapMode3.XZY:
                return (x, z, y);
        }

        throw new ArgumentException("Did you add a new enum member?", nameof(mode));
    }
}