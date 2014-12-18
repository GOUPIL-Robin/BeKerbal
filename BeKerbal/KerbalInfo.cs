using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace BeKerbal
{
    public class KerbalInfo
    {
        public Kerbal Kerbal;
        public Vessel Vessel;
        public KerbalEVA EVA;
        public bool IsEVA;

        public KerbalInfo(Vessel vessel)
        {
            Vessel = vessel;
            EVA = Vessel.evaController;
            IsEVA = Vessel.isEVA;
            Kerbal = null;
            if (!IsEVA)
            {
                // We can use reflection and extract the current kerbal from the CameraManager
                CameraManager cameraManager = CameraManager.Instance;
                FieldInfo[] fields = cameraManager.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].FieldType == typeof(Kerbal))
                    {
                        Kerbal = fields[i].GetValue(cameraManager) as Kerbal;
                        break;
                    }
                }

                // If we haven't started the IVA yet, the CameraManager will not know who is the current kerbal, take the first crew by default if possible
                if (Kerbal == null && Vessel.GetCrewCount() > 0)
                {
                    Kerbal = Vessel.GetVesselCrew()[0].KerbalRef;
                }
            }
            else
            {
                // In EVA the only crew is the current Kerbal, but I can't get the Kerbal (it is null), why?
                Kerbal = Vessel.GetVesselCrew()[0].KerbalRef;
                Debug.Log("EVA CREW (" + Vessel.GetVesselCrew().Count + "):");
                foreach (ProtoCrewMember ptc in Vessel.GetVesselCrew())
                {
                    if (ptc.KerbalRef != null)
                    {
                        Debug.Log("-- " + ptc.KerbalRef.crewMemberName);
                        Kerbal = ptc.KerbalRef;
                    }
                    else
                    {
                        Debug.Log("-- NULL");
                    }
                }
            }
        }

        public KerbalInfo(KerbalInfo other)
        {
            EVA = other.EVA;
            IsEVA = other.IsEVA;
            Kerbal = other.Kerbal;
            Vessel = other.Vessel;
        }
    }
}
