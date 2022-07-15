#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UnityAR
{
    [RequireComponent(typeof(ARPlaneManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(PlayerInput))]

    public class ExCenterPointer2 : MonoBehaviour
    {
        // public static int destroyCount = 0;
        [SerializeField]
        Text message;
        [SerializeField]
        Text timerText;
        [SerializeField]
        Text countText;
        [SerializeField]
        GameObject placementPrefab;
        [SerializeField]
        GameObject traceObjPrefab;
        [SerializeField]
        GameObject randomPrefab;

        // https://www.youtube.com/watch?v=TP41AH8GRRk
        // https://unity-guide.moon-bear.com/get-component/
        private int hour;
        private int minute;
        private float seconds;
        private float oldSeconds;

        private int getCount;
        public static string scoreString;

        ARPlaneManager planeManager;
        ARRaycastManager raycastManager;
        PlayerInput playerInput;
        bool isReady;
        float speedParameter = 0.005f;
        Vector3 newHitPose;


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
                // ShowMessage("平面検出");
                // AddMessage("床を撮影してください。しばらくすると平面が検出されます。平面にポインタが現れます。");
            }
        }
        GameObject instantiatedObject = null;
        GameObject traceObject = null;
        GameObject instantiatedRandomObject;

        // Start is called before the first frame update
        void Start()
        {
            hour = 0;
            minute = 0;
            seconds = 0f;
            oldSeconds = 0f;
        }

        // Update is called once per frame
        private void Update()
        {
            
        }

        // 一時停止を使いたいのでFixedUpdateを使う。
        private void FixedUpdate()
        {
            // https://tech.pjin.jp/blog/2021/09/29/unity-time_deltatime/
            seconds += Time.deltaTime;
            if (seconds >= 60f)
            {
                minute++;
                seconds = seconds - 60f;
            }

            if (minute >= 60f)
            {
                hour++;
                minute = minute - 60;
            }

            if ((int)seconds != (int)oldSeconds) { }
            {
                timerText.text = hour.ToString("00") + ":" + minute.ToString("00") + ":" + ((int)seconds).ToString("00");
            }
            oldSeconds = seconds;

            // https://unitygeek.hatenablog.com/entry/2017/04/15/143053
            // randomPrefabのdestroyCountをgetで取得。
            getCount = randomPrefab.GetComponent<DestroyChair>().Count;
            countText.text = getCount.ToString();
            if (getCount == 10)
            {
                // https://qiita.com/yokotate/items/adcb307fd4635246e441
                Time.timeScale = 0f;
                scoreString = hour.ToString("00") + ":" + minute.ToString("00") + ":" + ((int)seconds).ToString("00");
                SceneManager.LoadScene("EndCardCenterpointer2");
            }

            // https://docs.unity3d.com/ja/current/ScriptReference/Camera.ViewportToScreenPoint.html
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

                    // https://code.hildsoft.com/entry/2017/07/06/060658
                    var aim = instantiatedObject.transform.position - traceObject.transform.position;
                    var look = Quaternion.LookRotation(new Vector3(aim.x, aim.y, aim.z));
                    traceObject.transform.localRotation = look;
                }
                else
                {
                    instantiatedObject.transform.position = hitPose.position;
                    if ((instantiatedObject.transform.position - traceObject.transform.position).magnitude < 0.015f)
                    {

                    }
                    else
                    {
                        // https://code.hildsoft.com/entry/2017/07/06/060658
                        var aim = instantiatedObject.transform.position - traceObject.transform.position;
                        var look = Quaternion.LookRotation(new Vector3(aim.x, aim.y, aim.z));
                        traceObject.transform.localRotation = look;

                        // https://qiita.com/OKsaiyowa/items/167a0a6afa536c33fc38
                        traceObject.transform.position = Vector3.MoveTowards(traceObject.transform.position, instantiatedObject.transform.position, speedParameter);
                    }
                }
            }

            // https://dianxnao.com/unity%EF%BC%9A%E3%83%97%E3%83%AC%E3%83%8F%E3%83%96%E3%82%92%E3%83%A9%E3%83%B3%E3%83%80%E3%83%A0%E3%81%AA%E4%BD%8D%E7%BD%AE%E3%81%AB%E7%94%9F%E6%88%90%E3%81%95%E3%81%9B%E3%82%8B/
            int randomInterval = Random.Range(100, 200);
            float x = 0.0f;
            float z = 0.0f;
            float adjuster = 0.2f;
            if (Time.frameCount % randomInterval == 0)
            {
                float randomF = Random.Range(0.2f, 0.8f);
                if (0.4f <= randomF || randomF <= 0.6f)
                {
                    if (0.5f <= randomF)
                    {
                        x = randomF + adjuster;
                        z = randomF + adjuster;
                    }
                    else
                    {
                        x = randomF - adjuster;
                        z = randomF - adjuster;
                    }
                }

                var randomRayPointer = Camera.main.ViewportToScreenPoint(new Vector3(x, z));
                if (raycastManager.Raycast(randomRayPointer, hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    hitPose.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                    // hitPose.position = new Vector3(hitPose.position.x, hitPose.position.y + 1.0f, hitPose.position.z);
                    Instantiate(randomPrefab, hitPose.position, hitPose.rotation);
                }
            }

        }
    }
}
