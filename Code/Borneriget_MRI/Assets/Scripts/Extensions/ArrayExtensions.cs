using System;

namespace Borneriget.MRI
{
    public static class ArrayExtensions
    {
        public static T SafeGet<T>(this T[] array, int index)
        {
            return array[Math.Min(index, array.Length - 1)];
        }
    }
}
