using System.Collections.Generic;
using UnityEngine;

namespace CollisionSensors.Runtime
{
    public class CollisionData<T>
    {
        public T Item { get; set; }
        public HashSet<int> Colliders { get; }
        public int Count => Colliders.Count;

        public CollisionData()
        {
            Colliders = new HashSet<int>();
        }

        public bool Add(int id)
        {
            int count = Count;
            Colliders.Add(id);
            return count == 0 && Count > 0;
        }

        public bool Remove(int id)
        {
            int count = Count;
            Colliders.Remove(id);
            return count > 0 && Count == 0;
        }
    }
}