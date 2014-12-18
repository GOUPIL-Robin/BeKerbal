using System.Collections.Generic;
using UnityEngine;

namespace BeKerbal
{
    /// <summary>
    /// TODO:
    /// As we can right click on the EVA kerbal in thirdperson
    /// </summary>
    class EVAControl
    {
        private EVAView _Camera;

        public EVAControl()
        {
            EventManager.Instance.SwitchKerbal += OnSwitchKerbal;
            EventManager.Instance.StartEVA += OnStartEVA;
            EventManager.Instance.StopEVA += OnStopEVA;
            EventManager.Instance.UpdateEVA += OnUpdateEVA;
            EventManager.Instance.PausedUpdateEVA += OnPausedUpdateEVA;

            _Camera = new EVAView();
        }

        private void OnSwitchKerbal(KerbalInfo previous, KerbalInfo next)
        {
            if (next.EVA != null)
            {
                string pName = "NULL";
                if (previous.Kerbal != null)
                {
                    pName = previous.Kerbal.crewMemberName;
                }
                string nName = "NULL";
                if (next.Kerbal != null)
                {
                    nName = next.Kerbal.crewMemberName;
                }

                if (previous.IsEVA)
                {
                    // We switch kerbals from EVA to EVA
                    Debug.Log("Switched from Kerbal EVA " + pName + " to EVA " + nName);
                }
                else
                {
                    // We switch kerbals from IVA to EVA
                    Debug.Log("Switched from Kerbal IVA " + pName + " to EVA " + nName);
                }
            }
        }

        private void OnStartEVA(KerbalInfo kerbal)
        {
            _Camera.Start(kerbal);
        }

        private void OnStopEVA(KerbalInfo kerbal)
        {
            _Camera.Stop(kerbal);
        }

        private void OnUpdateEVA(KerbalInfo kerbal)
        {
            bool pressedMove = GameSettings.EVA_forward.GetKey() || GameSettings.EVA_back.GetKey() ||
                            GameSettings.EVA_left.GetKey() || GameSettings.EVA_right.GetKey() ||
                            GameSettings.EVA_Jump.GetKey();
            bool pressedJetpackMove = false;

            if (kerbal.EVA.JetpackDeployed && !kerbal.Vessel.LandedOrSplashed)
            {
                pressedJetpackMove |= GameSettings.EVA_Pack_back.GetKey() || GameSettings.EVA_Pack_forward.GetKey() ||
                            GameSettings.EVA_Pack_left.GetKey() || GameSettings.EVA_Pack_right.GetKey() ||
                            GameSettings.EVA_Pack_down.GetKey() || GameSettings.EVA_Pack_up.GetKey() ||
                            GameSettings.EVA_yaw_left.GetKey() || GameSettings.EVA_yaw_right.GetKey();
            }
            pressedMove |= pressedJetpackMove;

            _Camera.PressedMove = pressedMove;
            _Camera.Update(kerbal);
        }

        private void OnPausedUpdateEVA(KerbalInfo kerbal)
        {
            _Camera.PausedUpdate(kerbal);
        }

        /*
        public static void debugDir(Transform parent, Vector3 dir, Color begin, Color end)
        {
            GameObject obj = new GameObject("Line");
            LineRenderer line = null;

            line = obj.AddComponent<LineRenderer>();
            line.transform.parent = parent;
            line.useWorldSpace = false;
            line.transform.localPosition = Vector3.zero;
            line.transform.localEulerAngles = Vector3.zero;

            line.material = new Material(Shader.Find("Particles/Additive"));
            line.SetColors(begin, end);
            line.SetWidth(0.1f, 0);
            line.SetVertexCount(2);
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, dir * 2); 
        }
        */
    }
}
