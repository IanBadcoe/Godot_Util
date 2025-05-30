using System;

namespace Godot_Util
{
    public static class ObjectExtensions
    {
        public static WeakReference<T> AsWeak<T>(this T that) where T : class
        {
            return new WeakReference<T>(that);
        }
    }
}