using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAR
{
    public class FinalScore : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.GetComponent<Text>().text = ExCenterPointer2.scoreString;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
