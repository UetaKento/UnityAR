#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityAR
{
    [RequireComponent(typeof(ARFaceManager))]
    public class ExFaceTrackingPose : MonoBehaviour
    {
        [SerializeField] Text message;
        [SerializeField] ARCameraManager cameraManager;
        ARFaceManager faceManager;
        bool isReady;

        void ShowMessage(string text)
        {
            message.text = $"{text}\r\n";
        }
        void AddMessage(string text)
        {
            message.text += $"{text}\r\n";
        }
        void Awake()
        {
            if (message == null)
            {
                Application.Quit();
            }
            faceManager = GetComponent<ARFaceManager>();
            if (cameraManager == null || faceManager == null || faceManager.facePrefab == null)
            {
                isReady = false;
                ShowMessage("エラー：SerializeFieldなどの設定不備。");
            }
            else
            {
                isReady = true;
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!isReady)
            {
                return;
            }

            //ShowMessage("顔検出(Pose)");
            //AddMessage($"Poseサポート:{faceManager.descriptor.supportsFacePose}");
            //AddMessage($"現在のカメラの向き:{cameraManager.currentFacingDirection}");
            //AddMessage($"サポートされている顔検出数:{faceManager.supportedFaceCount}");
            //AddMessage($"現在検出できる顔の最大数:{faceManager.currentMaximumFaceCount}");
            //AddMessage($"現在検出している顔の数:{faceManager.trackables.count}");
        }
    }
}
