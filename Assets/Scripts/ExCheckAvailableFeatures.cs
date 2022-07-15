#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityAR
{
    public class ExCheckAvailableFeatures : MonoBehaviour
    {
        [SerializeField] Text message;
        [SerializeField] ARSession session;
        bool isReady;

        void ShowMessage(string text)
        {
            message.text = $"{text}\r\n";
        }
        void AddMessage(string text)
        {
            message.text = $"{text}\r\n";
        }
        void Awake()
        {
            if(message == null)
            {
                Application.Quit();
            }
            if(session == null)
            {
                isReady = false;
                ShowMessage("エラー:SerializeFieldの設定不備。");
            }
            else
            {
                isReady = true;
            }
        }
        bool planeDetectionSupported = false;
        bool raycastSupported = false;
        bool faceTrackingSupported = false;
        bool eyeTrackingSupported = false;
        bool faceTrackingWithWorldFacingCameraSupported = false;
        bool faceTrackingWithUserFacingCameraSupported = false;

        void CheckAvailableFeatures()
        {
            var planeDescriptors = new List<XRPlaneSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(planeDescriptors);
            planeDetectionSupported = planeDescriptors.Count > 0;

            var rayCastDescriptors = new List<XRRaycastSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(rayCastDescriptors);
            raycastSupported = rayCastDescriptors.Count > 0;

            var faceDescriptors = new List<XRFaceSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(faceDescriptors);
            if (faceDescriptors.Count > 0)
            {
                faceTrackingSupported = true;
                foreach (var faceDescriptor in faceDescriptors)
                {
                    if (faceDescriptor.supportsEyeTracking)
                    {
                        eyeTrackingSupported = true;
                        break;
                    }
                }
            }
            var configs = session.subsystem.GetConfigurationDescriptors(Unity.Collections.Allocator.Temp);
            if (configs.IsCreated)
            {
                using (configs)
                {
                    foreach (var config in configs)
                    {
                        if(config.capabilities.All(Feature.WorldFacingCamera | Feature.FaceTracking))
                        {
                            faceTrackingWithWorldFacingCameraSupported = true;
                        }
                        if (config.capabilities.All(Feature.UserFacingCamera | Feature.FaceTracking))
                        {
                            faceTrackingWithUserFacingCameraSupported = true;
                        }
                    }
                }
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            if (!isReady)
            {
                return;
            }
            CheckAvailableFeatures();
            ShowMessage("機能のサポート調査");
            AddMessage($"平面検出:{planeDetectionSupported}");
            AddMessage($"レイキャスト:{raycastSupported}");
            AddMessage($"顔検出:{faceTrackingSupported}");
            AddMessage($"視線検出:{eyeTrackingSupported}");
            AddMessage($"顔検出+WorldFacing:{faceTrackingWithWorldFacingCameraSupported}");
            AddMessage($"顔検出+UserFacing:{faceTrackingWithUserFacingCameraSupported}");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
