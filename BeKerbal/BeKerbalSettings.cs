using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeKerbal
{
    public struct BeKerbalSettings
    {
        #region EVA
        public Vector3 EVA_EyeOffset;
        public Vector3 EVA_HeadLocation;
        public float EVA_MaxPitch;
        public float EVA_MinPitch;
        public float EVA_MaxYaw;
        public float EVA_MinYaw;
        public float EVA_LookSentivity;
        public float EVA_ViewTurnPercent;
        public string EVA_RagdollComponentReferenceName;
        public string[] EVA_HiddenComponentsName;
        public string EVA_HelmetColliderComponentName;
        public float EVA_HelmetColliderRadiusFactor;
        public float EVA_NearClip;
        #endregion EVA

        #region IVA
        public KeyCode IVA_GoToEVAKeyCode;
        #endregion IVA

        #region Misc
        public String MapControl_LockID;
        #endregion Misc
    }
}
