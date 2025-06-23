using UnityEngine;

namespace Humelo
{
    [DisallowMultipleComponent]
    class LavstarLipsyncForBS : LavstarLipsync
    {
        private const int InvalidIndex = -1;


        [Header("Lipsync Target Object (for Blendshape Animation)")]
        [Tooltip("Put a SkinnedMeshRenderer object here for Lipsync")]
        public SkinnedMeshRenderer SkinnedMeshRendererTarget = null; ///< As the name implies

        [Header("Target Blendshape Name")]
        [Tooltip("Enter the name of Blendshape for lipsync")]
        public string SkinnedMeshRendererTargetBSName = "blendShape2.jawOpen"; ///< As the name implies

        [Header("Lerp Tunning Value for blendshape lipsync")]
        [Tooltip("Adjust when the lipsync result is not smooth")]
        public float BSOpenLerpInterpValue = 50.0f; ///< As the name implies

        [Header("Multiply Value for Lipsync (for Blendshape Animation)")]
        [Tooltip("for adjusting lipsync")]
        public float BSOpenMultiply = 300.0f; ///< As the name implies

        private int SkinnedMeshRendererTargetIndex = InvalidIndex; ///< As the name implies

        protected new void Awake()
        {
            base.Awake();

            if (SkinnedMeshRendererTarget == null)
                SkinnedMeshRendererTarget = gameObject.GetComponent<SkinnedMeshRenderer>();

            SkinnedMeshRendererTargetIndex = getBlendShapeIndex(SkinnedMeshRendererTarget, SkinnedMeshRendererTargetBSName);            
        }

        /*!
        * @brief A function for getting blendshape index by name.
        * @return int
        */
        public int getBlendShapeIndex(SkinnedMeshRenderer smr, string bsName)
        {
            Mesh m = smr.sharedMesh;

            for (int i = 0; i < m.blendShapeCount; i++)
            {
                string name = m.GetBlendShapeName(i);
                if (bsName.Equals(m.GetBlendShapeName(i)) == true)
                    return i;
            }

            return InvalidIndex;
        }

        protected new void Update()
        {
            base.Update();
            
            if (SkinnedMeshRendererTarget != null && SkinnedMeshRendererTargetIndex != InvalidIndex)
            {
                float interpValue = Time.deltaTime * BSOpenLerpInterpValue;

                float fJawValue = SkinnedMeshRendererTarget.GetBlendShapeWeight(SkinnedMeshRendererTargetIndex);

                fJawValue = Mathf.Lerp(fJawValue, levels[10] * BSOpenMultiply, interpValue);

                SkinnedMeshRendererTarget.SetBlendShapeWeight(SkinnedMeshRendererTargetIndex, fJawValue);
            }
        }
    }
}
// © 2019-2020 Humelo Inc.
