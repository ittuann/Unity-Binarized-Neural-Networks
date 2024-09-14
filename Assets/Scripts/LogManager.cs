using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputRecord
{
    public DateTime Time { get; set; }
    public string Output { get; set; }

    public override string ToString()
    {
        return $"Output: {Output}\nTime: {Time}, ";
    }
}

public class LogManager
{
    // 输出历史字典，键类型为int（图片ID），值为OutputRecord
    private Dictionary<int, OutputRecord> outputRecords = new Dictionary<int, OutputRecord>();

    public void AddRecord(int imgId, string log, DateTime time)
    {
        outputRecords[imgId] = new OutputRecord { Time = time, Output = log };
    }

    public string GetRecordString(int imgId)
    {
        if (outputRecords.TryGetValue(imgId, out OutputRecord record))
        {
            return record.ToString();
        }
        else
        {
            return "No Output";  //throw new KeyNotFoundException("Record not found.");
        }
    }
}
