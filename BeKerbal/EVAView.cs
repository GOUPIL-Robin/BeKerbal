using System;
using System.Reflection;
using UnityEngine;

namespace BeKerbal
{
    /// <summary>
    /// TODO:
    /// EVA Jetpack rotate without having to move;
    /// EVA Jetpack roll with keyboard;
    /// Unbind yaw left/right and the left mouse button to rotate kerbal in EVA Jetpack;
    /// When recovering, the fall might have the changed the kerbal forward direction, adapt the camera's forward to corespond instead of clamping it.
    ///          Will also handle when you switch back to the kerbal;
    /// The view is shaking when walking up hills;
    /// Recovery is still not working, it seems that we can't follow the mesh simply, we might have to dig deeper into the KerbalEVA to find the actual mesh to follow
    /// When jumping there is a probabiliy that we spin in the air, why;
    /// Increasing the radius of the helmet collider trigger some weird bug where the kerbal is spining when in ragdoll
    /// </summary>
    class EVAView : IView
    {
        public bool PressedMove;

        private Transform _HeadTransform;
        private Vector3 _Forward;
        private Vector3 _Up;
        private float _Pitch;
        private float _Yaw;
        private bool _CursorLocked;
        //private float _OriginHelmetColliderRadius = -1.0f; // Not used anymore but might be used again later

        public void Start(KerbalInfo kerbalInfo)
        {
            PressedMove = false;

            FlightCamera flightCam = FlightCamera.fetch;
            Vessel vessel = kerbalInfo.Vessel;
            KerbalEVA evaInst = kerbalInfo.EVA;

            foreach (Component component in vessel.transform.GetComponentsInChildren(typeof(Transform), true))
            {
                /*if (component.name.Contains(BeKerbal.Settings.EVA_HelmetColliderComponentName) && _OriginHelmetColliderRadius == -1.0f)
                {
                    _OriginHelmetColliderRadius = component.GetComponent<SphereCollider>().radius;
                    component.GetComponent<SphereCollider>().radius *= BeKerbal.Settings.EVA_HelmetColliderRadiusFactor;
                }*/

                if (component.name.Contains(BeKerbal.Settings.EVA_RagdollComponentReferenceName))
                {
                    _HeadTransform = component.transform;
                }

                foreach (string name in BeKerbal.Settings.EVA_HiddenComponentsName)
                {
                    if (component.name.Contains(name) && component.renderer != null)
                    {
                        component.renderer.enabled = false;
                        break;
                    }
                }
            }

            flightCam.DeactivateUpdate();

            _Pitch = 0.0f;
            _Yaw = 0.0f;

            _Forward = evaInst.transform.forward;
            _Up = evaInst.transform.up;

            _CursorLocked = true;

            flightCam.transform.position = Vector3.zero;
        }

        public void Stop(KerbalInfo kerbalInfo)
        {
            FlightCamera flightCam = FlightCamera.fetch;
            Vessel vessel = kerbalInfo.Vessel;
            KerbalEVA evaInst = kerbalInfo.EVA;

            foreach (Component component in vessel.transform.GetComponentsInChildren(typeof(Transform), true))
            {
                /*if (component.name.Contains(BeKerbal.Settings.EVA_HelmetColliderComponentName) && _OriginHelmetColliderRadius == -1.0f)
                {
                    component.GetComponent<SphereCollider>().radius = _OriginHelmetColliderRadius;
                    _OriginHelmetColliderRadius = -1.0f;
                }*/

                foreach (string name in BeKerbal.Settings.EVA_HiddenComponentsName)
                {
                    if (component.name.Contains(name) && component.renderer != null)
                    {
                        component.renderer.enabled = true;
                        break;
                    }
                }
            }

            flightCam.ActivateUpdate();

            Screen.lockCursor = false;
        }

        public void Update(KerbalInfo kerbalInfo)
        {
            FlightCamera flightCam = FlightCamera.fetch;
            Vessel vessel = kerbalInfo.Vessel;
            KerbalEVA evaInst = kerbalInfo.EVA;

            if (flightCam.updateActive)
            {
                flightCam.DeactivateUpdate();
            }

            float yaw = 0.0f;
            float pitch = 0.0f;

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                _CursorLocked = !_CursorLocked;
            }

            Screen.lockCursor = _CursorLocked;
            if (_CursorLocked && !evaInst.isRagdoll)
            {
                yaw = Input.GetAxis("Mouse X") * BeKerbal.Settings.EVA_LookSentivity;
                pitch = Input.GetAxis("Mouse Y") * BeKerbal.Settings.EVA_LookSentivity;
            }

            if (!evaInst.isRagdoll)
            {
                evaInst.fFwd = _Forward;
                evaInst.fUp = _Up;
            }

            flightCam.mainCamera.nearClipPlane = BeKerbal.Settings.EVA_NearClip;
            flightCam.transform.position = vessel.ReferenceTransform.position;
            flightCam.transform.rotation = Quaternion.identity;
            
            if (!evaInst.isRagdoll)
            {
                if (vessel.LandedOrSplashed || evaInst.OnALadder) // Is walking on a surface or climbing a ladder
                {
                    evaInst.CharacterFrameMode = false;
                    evaInst.CharacterFrameModeToggle = true;

                    _Yaw += yaw;
                    float absYaw = Mathf.Abs(_Yaw);
                    float yawExceed = Mathf.Max(0.0f, Mathf.Abs(_Yaw) - BeKerbal.Settings.EVA_MaxYaw) * Mathf.Sign(_Yaw);
                    _Yaw = Mathf.Clamp(_Yaw, BeKerbal.Settings.EVA_MinYaw, BeKerbal.Settings.EVA_MaxYaw);

                    _Pitch -= pitch;
                    _Pitch = Mathf.Clamp(_Pitch, BeKerbal.Settings.EVA_MinPitch, BeKerbal.Settings.EVA_MaxPitch);

                    if (evaInst.OnALadder)
                    {
                        flightCam.transform.rotation = Quaternion.LookRotation(evaInst.transform.forward, evaInst.transform.up) * SpaceNavigator.Rotation;
                    }
                    else
                    {
                        flightCam.transform.rotation = Quaternion.LookRotation(evaInst.fFwd, evaInst.transform.up) * SpaceNavigator.Rotation;

                        if (PressedMove)
                        {
                            float partYaw = _Yaw * BeKerbal.Settings.EVA_ViewTurnPercent;
                            _Yaw -= partYaw;
                            flightCam.transform.Rotate(Vector3.up * (yawExceed + partYaw), Space.Self);
                        }
                    }

                    _Forward = flightCam.transform.forward;
                    _Up = flightCam.transform.up;
                }
                else if (evaInst.JetpackDeployed) // Is using jetpack in the air
                {
                    evaInst.CharacterFrameMode = false;
                    evaInst.CharacterFrameModeToggle = true;

                    //flightCam.transform.rotation = Quaternion.LookRotation(-_HeadTransform.up, -_HeadTransform.right) * SpaceNavigator.Rotation;
                    flightCam.transform.rotation = Quaternion.LookRotation(evaInst.fFwd, evaInst.fUp) * SpaceNavigator.Rotation;

                    if (_CursorLocked)
                    {
                        _Yaw += yaw;
                        _Yaw = Mathf.Clamp(_Yaw, BeKerbal.Settings.EVA_MinYaw, BeKerbal.Settings.EVA_MaxYaw);

                        _Pitch -= pitch;
                        _Pitch = Mathf.Clamp(_Pitch, BeKerbal.Settings.EVA_MinPitch, BeKerbal.Settings.EVA_MaxPitch);

                        if (PressedMove)
                        {
                            float partYaw = _Yaw * BeKerbal.Settings.EVA_ViewTurnPercent;
                            float partPitch = _Pitch * BeKerbal.Settings.EVA_ViewTurnPercent;
                            _Yaw -= partYaw;
                            _Pitch -= partPitch;

                            flightCam.transform.rotation *= Quaternion.Euler(-(pitch - partPitch), yaw + partYaw, 0.0f);
                        }
                    }

                    _Forward = flightCam.transform.forward;
                    _Up = flightCam.transform.up;
                }
                else // Is falling without jetpack and not in ragdoll yet (probably jumping or drifting in the void)
                {
                    evaInst.CharacterFrameMode = true;
                    evaInst.CharacterFrameModeToggle = false;

                    _Yaw += yaw;
                    _Yaw = Mathf.Clamp(_Yaw, BeKerbal.Settings.EVA_MinYaw, BeKerbal.Settings.EVA_MaxYaw);

                    _Pitch -= pitch;
                    _Pitch = Mathf.Clamp(_Pitch, BeKerbal.Settings.EVA_MinPitch, BeKerbal.Settings.EVA_MaxPitch);

                    flightCam.transform.rotation = Quaternion.LookRotation(-_HeadTransform.up, -_HeadTransform.right) * SpaceNavigator.Rotation;

                    _Forward = evaInst.transform.forward;
                }
            }
            else // Is in ragdoll or TODO(is recovering from ragdoll)
            {
                evaInst.CharacterFrameMode = true;
                evaInst.CharacterFrameModeToggle = false;

                _Yaw = Mathf.Lerp(_Yaw, 0.0f, 2.0f * Time.deltaTime);
                _Pitch = Mathf.Lerp(_Pitch, 0.0f, 2.0f * Time.deltaTime);

                if (!evaInst.animation.enabled) // true ragdoll
                {
                    flightCam.transform.rotation = Quaternion.LookRotation(-_HeadTransform.up, -_HeadTransform.right) * SpaceNavigator.Rotation;
                }
                else // recovering
                {
                    flightCam.transform.rotation = Quaternion.LookRotation(-_HeadTransform.up, -_HeadTransform.right) * SpaceNavigator.Rotation;
                    _Forward = evaInst.transform.forward;
                    _Up = evaInst.transform.up;
                }
            }

            flightCam.transform.Translate(BeKerbal.Settings.EVA_HeadLocation + BeKerbal.Settings.EVA_EyeOffset, Space.Self);

            flightCam.transform.Rotate(Vector3.up * _Yaw, Space.Self);
            flightCam.transform.Rotate(Vector3.right * _Pitch, Space.Self);
        }

        public void PausedUpdate(KerbalInfo kerbalInfo)
        {
            Screen.lockCursor = false;
        }
    }
}
