using System;
using UnityEngine;
using UnityEngine.Events;

namespace Helpers
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
    public class TriggerArea : MonoBehaviour
    {
        public UnityAction<Collider> onTriggerEnter = delegate { };
        public UnityAction<Collider> onTriggerStay = delegate { };
        public UnityAction<Collider> onTriggerExit = delegate { };

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }
    }
}