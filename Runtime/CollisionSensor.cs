﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Sensors.Runtime
{
    public class CollisionSensor<T> : MonoBehaviour
        where T : MonoBehaviour
    {
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


        protected virtual void OnItemAdded(T item)
        {
        }

        protected virtual void OnItemRemoved(T item)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponentInParent<T>();
            if (item == null)
            {
                return;
            }

            var id = item.gameObject.GetInstanceID();
            if (!Items.ContainsKey(id))
            {
                Items[id] = item;
                OnItemAdded(item);
            }

            UpdateDebug();
        }

        private void OnTriggerExit(Collider other)
        {
            var item = other.GetComponentInParent<T>();
            if (item == null)
            {
                return;
            }

            var id = item.gameObject.GetInstanceID();
            if (Items.ContainsKey(id))
            {
                Items.Remove(id);
                OnItemRemoved((item));
                UpdateDebug();
            }
        }

        private void UpdateDebug()
        {
#if UNITY_EDITOR

            var sb = new StringBuilder();
            foreach (var kv in Items)
            {
                sb.AppendLine(kv.Value.transform.name);
            }

            debugSensor = $"{sb}";
#endif
        }
    }
}