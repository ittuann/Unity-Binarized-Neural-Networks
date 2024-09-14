using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public MNISTDisplay mnistDisplay;
    public LogDisplay logDisplay;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    public void OnButtonCSharpClick()
    {
        int showNum = mnistDisplay.getCurrentShowNum();
        byte[] mnistData = MNISTData.GetMNISTNumData(showNum, showNum);
        byte[] mnistLabel = MNISTData.GetMNISTNumLabel(showNum, showNum);

        CalCSharp calculatorCSharp = new CalCSharp();
        calculatorCSharp.Inference(mnistData, mnistLabel);

        // 更新日志显示
        logDisplay.UpdateCSharpLog(calculatorCSharp.logManager.GetRecordString(0));
        //GameObject logDisplayObject = GameObject.Find("LogTextHandler");
        //if (logDisplayObject != null)
        //{
        //    if (logDisplayObject.TryGetComponent<LogDisplay>(out LogDisplay logDisplay))
        //    {
        //        logDisplay.UpdateCSharpLog(calculatorCSharp.logManager.GetRecordString(0));
        //    }
        //}
    }

    public void OnButtonComputeShader()
    {
        int showNum = mnistDisplay.getCurrentShowNum();
        byte[] mnistData = MNISTData.GetMNISTNumData(showNum, showNum);
        byte[] mnistLabel = MNISTData.GetMNISTNumLabel(showNum, showNum);

        // 在当前 GameObject 上查找 CalComputeShader 组件
        if (TryGetComponent<CalComputeShader>(out CalComputeShader calculatorCS))
        {
            calculatorCS.Inference(mnistData, mnistLabel);
        }

        // 更新日志显示
        logDisplay.UpdateComputeShaderLog(calculatorCS.logManager.GetRecordString(0));
    }
}
