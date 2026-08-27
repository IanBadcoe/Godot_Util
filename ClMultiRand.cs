using System;
using System.Security.Cryptography;
using System.Text;
using Chickensoft.Collections;

namespace Godot_Util;

public static class ClRandExtensions
{
    public static ClMultiRand NewClMultiRand(this ClRand rand)
    {
        return new ClMultiRand(rand.Next());
    }
}

// does not need to be "Cl*" once we have an interface for RNGs
// as it can just take any RNG through the interface
public class ClMultiRand(int seed)
{
    // Using ClRand as a basis, supply a whole family of RNGs, unrelated in their number streams, but deterministic from a seed on
    // the MultiRand, and an unique name for each child-RNG, thus:
    //
    // ClMultiRand MR = new(123);                   // MR seed
    // ClRand InitialisationRNG = MR["init"];       // one unique deterministic RNG stream (determined by (123, "init"))
    // ClRand EnemyAIRand - MR("enemy");            // another unique deterministic RNG stream, (determined by (123, "enemy"))
    //
    // MR = new(123);                               // reset all RNGs
    // MR.Reset();                                  // (same)
    //
    // ClMultiRand MR = new(345);                   // different MR seed initialises _all_ contained RNGs to different sequences

    public int Seed { get; private init; } = seed;                 // .GetHashCode(); <-- this is inconsistent between runs?  and do we even need it?

    readonly Map<string, ClRand> RNGs = [];

    static SHA256 Sha256 = SHA256.Create();

    public ClRand this [string name]
    {
        get
        {
            return FindCreateRNG(name);
        }
    }

    public ClMultiRand NewMultiRand()
    {
        return new ClMultiRand(this["new"].Next());
    }

    public void Reset()
    {
        // force all descendant RNGs to be recreated and hence reset to their initial state/value
        RNGs.Clear();
    }

    int ConsistentHash(string input)
    {
        byte[] bytes = Sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

        int ret = 0;

        for(int i = 0; i < bytes.Length; i += 4)
        {
            int here = BitConverter.ToInt32(bytes, i);

            ret ^= here;
        }

        return ret;
    }

    ClRand FindCreateRNG(string name)
    {
        if (!RNGs.ContainsKey(name))
        {
            // var name_hash = name.GetHashCode(); string hash inconsistent across vs. of the library (expected)
            //                                     and also I am seeing inconsistency between runs  of the program
            var name_hash = ConsistentHash(name);

            RNGs[name] = new ClRand(HashCode.Combine(Seed, name_hash));
        }

        return RNGs[name];
    }
}