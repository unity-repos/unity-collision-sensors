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

        public bool AddCollider(int id)
        {
            return Colliders.Add(id);
        }

        public bool RemoveCollider(int id)
        {
            return Colliders.Remove(id);
        }
    }
}