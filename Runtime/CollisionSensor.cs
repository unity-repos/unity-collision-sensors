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
        public Dictionary<int, T> Items { get; set; }

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
            Items = new Dictionary<int, T>();
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
            if (other == null)
            {
                return;
            }

            if (RejectCollider(other))
            {
                return;
            }

            var item = other.GetComponentInParent<T>();
            if (item == null)
            {
                return;
            }

            if (RejectItem(item))
            {
                return;
            }

            var id = item.gameObject.GetInstanceID();
            if (!Items.ContainsKey(id))
            {
                Items[id] = item;
                Cleanup();
                UpdateDebug();

                OnItemAdded(item);
                CallbackCollisionEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == null)
            {
                return;
            }

            if (RejectCollider(other))
            {
                return;
            }

            var item = other.GetComponentInParent<T>();
            if (item == null)
            {
                return;
            }

            if (RejectItem(item))
            {
                return;
            }

            var id = item.gameObject.GetInstanceID();
            if (Items.ContainsKey(id))
            {
                Items.Remove(id);
                Cleanup();

                UpdateDebug();
                OnItemRemoved((item));
                CallbackCollisionExit?.Invoke();
            }
        }

        private void Cleanup()
        {
            var items = Items
                .Where(t => t.Value == null)
                .Select(t => t.Key);

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
                if (kv.Value == null)
                {
                    continue;
                }

                var t = kv.Value.transform;
                if (t == null)
                {
                    continue;
                }

                sb.AppendLine(t.name);
            }

            debugSensor = $"{sb}";
#endif
        }
    }
}