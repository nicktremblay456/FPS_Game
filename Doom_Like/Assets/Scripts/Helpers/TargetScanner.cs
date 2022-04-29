using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNT
{
    [System.Serializable]
    public class TargetScanner
    {
        public float heightOffset = 0.0f;
        public float detectionRadius = 10;
        [Range(0.0f, 360.0f)]
        public float detectionAngle = 270;
        public float maxHeightDifference = 1.0f;
        public LayerMask viewBlockerLayerMask;

        public PlayerController Detect(Transform detector, bool useHeightDifference = true)
        {
            //if either the player is not spawned or they are spawning, we do not target them
            if (PlayerController.Instance == null)
            {
                return null;
            }
            Vector3 eyePos = detector.position + Vector3.up * heightOffset;
            Vector3 toPlayer = PlayerController.Instance.transform.position - eyePos;
            Vector3 toPlayerTop = PlayerController.Instance.transform.position + Vector3.up * 1.5f - eyePos;

            if (useHeightDifference && Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
            {
                //if the target is too high or to low no need to try to reach it, just abandon pursuit
                return null;
            }

            Vector3 toPlayerFlat = toPlayer;
            toPlayerFlat.y = 0;

            if (toPlayerFlat.sqrMagnitude <= detectionRadius * detectionRadius)
            {
                if (Vector3.Dot(toPlayerFlat.normalized, detector.forward) >
                    Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
                {
                    bool canSee = false;

                    Debug.DrawRay(eyePos, toPlayer, Color.blue);
                    Debug.DrawRay(eyePos, toPlayerTop, Color.blue);

                    canSee |= !Physics.Raycast(eyePos, toPlayer.normalized, detectionRadius,
                        viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                    canSee |= !Physics.Raycast(eyePos, toPlayerTop.normalized, toPlayerTop.magnitude,
                        viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                    if (canSee)
                        return PlayerController.Instance;
                }
            }

            return null;
        }

    #if UNITY_EDITOR
            public void EditorGizmo(Transform transform)
            {
                Color c = new Color(0, 0, 0.7f, 0.4f);

                UnityEditor.Handles.color = c;
                Vector3 rotateForward = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
                UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotateForward, detectionAngle, detectionRadius);

                Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                Gizmos.DrawWireSphere(transform.position + Vector3.up * heightOffset, 0.2f);
            }
    #endif
    }
}
