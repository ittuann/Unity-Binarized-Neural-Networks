using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MNISTDisplay : MonoBehaviour
{
    // 声明一个Texture2D对象，用于显示MNIST图像
    public Texture2D texture;
    [SerializeField]
    public int MNISTCurrentShowNum = 0;

    private void LoadMNISTData(int showNum)
    {
        byte[] mnistData = MNISTData.GetMNISTNumData(showNum, showNum);

        for (int i = 0; i < mnistData.Length; i++)
        {
            // 将像素值从0-255规范化到0-1之间，用于颜色表示
            float value = mnistData[i] / 255.0f;

            // 将MNIST图像的像素值填充到Texture2D中
            // 设置纹理上对应像素的颜色。纹理坐标需要将一维数组转换为二维坐标（x, y）
            texture.SetPixel(i % 28, 27 - i / 28, new Color(value, value, value));
        }

        // 应用所有像素的修改到纹理上
        texture.Apply();
    }

    public int getCurrentShowNum()
    {
        return MNISTCurrentShowNum;
    }
    public int setCurrentShowNum(int num)
    {
        MNISTCurrentShowNum = num;
        return MNISTCurrentShowNum;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初始化Texture2D，尺寸为MNIST图像的28x28像素
        texture = new Texture2D(28, 28);

        // 创建一个Sprite，将Texture2D作为材质。并设置其位置和中心点
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // 加载MNIST数据
        LoadMNISTData(MNISTCurrentShowNum);
    }

    // Update is called once per frame
    void Update()
    {
        LoadMNISTData(MNISTCurrentShowNum);
    }
}
