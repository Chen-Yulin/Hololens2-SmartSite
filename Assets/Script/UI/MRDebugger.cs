using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class MRDebugger : MonoBehaviour
{
    public ToolTip logTextMeshPro;  // Drag your TextMeshPro UI object here in the inspector
    public float logDisplayDuration = 1f;   // 每行日志显示时间（秒）
    private Queue<string> logMessages = new Queue<string>();  // 用于存储日志的队列
    private float timer = 0f;
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        logDisplayDuration = 5f / (logMessages.Count+1f);
        timer += Time.deltaTime;
        if (timer >= logDisplayDuration)
        {
            timer = 0f;
            if (logMessages.Count > 0)
            {
                logMessages.Dequeue();  // 删除最旧的一条日志
                UpdateLogText();  // 更新显示的日志信息
            }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 检查新的日志是否与最后一条日志相同
        if (logMessages.Count == 0 || logMessages.Peek() != logString)
        {
            logMessages.Enqueue(logString);  // 添加新日志到队列
            UpdateLogText();  // 更新显示的日志信息
        }
    }

    void UpdateLogText()
    {
        logTextMeshPro.ToolTipText = string.Join("\n", logMessages.ToArray());
    }
}
