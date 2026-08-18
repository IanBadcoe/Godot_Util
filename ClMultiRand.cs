using System;
using Chickensoft.Collections;

namespace Godot_Util;

public static class ClRandExtensions
{
    public static ClMultiRand NewClMultiRand(this ClRand rand)
    {
        return new ClMultiRand(rand.Next());
    }
}

public class ClMultiRand(int Seed)
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

    int SeedHash { get; init; } = Seed.GetHashCode();

    readonly Map<string, ClRand> RNGs = [];

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

    ClRand FindCreateRNG(string name)
    {
        if (!RNGs.ContainsKey(name))
        {
            var name_hash = name.GetHashCode();

            RNGs[name] = new ClRand(HashCode.Combine(SeedHash, name_hash));
        }

        return RNGs[name];
    }
}