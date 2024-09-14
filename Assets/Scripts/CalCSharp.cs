using UnityEngine;

public class CalCSharp
{
    // 创建BNNInference类的实例
    private BNNInference bnnInference = new BNNInference();
    public float aveElapsedTime;
    public int[] bnnInferenceResult;
    public LogManager logManager = new LogManager();

    public void Inference(byte[] mnistData, byte[] mnistLabel)
    {
        // 获取BNN推理开始时间
        float startTime = Time.realtimeSinceStartup;

        bnnInferenceResult = bnnInference.PerformInference(mnistData);

        // 获取BNN推理结束时间
        float endTime = Time.realtimeSinceStartup;
        aveElapsedTime = (endTime - startTime) / bnnInferenceResult.Length;

        // 控制台输出BNN推理结果
        for (int i = 0; i < bnnInferenceResult.Length; i++)
        {
            if (bnnInferenceResult[i] == mnistLabel[i])
            {
                Debug.Log($"(Correct) C Sharp Cla: Sample {i + 1} -  Predicted = {bnnInferenceResult[i]}, Actual = {mnistLabel[i]}");

                // 记录推理结果
                logManager.AddRecord(0, $"<color=green>(Correct) Sample {i + 1}:  Predicted = {bnnInferenceResult[i]}, Actual = {mnistLabel[i]}</color>" + $"\nAve time: {aveElapsedTime * 1e3:N6} ms", System.DateTime.Now);
            }
            else
            {
                Debug.Log($"(Incorrect)  C Sharp Cla: Sample {i + 1} - Predicted = {bnnInferenceResult[i]}, Actual = {mnistLabel[i]}");

                logManager.AddRecord(0, $"<color=red>(Incorrect) Sample {i + 1}:  Predicted = {bnnInferenceResult[i]}, Actual = {mnistLabel[i]}</color>" + $"\nAve time: {aveElapsedTime * 1e3:N6} ms", System.DateTime.Now);
            }
        }
        Debug.Log($"C Sharp Ave time: {aveElapsedTime * 1e3:N6} ms");
    }
}
