#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

namespace UnityAR
{
    [RequireComponent(typeof(ARPlaneManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(PlayerInput))]

    public class ExPlaneDetection : MonoBehaviour
    {
        [SerializeField]
        Text message;
        [SerializeField]
        GameObject placementPrefab;

        ARPlaneManager planeManager;
        ARRaycastManager raycastManager;
        PlayerInput playerInput;
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
            planeManager = GetComponent<ARPlaneManager>();
            playerInput = GetComponent<PlayerInput>();
            raycastManager = GetComponent<ARRaycastManager>();
            if (placementPrefab == null ||
                planeManager == null ||
                planeManager.planePrefab == null ||
                raycastManager == null ||
                playerInput == null ||
                playerInput.actions == null)
            {
                isReady = false;
                ShowMessage("エラー：SerializeFieldなどの設定不備");
            }
            else
            {
                isReady = true;
                ShowMessage("平面検出");
                AddMessage("床を撮影してください。しばらくすると平面が検出されます。平面をタップすると椅子が表示されます。");
            }
        }
        GameObject instantiatedObject = null;
        void OnTouch(InputValue touchInfo)
        {
            if (!isReady)
            {
                return;
            }
            var touchPosition = touchInfo.Get<Vector2>();
            var hits = new List<ARRaycastHit>();
            if(raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                if(instantiatedObject == null)
                {
                    instantiatedObject = Instantiate(placementPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    instantiatedObject.transform.position = hitPose.position;
                }
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
