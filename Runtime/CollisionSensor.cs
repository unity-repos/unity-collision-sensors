using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CollisionSensors.Runtime
{
    public class CollisionSensor<T> : MonoBehaviour, ICollisionSensor
        where T : MonoBehaviour
    {
        [SerializeField] [TextArea(0,10)] private string debugSensor;

        private bool _initialized;
        public Dictionary<int, CollisionData<T>> Items { get; set; }

        public void OnEnable()
        {
            Init();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!TryGetItem(other, out var item))
            {
                return;
            }

            var gameObjectId = item.gameObject.GetInstanceID();
            var colliderId = other.GetInstanceID();

            if (!Items.TryGetValue(gameObjectId, out var collisionData))
            {
                collisionData = new CollisionData<T>
                {
                    Item = item,
                };
                Items[gameObjectId] = collisionData;
            }


            bool didAddItem = collisionData.AddCollider(colliderId);

            Cleanup();
            UpdateDebug();

            if (didAddItem)
            {
                OnItemAdded(item);
                CallbackCollisionEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!TryGetItem(other, out var item))
            {
                return;
            }

            var gameObjectId = item.gameObject.GetInstanceID();
            var colliderId = other.GetInstanceID();

            if (!Items.ContainsKey(gameObjectId))
            {
                return;
            }

            var didRemoveItem = Items[gameObjectId].RemoveCollider(colliderId);

            Cleanup();
            UpdateDebug();

            if (didRemoveItem)
            {
                OnItemRemoved(item);
                CallbackCollisionExit?.Invoke();
            }
        }

        public Action CallbackCollisionEnter { get; set; }
        public Action CallbackCollisionExit { get; set; }
        public int Count => Items.Count;

        public void Init()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            Items = new Dictionary<int, CollisionData<T>>();
        }

        public void Clear()
        {
            Items.Clear();
            UpdateDebug();
        }

        protected virtual bool RejectItem(T other)
        {
            return false;
        }

        protected virtual bool RejectCollider(Collider other)
        {
            return false;
        }

        protected virtual void OnItemAdded(T item)
        {
        }

        protected virtual void OnItemRemoved(T item)
        {
        }

        private bool TryGetItem(Collider other, out T item)
        {
            item = default;
            if (other == null)
            {
                return false;
            }

            if (RejectCollider(other))
            {
                return false;
            }

            item = other.GetComponentInParent<T>();
            if (item == null)
            {
                return false;
            }

            if (RejectItem(item))
            {
                return false;
            }

            return true;
        }

        private void Cleanup()
        {
            var items = Items
                .Where(t => t.Value.Count == 0)
                .Select(t => t.Key)
                .ToArray();

            foreach (var id in items)
            {
                Items.Remove(id);
            }
        }


        private void UpdateDebug()
        {
#if UNITY_EDITOR
            var sb = new StringBuilder();
            foreach (var kv in Items)
            {
                try
                {
                    var collisionData = kv.Value;
                    if (collisionData == null)
                    {
                        continue;
                    }

                    var t = collisionData.Item?.transform;
                    if (t == null)
                    {
                        continue;
                    }

                    sb.AppendLine(t.name);
                    foreach (var id in collisionData.Colliders)
                    {
                        sb.AppendLine($"\t{id}");
                    }
                }
                catch
                {
                    // ignored
                }
            }

            debugSensor = $"{sb}";
#endif
        }
    }
}