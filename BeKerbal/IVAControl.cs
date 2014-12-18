using System.Collections.Generic;
using UnityEngine;

namespace BeKerbal
{
    /// <summary>
    /// TODO:
    /// As we can right click on the cockpit in thirs person
    /// </summary>
    class IVAControl
    {
        private IVAView _Camera;

        public IVAControl()
        {
            EventManager.Instance.SwitchKerbal += OnSwitchKerbal;
            EventManager.Instance.StartIVA += OnStartIVA;
            EventManager.Instance.StopIVA += OnStopIVA;
            EventManager.Instance.UpdateIVA += OnUpdateIVA;
            EventManager.Instance.PausedUpdateIVA += OnPausedUpdateIVA;

            _Camera = new IVAView();
        }

        private void OnSwitchKerbal(KerbalInfo previous, KerbalInfo next)
        {
            if (next.EVA == null)
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
                if (!previous.IsEVA)
                {
                    // We switch kerbals from IVA to IVA
                    Debug.Log("Switched from Kerbal IVA " + pName + " to IVA " + nName);
                }
                else
                {
                    // We switch kerbals from EVA to IVA
                    Debug.Log("Switched from Kerbal EVA " + pName + " to IVA " + nName);
                }
            }
        }

        private void OnStartIVA(KerbalInfo kerbal)
        {
            _Camera.Start(kerbal);
        }

        private void OnStopIVA(KerbalInfo kerbal)
        {
            _Camera.Stop(kerbal);
        }

        private void OnUpdateIVA(KerbalInfo kerbal)
        {
            _Camera.Update(kerbal);

            Debug.Log("G");
            if (Input.GetKeyDown(BeKerbal.Settings.IVA_GoToEVAKeyCode))
            {
                CameraManager.Instance.SetCameraFlight();
                FlightEVA.SpawnEVA(kerbal.Kerbal);
            }
            Debug.Log("H");
        }

        private void OnPausedUpdateIVA(KerbalInfo kerbal)
        {
            _Camera.PausedUpdate(kerbal);
        }
    }
}
