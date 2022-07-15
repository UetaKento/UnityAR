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

    public class ExCenterPointer : MonoBehaviour
    {
        [SerializeField]
        Text message;
        [SerializeField]
        GameObject placementPrefab;
        [SerializeField]
        GameObject traceObjPrefab;

        ARPlaneManager planeManager;
        ARRaycastManager raycastManager;
        PlayerInput playerInput;
        bool isReady;
        float speedParameter = 0.01f;

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
                AddMessage("床を撮影してください。しばらくすると平面が検出されます。平面にポインタが現れます。");
            }
        }
        GameObject instantiatedObject = null;
        GameObject traceObject = null;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                if (instantiatedObject == null)
                {
                    traceObject = Instantiate(traceObjPrefab, hitPose.position, hitPose.rotation);
                    hitPose.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                    instantiatedObject = Instantiate(placementPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    instantiatedObject.transform.position = hitPose.position;
                    // https://qiita.com/Unity_mametarou/items/8527754a52bdae2221a0
                    Vector3 dir = (instantiatedObject.transform.position - traceObject.transform.position).normalized;

                    // https://code.hildsoft.com/entry/2017/07/06/060658
                    var aim = instantiatedObject.transform.position - traceObject.transform.position;
                    var look = Quaternion.LookRotation(new Vector3(-aim.x, aim.y, -aim.z));
                    traceObject.transform.localRotation = look;

                    // https://qiita.com/OKsaiyowa/items/167a0a6afa536c33fc38
                    traceObject.transform.position = Vector3.MoveTowards(traceObject.transform.position, instantiatedObject.transform.position, speedParameter);

                }
            }
        }
    }
}
