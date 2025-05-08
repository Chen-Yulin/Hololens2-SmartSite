using UnityEngine;
using System.IO;
using System.Text;
using System;

public class fpsRecorder : MonoBehaviour
{
    private float lastTime = 0f;
    private string filePath;

    void Start()
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        filePath = Path.Combine(desktopPath, "FrameIntervals.txt");
        // 如果文件不存在就创建，并添加表头
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Time(s),Interval(s)\n", Encoding.UTF8);
        }

        lastTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        float currentTime = Time.realtimeSinceStartup;
        float interval = currentTime - lastTime;
        lastTime = currentTime;

        string log = $"{currentTime:F4},{interval:F6}\n";
        File.AppendAllText(filePath, log, Encoding.UTF8);
    }
}
