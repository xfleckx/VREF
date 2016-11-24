using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

namespace Assets.Scripts.VREF.Logic
{
    public class FocusTargetObserver : MonoBehaviour
    {
        /// <summary>
        /// Describes the tag for creating an event whenever a object is in focus
        /// </summary>
        public string TagOfInterest = string.Empty;

        public float maxRayDistance = 1.5f;

        public int TargetLayer = 1;

        private bool rayHitSomething = false;

        private RaycastHit hit;

        private GameObject objectInFocus = null;

        public TargetFocusEvent OnGetInFocus;

        public TargetFocusEvent OnLostFocus;

        void Update()
        {

            int layerMask = 1 << TargetLayer;

            rayHitSomething = Physics.Raycast(transform.position, transform.forward, out hit, maxRayDistance, layerMask, QueryTriggerInteraction.Collide);

            if (rayHitSomething)
            {
                if (hit.collider.tag.Equals(TagOfInterest) && objectInFocus == null)
                {
                    objectInFocus = hit.collider.gameObject;

                    if (OnGetInFocus.GetPersistentEventCount() > 0)
                        OnGetInFocus.Invoke(objectInFocus);
                }
            }
            else if (objectInFocus != null)
            {

                if (OnLostFocus.GetPersistentEventCount() > 0)
                    OnLostFocus.Invoke(objectInFocus);

                //Reset
                objectInFocus = null;
            }
        }

        void OnDrawGizmos()
        {
            var color = rayHitSomething ? Color.green : Color.gray;

            Debug.DrawRay(transform.position, transform.forward * maxRayDistance, color);
        }
    }

    [Serializable]
    public class TargetFocusEvent : UnityEvent<GameObject> { }
}
