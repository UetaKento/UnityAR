#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityAR
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ExImageTracking : MonoBehaviour
    {
        [SerializeField] Text message;
        [SerializeField] List<GameObject> placementPrefabs;
        ARTrackedImageManager imageManager;
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
            imageManager = GetComponent<ARTrackedImageManager>();
            if (imageManager == null || imageManager.referenceLibrary == null || imageManager.referenceLibrary.count != placementPrefabs.Count)
            {
                isReady = false;
                ShowMessage("エラー：SerializeFieldなどの設定不備。");
            }
            else
            {
                isReady = true;
            }
        }
        Dictionary<string, GameObject> correspondingChartForMarkersAndPrefabs = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();

        void OnEnable()
        {
            if (!isReady)
            {
                return;
            }
            var markerList = new List<string>();
            for (var i = 0; i < imageManager.referenceLibrary.count; i++)
            {
                markerList.Add(imageManager.referenceLibrary[i].name);
            }
            markerList.Sort();
            for (var i = 0; i < placementPrefabs.Count; i++)
            {
                correspondingChartForMarkersAndPrefabs.Add(markerList[i], placementPrefabs[i]);
                instantiatedObjects.Add(markerList[i], null);
            }
            imageManager.trackedImagePrefab = null;
            imageManager.trackedImagesChanged += OnTrackedImageChanged;

            ShowMessage("ARマーカーとプレハブの対応");
            foreach(var data in correspondingChartForMarkersAndPrefabs)
            {
                AddMessage($"{data.Key}: {data.Value}");
            }
            AddMessage("ARマーカーと配置するプレハブの対応を確認後、ARマーカーを撮影してください。");

        }

        void OnDisable()
        {
            if (!isReady)
            {
                return;
            }
            imageManager.trackedImagesChanged -= OnTrackedImageChanged;
        }

        void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            ShowMessage("イメージ検出");
            foreach(var trackedImage in eventArgs.added)
            {
                var imageName = trackedImage.referenceImage.name;
                if (correspondingChartForMarkersAndPrefabs.TryGetValue(imageName, out var prefab))
                {
                    var scale = 0.2f;
                    trackedImage.transform.localScale = Vector3.one * scale;
                    instantiatedObjects[imageName] = Instantiate(prefab, trackedImage.transform);
                }
            }
            foreach (var trackedImage in eventArgs.updated)
            {
                var imageName = trackedImage.referenceImage.name;
                if(instantiatedObjects.TryGetValue(imageName,out var instantiatedObject))
                {
                    if (trackedImage.trackingState != TrackingState.None)
                    {
                        instantiatedObject.SetActive(true);
                    }
                    else
                    {
                        instantiatedObject.SetActive(false);
                    }
                    AddMessage($"{imageName}: {trackedImage.trackingState}");
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
