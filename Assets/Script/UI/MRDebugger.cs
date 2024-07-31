using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class MRDebugger : MonoBehaviour
{
    public ToolTip logTextMeshPro;  // Drag your TextMeshPro UI object here in the inspector
    public float logDisplayDuration = 1f;   // ÿ����־��ʾʱ�䣨�룩
    private Queue<string> logMessages = new Queue<string>();  // ���ڴ洢��־�Ķ���
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
                logMessages.Dequeue();  // ɾ����ɵ�һ����־
                UpdateLogText();  // ������ʾ����־��Ϣ
            }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // ����µ���־�Ƿ������һ����־��ͬ
        if (logMessages.Count == 0 || logMessages.Peek() != logString)
        {
            logMessages.Enqueue(logString);  // �������־������
            UpdateLogText();  // ������ʾ����־��Ϣ
        }
    }

    void UpdateLogText()
    {
        logTextMeshPro.ToolTipText = string.Join("\n", logMessages.ToArray());
    }
}
