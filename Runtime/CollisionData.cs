using System.Collections.Generic;
using UnityEngine;

namespace CollisionSensors.Runtime
{
    public class CollisionData<T>
    {
        public T Item { get; set; }
        public Dictionary<int, Collider> Colliders { get; }
        public int Count => Colliders.Count;

        public CollisionData()
        {
            Colliders = new Dictionary<int, Collider>();
        }

        public bool AddCollider(int id, Collider collider)
        {
            return Colliders[id] = collider;
        }

        public bool RemoveCollider(int id)
        {
            return Colliders.Remove(id);
        }
    }
}