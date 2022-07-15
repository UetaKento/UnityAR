using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAR
{

    public class DestroyChair : MonoBehaviour
    {
        private static int destroyCount = 0;

        // https://unitygeek.hatenablog.com/entry/2017/04/15/143053
        public int Count
        {
            get { return destroyCount; }
        }
        // Start is called before the first frame update

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            this.transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime, Space.World);
        }

        // https://programming.sincoston.com/unity-hit-destroy/
        //void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.tag == "Player")
        //    {
        //        Destroy(this.gameObject);
        //        ExCenterPointer2.AddCount();
        //    }
        //}

        // https://xr-hub.com/archives/7499
        public void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                Destroy(this.gameObject);
                destroyCount += 1;
            }
        }
    }
}
