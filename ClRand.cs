using System;
using System.Collections.Generic;

using Godot;

namespace Godot_Util
{
    // taken from MS sample code and only changed in name and
    // making it cloneable

    // ==++==
    //
    //   Copyright (c) Microsoft Corporation.  All rights reserved.
    //
    // ==--==
    /*============================================================
    **
    ** Class:  Random
    **
    **
    ** Purpose: A random number generator.
    **
    **
    ===========================================================*/
    public class ClRand
    {
        //
        // Private Constants
        //
        const int MAX_INT = Int32.MaxValue;
        const int MAX_SEED = 161803398;
        const int MZ = 0;


        //
        // Member Variables
        //
        int INext;
        int INextP;
        readonly int[] SeedArray = new int[56];

        //
        // Public Constants
        //

        //
        // Native Declarations
        //

        //
        // Constructors
        //

        public ClRand()
            : this((int)Time.GetTicksUsec())
        {
        }

        public T RandomFromList<T>(List<T> list)
        {
            return list[IntRange(0, list.Count)];
        }

        public ClRand(int Seed)
        {
            int ii;
            int mj, mk;

            //Initialize our Seed array.
            //This algorithm comes from Numerical Recipes in C (2nd Ed.)
            int subtraction = (Seed == Int32.MinValue) ? Int32.MaxValue : Mathf.Abs(Seed);
            mj = MAX_SEED - subtraction;
            SeedArray[55] = mj;
            mk = 1;
            for (int i = 1; i < 55; i++)
            {  //Apparently the range [1..55] is special (Knuth) and so we're wasting the 0'th position.
                ii = (21 * i) % 55;
                SeedArray[ii] = mk;
                mk = mj - mk;
                if (mk < 0)
                {
                    mk += MAX_INT;
                }

                mj = SeedArray[ii];
            }
            for (int k = 1; k < 5; k++)
            {
                for (int i = 1; i < 56; i++)
                {
                    SeedArray[i] -= SeedArray[1 + (i + 30) % 55];
                    if (SeedArray[i] < 0)
                    {
                        SeedArray[i] += MAX_INT;
                    }
                }
            }
            INext = 0;
            INextP = 21;
            Seed = 1;
        }

        ClRand Clone()
        {
            return new ClRand(this);
        }

        private ClRand(ClRand old)
        {
            INext = old.INext;
            INextP = old.INextP;
            for (int i = 0; i < 56; i++)
            {
                SeedArray[i] = old.SeedArray[i];
            }
        }

        private ClRand NextRand()
        {
            return new ClRand(Next());
        }

        //
        // Package Private Methods
        //

        /*====================================Sample====================================
        **Action: Return a new random number [0..1) and reSeed the Seed array.
        **Returns: A float [0..1)
        **Arguments: None
        **Exceptions: None
        ==============================================================================*/
        protected float Sample()
        {
            //Including this division at the end gives us significantly improved
            //random number distribution.
            return (InternalSample() * (1.0f / MAX_INT));
        }

        int InternalSample()
        {
            int retVal;
            int locINext = INext;
            int locINextP = INextP;

            if (++locINext >= 56)
            {
                locINext = 1;
            }

            if (++locINextP >= 56)
            {
                locINextP = 1;
            }

            retVal = SeedArray[locINext] - SeedArray[locINextP];

            if (retVal == MAX_INT)
            {
                retVal--;
            }

            if (retVal < 0)
            {
                retVal += MAX_INT;
            }

            SeedArray[locINext] = retVal;

            INext = locINext;
            INextP = locINextP;

            return retVal;
        }

        //
        // Public Instance Methods
        //


        /*=====================================Next=====================================
        **Returns: An int [0..Int32.MaxValue)
        **Arguments: None
        **Exceptions: None.
        ==============================================================================*/
        public int Next()
        {
            return InternalSample();
        }

        float GetSampleForLargeRange()
        {
            // The distribution of float value returned by Sample
            // is not distributed well enough for a large range.
            // If we use Sample for a range [Int32.MinValue..Int32.MaxValue)
            // We will end up getting even numbers only.

            int result = InternalSample();
            // Note we can't use addition here. The distribution will be bad if we do that.
            bool negative = (InternalSample() % 2 == 0) ? true : false;  // decide the sign based on second sample
            if (negative)
            {
                result = -result;
            }
            float d = result;
            d += (Int32.MaxValue - 1); // get a number in range [0 .. 2 * Int32MaxValue - 1)
            d /= 2 * (uint)Int32.MaxValue - 1;
            return d;
        }


        /*=====================================Next=====================================
        **Returns: An int [minValue..maxValue)
        **Arguments: minValue -- the least legal value for the Random number.
        **           maxValue -- One greater than the greatest legal return value.
        **Exceptions: None.
        ==============================================================================*/
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue", /*Environment.GetResourceString(*/"Argument_MinMaxValue"/*, "minValue", "maxValue")*/);
            }
            //Contract.EndContractBlock();

            long range = (long)maxValue - minValue;
            if (range <= int.MaxValue)
            {
                return ((int)(Sample() * range) + minValue);
            }
            else
            {
                return (int)((long)(GetSampleForLargeRange() * range) + minValue);
            }
        }


        /*=====================================Next=====================================
        **Returns: An int [0..maxValue)
        **Arguments: maxValue -- One more than the greatest legal return value.
        **Exceptions: None.
        ==============================================================================*/
        public int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue", /*Environment.GetResourceString(*/"ArgumentOutOfRange_MustBePositive"/*, "maxValue")*/);
            }
            //Contract.EndContractBlock();
            return (int)(Sample() * maxValue);
        }


        /*=====================================Next=====================================
        **Returns: A float [0..1)
        **Arguments: None
        **Exceptions: None
        ==============================================================================*/
        public float Float()
        {
            return Sample();
        }


        public int IntRange(int exclusive_max)
        {
            return IntRange(0, exclusive_max);
        }


        public int IntRange(int inclusive_min, int exclusive_max)
        {
            return (int)(Sample() * (exclusive_max - inclusive_min) + inclusive_min);
        }


        public Vector2 Vector2(float low, float high)
        {
            return new Vector2(
                Float() * (high - low) + low,
                Float() * (high - low) + low);
        }

        public float FloatRange(float min, float max)
        {
            return Float() * (max - min) + min;
        }

        // public Vector3I Cell(CellGridBhv grid)
        // {
        //     var bounds = grid.Bounds;

        //     return new InVec3Int(IntRange(0, bounds.x),
        //         IntRange(0, bounds.y),
        //         IntRange(0, bounds.z));
        // }

        public ClRand NewClRand()
        {
            return new ClRand(Next());
        }

        public T EnumerationValue<T>()
        {
            var arr = Enum.GetValues(typeof(T));

            int which = IntRange(0, arr.Length);

            return (T)arr.GetValue(which);
        }

        /*==================================NextBytes===================================
        **Action:  Fills the byte array with random bytes [0..0x7f].  The entire array is filled.
        **Returns:Void
        **Arguments:  buffer -- the array to be filled.
        **Exceptions: None
        ==============================================================================*/
        public void Bytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            //Contract.EndContractBlock();
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(InternalSample() % (Byte.MaxValue + 1));
            }
        }
    }
}
