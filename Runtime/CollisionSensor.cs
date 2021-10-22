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
        public Action CallbackCollisionEnter { get; set; }
        public Action CallbackCollisionExit { get; set; }
        public int Count => Items.Count;
        public Dictionary<int, CollisionData<T>> Items { get; set; }

        [SerializeField] [TextArea] private string debugSensor;

        private bool _initialized;

        public void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            Items = new Dictionary<int, CollisionData<T>>();
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

        private void OnTriggerEnter(Collider other)
        {
            if (!TryGetItem(other, out var item))
            {
                return;
            }

            var id = item.gameObject.GetInstanceID();

            if (!Items.ContainsKey(id))
            {
                if (!Items.ContainsKey(id))
                {
                    var d = new CollisionData<T>()
                    {
                        Item = item
                    };
                    Items.Add(id, d);
                }

                var data = Items[id];
                bool uniqueAdd = data.Add(other.GetInstanceID());

                Cleanup();
                UpdateDebug();
                if (uniqueAdd)
                {
                    OnItemAdded(item);
                    CallbackCollisionEnter?.Invoke();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!TryGetItem(other, out var item))
            {
                return;
            }

            var id = item.gameObject.GetInstanceID();
            if (Items.ContainsKey(id))
            {
                var empty = Items[id].Remove(other.GetInstanceID());

                Cleanup();
                UpdateDebug();
                
                if (empty)
                {
                    OnItemRemoved(item);
                    CallbackCollisionExit?.Invoke();
                }
            }
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
                    if (kv.Value == null)
                    {
                        continue;
                    }

                    var t = kv.Value.Item?.transform;
                    if (t == null)
                    {
                        continue;
                    }

                    sb.AppendLine(t.name);
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