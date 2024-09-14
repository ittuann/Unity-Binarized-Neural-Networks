using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    public MNISTDisplay mnistDisplay;
    public LogDisplay logDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            int showNum = mnistDisplay.getCurrentShowNum() - 1;
            if (showNum < 0)
            {
                showNum = MNISTData.mnistLabel.Length - 1;
            }
            mnistDisplay.setCurrentShowNum(showNum);
            logDisplay.UpdateNumLog($"{showNum + 1} / {MNISTData.mnistLabel.Length}");

            //GameObject mnistDisplayObject = GameObject.Find("MNIST Image");
            //if (mnistDisplayObject != null)
            //{
            //    if (mnistDisplayObject.TryGetComponent<MNISTDisplay>(out MNISTDisplay mnistDisplay))
            //    {
            //        int showNum = mnistDisplay.getCurrentShowNum() - 1;
            //        if (showNum < 0)
            //        {
            //            showNum = MNISTData.mnistLabel.Length - 1;
            //        }
            //        mnistDisplay.setCurrentShowNum(showNum);
            //    }
            //}
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            int showNum = mnistDisplay.getCurrentShowNum() + 1;
            if (showNum >= MNISTData.mnistLabel.Length)
            {
                showNum = 0;
            }
            mnistDisplay.setCurrentShowNum(showNum);
            logDisplay.UpdateNumLog($"{showNum + 1} / {MNISTData.mnistLabel.Length}");
        }
    }
}
