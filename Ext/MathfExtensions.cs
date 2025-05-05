using System.Linq;
using Godot;

static class MathfExtensions
{
    public static int Max(params int[] values)
    {
        int max = values.First();

        foreach(int val in values.Skip(1))
        {
            max = Mathf.Max(max, val);
        }

        return max;
    }

    public static float Max(params float[] values)
    {
        float max = values.First();

        foreach(float val in values.Skip(1))
        {
            max = Mathf.Max(max, val);
        }

        return max;
    }
}