using UnityEngine;

namespace Humelo
{
    [DisallowMultipleComponent]
    class LavstarLipsyncForJaw :  LavstarLipsync
    {
        [Header("Lipsync Target Object (for Jaw Rotation)")]
        [Tooltip("Put a jaw joint object here for Lipsync")]
        public GameObject JawRotationTargetObject = null; ///< As the name implies

        [Header("Lerp Tunning Value for Jaw Rotation")]
        [Tooltip("Adjust when the lipsync result is not smooth")]
        public float JawRotationLerpInterpValue = 70.0f; ///< As the name implies

        [Header("Multiply Value for Lipsync (for Jaw Rotation)")]
        [Tooltip("for adjusting lipsync")]
        public float JawRotationMultiply = 500.0f; ///< As the name implies
            
        public enum JawRotationAxis
        {
            Axis_X,
            Axis_Y,
            Axis_Z
        }

        public JawRotationAxis rotAxis = JawRotationAxis.Axis_X;

        public float bias = 0.0f;

        protected new void Awake()
        {
            base.Awake();

            if (JawRotationTargetObject == null)
                JawRotationTargetObject = gameObject;
        }

        protected new void Update()
        {
            base.Update();

            if (JawRotationTargetObject != null)
            {
                float interpValue = Time.deltaTime * JawRotationLerpInterpValue;

                float destValue = levels[10] * JawRotationMultiply;

                Vector3 targetLocalAngle = JawRotationTargetObject.transform.localEulerAngles;
                
                if(rotAxis == JawRotationAxis.Axis_X)
                {
                    targetLocalAngle.x = Mathf.Lerp(targetLocalAngle.x, destValue + bias, interpValue);
                }
                else if(rotAxis == JawRotationAxis.Axis_Y)
                {
                    targetLocalAngle.y = Mathf.Lerp(targetLocalAngle.y, destValue + bias, interpValue);

                }
                else
                {
                    targetLocalAngle.z = Mathf.Lerp(targetLocalAngle.z, destValue + bias, interpValue);
                }

                JawRotationTargetObject.transform.localEulerAngles = targetLocalAngle;


                /*
                 * old code 2
                 * 
                float interpValue = Time.deltaTime * JawRotationLerpInterpValue;

                Vector3 rot = JawRotationTargetObject.transform.rotation.eulerAngles;

                rot.x = Mathf.Lerp(rot.x, levels[10] * JawRotationMultiply, interpValue);

                JawRotationTargetObject.transform.rotation = Quaternion.Euler(rot);
                */

                /*
                 * old code 1
                 * 
                float interpValue = Time.deltaTime * JawRotationLerpInterpValue;
                float fJawValue = Mathf.Lerp(JawRotationTargetObject.gameObject.transform.rotation.x, levels[10] * JawRotationMultiply, interpValue);
                JawRotationTargetObject.gameObject.transform.rotation = JawRotationTargetObject.gameObject.transform.parent.rotation * Quaternion.Euler(fJawValue, 0, 0);
                */
            }
        }
    }
}
// © 2019-2020 Humelo Inc.
