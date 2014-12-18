using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeKerbal
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class EventManager : MonoBehaviour
    {
        private IVAControl _IVAControl;
        private EVAControl _EVAControl;
        private KerbalInfo _CurrentKerbal;

        private static EventManager _Instance = null;
        public static EventManager Instance
        {
            get
            {
                return _Instance;
            }
            private set
            {
                _Instance = value;
            }
        }

        #region Events
        public event StartEVAHandler StartEVA;
        public event UpdateEVAHandler UpdateEVA;
        public event PausedUpdateEVAHandler PausedUpdateEVA;
        public event StopEVAHandler StopEVA;
        public event StartIVAHandler StartIVA;
        public event UpdateIVAHandler UpdateIVA;
        public event PausedUpdateIVAHandler PausedUpdateIVA;
        public event StopIVAHandler StopIVA;
        public event SwitchKerbalHandler SwitchKerbal;

        #region Handlers
        public delegate void StartEVAHandler(KerbalInfo kerbal);
        public delegate void UpdateEVAHandler(KerbalInfo kerbal);
        public delegate void PausedUpdateEVAHandler(KerbalInfo kerbal);
        public delegate void StopEVAHandler(KerbalInfo kerbal);
        public delegate void StartIVAHandler(KerbalInfo kerbal);
        public delegate void UpdateIVAHandler(KerbalInfo kerbal);
        public delegate void PausedUpdateIVAHandler(KerbalInfo kerbal);
        public delegate void StopIVAHandler(KerbalInfo kerbal);
        public delegate void SwitchKerbalHandler(KerbalInfo previous, KerbalInfo next);
        #endregion Handlers

        #region Functions
        protected virtual void OnStartEVA(KerbalInfo kerbal)
        {
            if (StartEVA != null)
            {
                try
                {
                    StartEVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in StartEVA: " + e.Message);
                }
            }
        }

        protected virtual void OnUpdateEVA(KerbalInfo kerbal)
        {
            if (UpdateEVA != null)
            {
                try
                {
                    UpdateEVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in UpdateEVA: " + e.Message);
                }
            }
        }

        protected virtual void OnPausedUpdateEVA(KerbalInfo kerbal)
        {
            if (PausedUpdateEVA != null)
            {
                try
                {
                    PausedUpdateEVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in PausedUpdateEVA: " + e.Message);
                }
            }
        }

        protected virtual void OnStopEVA(KerbalInfo kerbal)
        {
            if (StopEVA != null)
            {
                try
                {
                    StopEVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in StopEVA: " + e.Message);
                }
            }
        }

        protected virtual void OnStartIVA(KerbalInfo kerbal)
        {
            if (StartIVA != null)
            {
                try
                {
                    StartIVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in StartIVA: " + e.Message);
                }
            }
        }

        protected virtual void OnUpdateIVA(KerbalInfo kerbal)
        {
            if (UpdateIVA != null)
            {
                try
                {
                    UpdateIVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in UpdateIVA: " + e.Message);
                }
            }
        }

        protected virtual void OnPausedUpdateIVA(KerbalInfo kerbal)
        {
            if (PausedUpdateIVA != null)
            {
                try
                {
                    PausedUpdateIVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in PausedUpdateIVA: " + e.Message);
                }
            }
        }

        protected virtual void OnStopIVA(KerbalInfo kerbal)
        {
            if (StopIVA != null)
            {
                try
                {
                    StopEVA(kerbal);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in StopIVA: " + e.Message);
                }
            }
        }

        protected virtual void OnSwitchKerbal(KerbalInfo previous, KerbalInfo next)
        {
            if (SwitchKerbal != null)
            {
                try
                {
                    SwitchKerbal(previous, next);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in SwitchKerbal: " + e.Message);
                }
            }
        }
        #endregion Functions
        #endregion Events

        void Start()
        {
            Debug.Log("EventManager started");

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("BeKerbal: There is more than one EventManager in the current scene. Destroying the excess.");
                Destroy(this);
            }

            _IVAControl = new IVAControl();
            _EVAControl = new EVAControl();

            InputLockManager.SetControlLock(ControlTypes.MAP, BeKerbal.Settings.MapControl_LockID);

            _CurrentKerbal = new KerbalInfo(FlightGlobals.ActiveVessel);
            if (_CurrentKerbal.IsEVA)
            {
                OnStartEVA(_CurrentKerbal);
            }
            else
            {
                OnStartIVA(_CurrentKerbal);
            }
        }

        void OnDestroy()
        {
            InputLockManager.RemoveControlLock(BeKerbal.Settings.MapControl_LockID);
        }

        void Update()
        {
            if (FlightDriver.Pause)
            {
                if (_CurrentKerbal.IsEVA)
                {
                    OnPausedUpdateEVA(_CurrentKerbal);
                }
                else if (!_CurrentKerbal.IsEVA)
                {
                    OnPausedUpdateIVA(_CurrentKerbal);
                }
                return;
            }

            KerbalInfo lastKerbal = _CurrentKerbal;
            KerbalInfo currentKerbal = new KerbalInfo(FlightGlobals.ActiveVessel);

            if (!_KerbalEquals(lastKerbal, currentKerbal))
            {
                Debug.Log("Switch pEVA=" + lastKerbal.IsEVA + " nEVA=" + currentKerbal.IsEVA);
                OnSwitchKerbal(lastKerbal, currentKerbal);
                if (lastKerbal.IsEVA && currentKerbal.IsEVA)
                {
                    OnStopEVA(lastKerbal);
                    OnStartEVA(currentKerbal);
                }
                else if (!lastKerbal.IsEVA && !currentKerbal.IsEVA)
                {
                    OnStopIVA(lastKerbal);
                    OnStartIVA(currentKerbal);
                }
            }

            if (!lastKerbal.IsEVA && currentKerbal.IsEVA)
            {
                OnStopIVA(currentKerbal);
                OnStartEVA(currentKerbal);
            }
            else if (lastKerbal.IsEVA && !currentKerbal.IsEVA)
            {
                OnStopEVA(currentKerbal);
                OnStartIVA(currentKerbal);
            }
            
            if (lastKerbal.IsEVA && currentKerbal.IsEVA)
            {
                OnUpdateEVA(currentKerbal);
            }
            else if (!lastKerbal.IsEVA && !currentKerbal.IsEVA)
            {
                OnUpdateIVA(currentKerbal);
            }

            _CurrentKerbal = new KerbalInfo(currentKerbal);
        }

        void LateUpdate()
        {
            // Just to make sure...
            if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Map || MapView.MapIsEnabled)
            {
                MapView.ExitMapView();
            }
        }

        private bool _KerbalEquals(KerbalInfo a, KerbalInfo b) { return _KerbalEquals(a.Kerbal, b.Kerbal); }
        private bool _KerbalEquals(Kerbal a, Kerbal b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if ((a != null && b == null) || (a == null && b != null))
            {
                return false;
            }
            if (a.crewMemberName != b.crewMemberName)
            {
                return false;
            }
            return true;
        }
    }
}
