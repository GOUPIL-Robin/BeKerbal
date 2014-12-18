using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeKerbal
{
    class IVAView : IView
    {
        public void Start(KerbalInfo kerbalInfo)
        {
            Kerbal kerbal = kerbalInfo.Kerbal;
            CameraManager cameraManager = CameraManager.Instance;

            cameraManager.SetCameraIVA(kerbal, true);
        }

        public void Stop(KerbalInfo kerbalInfo)
        {
            Kerbal kerbal = kerbalInfo.Kerbal;
        }

        public void Update(KerbalInfo kerbalInfo)
        {
            Kerbal kerbal = kerbalInfo.Kerbal;
            CameraManager cameraManager = CameraManager.Instance;

            if (cameraManager.currentCameraMode == CameraManager.CameraMode.Flight)
            {
                if (kerbal != null)
                {
                    try
                    {
                        cameraManager.SetCameraIVA(kerbal, true);
                    }
                    catch
                    {
                        cameraManager.SetCameraIVA();
                    }
                }
                else
                {
                    cameraManager.SetCameraIVA();
                }
            }
        }

        public void PausedUpdate(KerbalInfo kerbalInfo)
        {

        }
    }
}
