using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogDisplay : MonoBehaviour
{
    public TextMeshProUGUI logTextCSharp;
    public TextMeshProUGUI logTextComputeShader;
    public TextMeshProUGUI logTextNum;

    private string currentCSharpLog = "";
    private string currentComputeShaderLog = "";
    private string currentNumLog = $"1 / {MNISTData.mnistLabel.Length}";

    // Start is called before the first frame update
    void Start()
    {
        logTextCSharp.text = "CSharp Output Log:\n";
        logTextComputeShader.text = "ComputeShader Output Log:\n";
        logTextNum.text = currentNumLog;
    }

    public void UpdateCSharpLog(string outputs)
    {
        currentCSharpLog = outputs;
    }
    public void UpdateComputeShaderLog(string outputs)
    {
        currentComputeShaderLog = outputs;
    }
    public void UpdateNumLog(string outputs)
    {
        currentNumLog = outputs;
    }

    // Update is called once per frame
    void Update()
    {
        logTextCSharp.text = "CSharp Output Log:\n" + currentCSharpLog;
        logTextComputeShader.text = "ComputeShader Output Log:\n" + currentComputeShaderLog;
        logTextNum.text = currentNumLog;
    }
}
