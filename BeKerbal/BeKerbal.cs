using System.Collections.Generic;
using UnityEngine;

namespace BeKerbal
{
    /// <summary>
    /// How to deal with external seats from a user point of view?
    ///     - As in IVA: select a default kerbal then cycle through using V. Could set that the default kerbal is the one that just sat.
    ///     The issue is that the game treat external seat as a command pod, need to dig this
    /// TODO: KerbQuake support, custom EVA quaking (since no use of the FlightCamera thingy)
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class BeKerbal : MonoBehaviour
    {
        public static BeKerbalSettings Settings { get; private set; }
        public static Kerbal CurrentKerbal;
        public static Vessel CurrentVessel;

        void Start()
        {
            Settings = new BeKerbalSettings()
            {
                EVA_EyeOffset = Vector3.back * 0.0f,//0.1f,
                EVA_HeadLocation = Vector3.up * 0.35f,
                EVA_MaxPitch = 25.0f,
                EVA_MinPitch = -25.0f,
                EVA_MaxYaw = 45.0f,
                EVA_MinYaw = -45.0f,
                EVA_LookSentivity = 1.0f,
                EVA_ViewTurnPercent = 0.25f,
                EVA_RagdollComponentReferenceName = "helmetCollider",
                EVA_HiddenComponentsName = new string[] { "headMesh", "eyeball", "upTeeth", "pupil", },
                EVA_HelmetColliderComponentName = "helmetCollider",
                EVA_HelmetColliderRadiusFactor = 1.5f,
                EVA_NearClip = 0.01f,

                IVA_GoToEVAKeyCode = KeyCode.Backspace,

                MapControl_LockID = "BeKerbal_MapControlID",
            };

            CurrentKerbal = null;
        }
    }
}
