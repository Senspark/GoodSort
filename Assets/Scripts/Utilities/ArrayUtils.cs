using System;
using System.Collections.Generic;

namespace Utilities
{
    public class ArrayUtils
    {
        // Shuffle array
        public static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
        
        // Get Random index
        public static int GetRandomIndex(int[] array)
        {
            return UnityEngine.Random.Range(0, array.Length);
        }
        
        // Get random element
        public static T GetRandomElement<T>(T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
        
        // Generate random array from pick from old
        public static T[] GenerateRandomArray<T>(T[] oldArray, int count)
        {
            T[] newArray = new T[count];
            for (var i = 0; i < count; i++)
            {
                newArray[i] = oldArray[UnityEngine.Random.Range(0, oldArray.Length)];
            }
            return newArray;
        }
        
        // Generate random array pick from old, but no duplicate
        public static T[] GenerateRandomArrayNoDuplicate<T>(T[] oldArray, int count)
        {
            if (count > oldArray.Length)
                throw new ArgumentException("Count cannot be greater than array length.");
            
            T[] clone = (T[])oldArray.Clone();

            // Fisher-Yates shuffle
            for (int i = clone.Length - 1; i > 0; i--)
            {
                int randIndex = UnityEngine.Random.Range(0, i + 1);
                (clone[i], clone[randIndex]) = (clone[randIndex], clone[i]);
            }

            T[] newArray = new T[count];
            Array.Copy(clone, newArray, count);

            return newArray;
        }
        
        // Generate random unique indexes from oldArray
        public static int[] GenerateRandomArrayIndexNoDuplicate<T>(T[] oldArray, int count)
        {
            if (count > oldArray.Length)
                throw new ArgumentException("Count cannot be greater than array length.");

            // Tạo mảng index [0..N-1]
            int[] indexes = new int[oldArray.Length];
            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = i;

            // Fisher-Yates shuffle cho index
            for (int i = indexes.Length - 1; i > 0; i--)
            {
                int randIndex = UnityEngine.Random.Range(0, i + 1);
                (indexes[i], indexes[randIndex]) = (indexes[randIndex], indexes[i]);
            }

            // Copy count index đầu tiên
            int[] result = new int[count];
            Array.Copy(indexes, result, count);

            return result;
        }
    }
}