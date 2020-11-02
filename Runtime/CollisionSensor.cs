using System;
using System.Collections.Generic;
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
                OnItemAdded(item);
                CallbackCollisionEnter?.Invoke();
            }

            UpdateDebug();
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


                OnItemRemoved((item));
                CallbackCollisionExit?.Invoke();
            }

            UpdateDebug();
        }


        private void UpdateDebug()
        {
#if UNITY_EDITOR


            var sb = new StringBuilder();
            foreach (var kv in Items)
            {
                var v = kv.Value?.transform;
                if (v == null)
                {
                    continue;
                }

                sb.AppendLine(v.name);
            }

            debugSensor = $"{sb}";
#endif
        }
    }
}