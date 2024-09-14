using System;

public class BNNImplement
{
    /**
     * 二值化全连接层推理 - uint8输入
     * @inputs 指向输入数据数组的指针。使用一维数组模拟二维矩阵
     *
     * 输入数据为指向uint8类型数据数组的指针
     * 使用int8类型权重进行全连接层的内积整形乘法计算
     * 输出类型为float(因为不确定是否存在偏置项)
     *
     * 优化方向: 模型结构确定后，可以将输入数据类型根据情况修改为int，减少数据类型转换
     */
    public static void BinaryFullyConnectedInferenceUInt8(byte[] inputs, int batchSize, int inputDim, int outputDim, sbyte[] weights, float[] bias, float[] output)
    {
        for (int i = 0; i < batchSize; i++)
        {
            // 遍历批次内的每一个样本
            for (int j = 0; j < outputDim; j++)
            {
                // 遍历输出的每一个维度
                int sumInt = 0;
                for (int k = 0; k < inputDim; k++)
                {
                    sumInt += inputs[i * inputDim + k] * weights[k * outputDim + j];
                }
                // 处理浮点数偏置项
                if (bias != null)
                {
                    output[i * outputDim + j] = sumInt + bias[j];  // 隐式类型转换
                }
                else
                {
                    output[i * outputDim + j] = sumInt;
                }
            }
        }
    }

    /**
     * 批量归一化层推理
     *
     * 使用标准差std代替方差var，减少开方运算
     */
    public static void BatchNormalizationInferenceStd(float[] inputs, int batch_size, int dim,
                                                      float[] gamma, float[] beta, float[] mean, float[] std,
                                                      float[] outputs)
    {
        for (int i = 0; i < batch_size; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                // 计算归一化值
                float normalized = (inputs[i * dim + j] - mean[j]) / std[j];
                // 计算输出
                outputs[i * dim + j] = gamma[j] * normalized + beta[j];
            }
        }
    }

    /**
     * Softmax 层推理
     */
    public static void SoftmaxInference(float[] inputs, int batch_size, int classes, float[] outputs)
    {
        for (int i = 0; i < batch_size; i++)
        {
            // 找到每个样本的最大值
            float maxInput = float.MinValue;
            for (int j = 0; j < classes; j++)
            {
                if (inputs[i * classes + j] > maxInput)
                {
                    maxInput = inputs[i * classes + j];
                }
            }

            // 计算指数值，并累加得到指数和
            float sum = 0.0f;
            for (int j = 0; j < classes; j++)
            {
                outputs[i * classes + j] = (float)Math.Exp(inputs[i * classes + j] - maxInput);
                sum += outputs[i * classes + j];
            }

            // 归一化
            for (int j = 0; j < classes; j++)
            {
                outputs[i * classes + j] /= sum;
            }
        }
    }

    /**
     * 从Softmax层推理输出的结果中，输出最大概率的索引
     */
    public static void MaxSoftmaxInference(float[] inputs, int batch_size, int classes, int[] outputs)
    {
        for (int i = 0; i < batch_size; i++)
        {
            float maxProb = float.MinValue;
            int maxIndex = 0;
            for (int j = 0; j < classes; j++)
            {
                if (inputs[i * classes + j] > maxProb)
                {
                    maxProb = inputs[i * classes + j];
                    maxIndex = j;
                }
            }
            outputs[i] = maxIndex;  // 输出每个样本的最大概率值的索引
        }
    }
}

public class BNNInference
{
    private const int inputDim = 784;
    private const int outputDim = 10;
    private const int batchSize = 1;

    // 中间层输出
    private float[] fc_output = new float[batchSize * outputDim];
    private float[] bn_output = new float[batchSize * outputDim];
    private float[] softmax_output = new float[batchSize * outputDim];
    private int[] max_softmax_output = new int[batchSize];

    public int[] PerformInference(byte[] inputs)
    {
        // 二值化全连接层推理
        BNNImplement.BinaryFullyConnectedInferenceUInt8(inputs, batchSize, inputDim, outputDim, BNNModelData.binarize_fc1_w, BNNModelData.binarize_fc1_b, fc_output);
        // 批量归一化推理
        BNNImplement.BatchNormalizationInferenceStd(fc_output, batchSize, outputDim, BNNModelData.binarize_fc1_bn_gamma, BNNModelData.binarize_fc1_bn_beta, BNNModelData.binarize_fc1_bn_mean, BNNModelData.binarize_fc1_bn_std, bn_output);
        // Softmax 推理
        BNNImplement.SoftmaxInference(bn_output, batchSize, outputDim, softmax_output);
        // Softmax 最大概率索引
        BNNImplement.MaxSoftmaxInference(softmax_output, batchSize, outputDim, max_softmax_output);

        return max_softmax_output;
    }
}
