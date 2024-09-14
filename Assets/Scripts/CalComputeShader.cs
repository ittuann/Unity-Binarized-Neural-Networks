using UnityEngine;

public class CalComputeShader : MonoBehaviour
{
    public ComputeShader shader;
    private ComputeBuffer weightBuffer, biasBuffer, gammaBuffer, betaBuffer, meanBuffer, stdBuffer;
    private ComputeBuffer inputBuffer;
    private ComputeBuffer fcOutputBuffer, bnOutputBuffer, softmaxOutputBuffer, softmaxMaxOutputBuffer;

    public float aveElapsedTime;
    public int[] bnnInferenceResult;
    public LogManager logManager = new LogManager();

    private void InitializeComputeBuffers()
    {
        float[] float_binarize_fc1_w = new float[BNNModelData.binarize_fc1_w.Length];
        for (int i = 0; i < BNNModelData.binarize_fc1_w.Length; i++)
        {
            float_binarize_fc1_w[i] = (float)BNNModelData.binarize_fc1_w[i];
        }
        weightBuffer = new ComputeBuffer(float_binarize_fc1_w.Length, sizeof(float));
        weightBuffer.SetData(float_binarize_fc1_w);

        biasBuffer = new ComputeBuffer(BNNModelData.binarize_fc1_b.Length, sizeof(float));
        biasBuffer.SetData(BNNModelData.binarize_fc1_b);

        gammaBuffer = new ComputeBuffer(BNNModelData.binarize_fc1_bn_gamma.Length, sizeof(float));
        gammaBuffer.SetData(BNNModelData.binarize_fc1_bn_gamma);

        betaBuffer = new ComputeBuffer(BNNModelData.binarize_fc1_bn_beta.Length, sizeof(float));
        betaBuffer.SetData(BNNModelData.binarize_fc1_bn_beta);

        meanBuffer = new ComputeBuffer(BNNModelData.binarize_fc1_bn_mean.Length, sizeof(float));
        meanBuffer.SetData(BNNModelData.binarize_fc1_bn_mean);

        stdBuffer = new ComputeBuffer(BNNModelData.binarize_fc1_bn_std.Length, sizeof(float));
        stdBuffer.SetData(BNNModelData.binarize_fc1_bn_std);

        int kernelIndex = shader.FindKernel("BinaryFullyConnected");
        shader.SetBuffer(kernelIndex, "WeightBuffer", weightBuffer);
        shader.SetBuffer(kernelIndex, "BiasBuffer", biasBuffer);
        kernelIndex = shader.FindKernel("BatchNormalization");
        shader.SetBuffer(kernelIndex, "GammaBuffer", gammaBuffer);
        shader.SetBuffer(kernelIndex, "BetaBuffer", betaBuffer);
        shader.SetBuffer(kernelIndex, "MeanBuffer", meanBuffer);
        shader.SetBuffer(kernelIndex, "StdBuffer", stdBuffer);

        shader.SetInt("InputDim", 784);
        shader.SetInt("OutputDim", 10);
        shader.SetInt("BatchSize", 1);
        shader.SetInt("ClassesNum", 10);
    }

    public void Inference(byte[] mnistData, byte[] mnistLabel)
    {
        InitializeComputeBuffers();

        // 初始化Compute Buffer网络输入
        float[] floatmnistData = new float[mnistData.Length];
        for (int i = 0; i < mnistData.Length; i++)
        {
            floatmnistData[i] = (float)mnistData[i];
        }
        inputBuffer = new ComputeBuffer(floatmnistData.Length, sizeof(float));
        inputBuffer.SetData(floatmnistData);
        shader.SetBuffer(shader.FindKernel("BinaryFullyConnected"), "InputBuffer", inputBuffer);

        const int batchSize = 1;
        const int outputDim = 10;

        // 初始化Compute Buffer网络中间层输出
        float[] fcOutput = new float[batchSize * outputDim];
        float[] bnOutput = new float[batchSize * outputDim];
        float[] softmaxOutput = new float[batchSize * outputDim];
        int[] softmaxMaxOutput = new int[batchSize];

        fcOutputBuffer = new ComputeBuffer(fcOutput.Length, sizeof(float));
        bnOutputBuffer = new ComputeBuffer(bnOutput.Length, sizeof(float));
        softmaxOutputBuffer = new ComputeBuffer(softmaxOutput.Length, sizeof(float));
        softmaxMaxOutputBuffer = new ComputeBuffer(softmaxMaxOutput.Length, sizeof(int));

        shader.SetBuffer(shader.FindKernel("BinaryFullyConnected"), "FCOutputBuffer", fcOutputBuffer);
        shader.SetBuffer(shader.FindKernel("BatchNormalization"), "FCOutputBuffer", fcOutputBuffer);
        shader.SetBuffer(shader.FindKernel("BatchNormalization"), "BNOutputBuffer", bnOutputBuffer);
        shader.SetBuffer(shader.FindKernel("Softmax"), "BNOutputBuffer", bnOutputBuffer);
        shader.SetBuffer(shader.FindKernel("Softmax"), "SMOutputBuffer", softmaxOutputBuffer);
        shader.SetBuffer(shader.FindKernel("SoftmaxMax"), "SMOutputBuffer", softmaxOutputBuffer);
        shader.SetBuffer(shader.FindKernel("SoftmaxMax"), "SMMaxOutputBuffer", softmaxMaxOutputBuffer);

        // 调用Compute Shader
        shader.Dispatch(shader.FindKernel("BinaryFullyConnected"), batchSize, 1, 1);
        fcOutputBuffer.GetData(fcOutput);
        shader.Dispatch(shader.FindKernel("BatchNormalization"), batchSize, 1, 1);
        bnOutputBuffer.GetData(bnOutput);
        shader.Dispatch(shader.FindKernel("Softmax"), batchSize, 1, 1);
        softmaxOutputBuffer.GetData(softmaxOutput);
        shader.Dispatch(shader.FindKernel("SoftmaxMax"), batchSize, 1, 1);
        softmaxMaxOutputBuffer.GetData(softmaxMaxOutput);

        bnnInferenceResult = softmaxMaxOutput;

        // 控制台输出BNN推理结果
        for (int i = 0; i < softmaxMaxOutput.Length; i++)
        {
            if (softmaxMaxOutput[i] == mnistLabel[i])
            {
                Debug.Log($"(Correct) Computer Shader Cla: Sample {i + 1} - Predicted = {softmaxMaxOutput[i]}, Actual = {mnistLabel[i]}");

                // 记录推理结果
                logManager.AddRecord(0, $"<color=green>(Correct) Sample {i + 1}:  Predicted = {bnnInferenceResult[i]}, Actual = {mnistLabel[i]}</color>", System.DateTime.Now);
            }
            else
            {
                Debug.Log($"(Incorrect) Computer Shader Cla: Sample {i + 1} -  Predicted = {softmaxMaxOutput[i]}, Actual = {mnistLabel[i]}");

                logManager.AddRecord(0, $"<color=red>(Incorrect) Sample {i + 1}:  Predicted = {bnnInferenceResult[i]}, Actual = {mnistLabel[i]}</color>", System.DateTime.Now);
            }
        }

        // 释放Compute Buffer
        weightBuffer.Release();
        biasBuffer.Release();
        gammaBuffer.Release();
        betaBuffer.Release();
        meanBuffer.Release();
        stdBuffer.Release();

        inputBuffer.Release();
        fcOutputBuffer.Release();
        bnOutputBuffer.Release();
        softmaxOutputBuffer.Release();
        softmaxMaxOutputBuffer.Release();
    }
}
