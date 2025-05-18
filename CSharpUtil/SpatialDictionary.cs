using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Geom_Util.Interfaces;
using Geom_Util;
using System.Linq;

namespace Godot_Util.CSharp_Util
{
    public interface ISpatialValue<KeyT> : IBounded
    {
        KeyT Key { get; set; }
    }

    // store a value against an IBounded key, so we can search by the key value, OR, by location in space
    [DebuggerDisplay("{Forwards.Count} ({SpaceMap.GetBounds().Min.X}, {SpaceMap.GetBounds().Min.Y}, {SpaceMap.GetBounds().Min.Z}) -> ({SpaceMap.GetBounds().Max.X}, {SpaceMap.GetBounds().Max.Y}, {SpaceMap.GetBounds().Max.Z})")]
    public class SpatialDictionary<Key, BoundedValue> : IEnumerable<KeyValuePair<Key, BoundedValue>> where BoundedValue : class, ISpatialValue<Key>
    {
        Dictionary<Key, BoundedValue> ForwardsInner = [];

        RTree<BoundedValue> SpaceMap = [];

        public SpatialDictionary() {}

        public SpatialDictionary(IEnumerable<KeyValuePair<Key, BoundedValue>> pairs)
        {
            foreach(var pair in pairs)
            {
                this[pair.Key] = pair.Value;
            }
        }

        public IEnumerable<Key> FindKeys(ImBounds bounds, IReadOnlyRTree.SearchMode mode)
        {
            foreach(BoundedValue value in SpaceMap.Search(bounds, mode))
            {
                yield return value.Key;
            }
        }

        public IEnumerable<BoundedValue> FindValues(ImBounds bounds, IReadOnlyRTree.SearchMode mode)
        {
            foreach(BoundedValue idx in SpaceMap.Search(bounds, mode)) {
                yield return idx;
            }
        }

        public BoundedValue this [Key idx]
        {
            get => ForwardsInner[idx];
            set => Add(value, idx);
        }

        public int Count
        {
            get => ForwardsInner.Count;
        }

        void Add(BoundedValue t1, Key t2)
        {
            ForwardsInner[t2] = t1;
            SpaceMap.Insert(t1);

            t1.Key = t2;
        }

        public BoundedValue Remove(Key idx)
        {
            BoundedValue value = ForwardsInner[idx];
            ForwardsInner.Remove(idx);
            SpaceMap.Remove(value);

            return value;
        }

        public bool Contains(BoundedValue val)
        {
            return FindValues(val.GetBounds(), IReadOnlyRTree.SearchMode.ExactMatch)
                .Where(x => ReferenceEquals(x, val))
                .Any();
        }

        public bool Contains(Key idx)
        {
            return ForwardsInner.ContainsKey(idx);
        }

        public IEnumerable<Key> Keys
        {
            get => ForwardsInner.Keys;
        }

        public IEnumerable<BoundedValue> Values
        {
            get => ForwardsInner.Values;
        }

        public IEnumerator<KeyValuePair<Key, BoundedValue>> GetEnumerator() => ForwardsInner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ForwardsInner.GetEnumerator();

        public IReadOnlyDictionary<Key, BoundedValue> Forwards { get => ForwardsInner; }
    }
}