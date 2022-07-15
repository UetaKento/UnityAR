#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

namespace UnityAR
{
    // https://ekulabo.com/require-component
    [RequireComponent(typeof(ARSession))]
    [RequireComponent(typeof(ARPlaneManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(PlayerInput))]
    public class ExMakeAppearOnPlane : MonoBehaviour
    {
        ARSessionOrigin sessionOrigin;
        ARPlaneManager planeManager;
        ARRaycastManager raycastManager;
        PlayerInput playerInput;
        [SerializeField] GameObject placementPrefab;
        GameObject instantiatedObject = null;
        float scale;
        public float Scale
        {
            // https://unitygeek.hatenablog.com/entry/2017/04/15/143053
            get { return scale; }
            set
            {
                scale = value;
                if(sessionOrigin != null && instantiatedObject != null)
                {
                    sessionOrigin.transform.localScale = Vector3.one / scale;
                }
            }
        }
        Quaternion rotation;
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                if(sessionOrigin != null && instantiatedObject != null)
                {
                    sessionOrigin.MakeContentAppearAt(instantiatedObject.transform, instantiatedObject.transform.position, rotation);
                }
            }
        }
        public bool IsAvailable
        {
            get;
            private set;
        }

        // https://dkrevel.com/unity-explain/how-to-call-start-awake-onenable/
        void Awake()
        {
            sessionOrigin = GetComponent<ARSessionOrigin>();
            planeManager = GetComponent<ARPlaneManager>();
            raycastManager = GetComponent<ARRaycastManager>();
            playerInput = GetComponent<PlayerInput>();
            if (sessionOrigin == null || sessionOrigin.camera == null ||
                planeManager == null || planeManager.planePrefab == null ||
                raycastManager == null || playerInput == null ||
                playerInput.actions == null || placementPrefab == null)
            {
                IsAvailable = false;
            }
            else
            {
                IsAvailable = true;
            }
        }
        void OnTouch(InputValue touchInfo)
        {
            if (!IsAvailable)
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
                    sessionOrigin.MakeContentAppearAt(instantiatedObject.transform, hitPose.position, rotation);
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
            if(instantiatedObject == null)
            {
                return;
            }
            // https://mogi0506.com/unity-foreach/
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}
