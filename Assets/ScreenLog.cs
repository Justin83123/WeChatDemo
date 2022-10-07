using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ScreenLog : MonoBehaviour
{
    struct LogContant
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    static readonly Dictionary<LogType, Color> m_logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

    private Action<string> m_callBack;
    public bool m_restrictLogCount = false;
    public int m_maxLogs = 1000;

    private List<LogContant> m_logs = new List<LogContant>();
    private Vector2 m_scrollPosition;
    private bool m_visible;
    private bool m_collapse;

    private const int m_margin = 160;
    private static readonly GUIContent m_clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    private static readonly GUIContent m_collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    private Rect m_titleBarRect;
    private Rect m_windowRect;
    private Rect m_visibleBtn;
    private Rect m_cmdBtn;
    private Rect m_cmdRect;
    private string m_cmd;
    private GUISkin m_skin;
    public FileStream m_filesSream;

    public void Initiate(GUISkin skin, Action<string> callBack)
    {
        m_filesSream = new FileStream(Path.Combine(Application.persistentDataPath, "gameLog.txt"), FileMode.OpenOrCreate);
        m_skin = skin;
        m_restrictLogCount = false;
        m_maxLogs = 1000;        
        m_visible = false;
        Rect m_titleBarRect = new Rect(0, 0, 10000, 20);
        m_windowRect = new Rect(m_margin, 10 + m_margin, Screen.width - (m_margin * 2), Screen.height - (m_margin * 2));
        m_visibleBtn = new Rect(80.0f, 80.0f, 100.0f, 100.0f);
        m_cmdBtn = new Rect(150.0f, 10.0f, 50.0f, 40.0f);
        m_cmdRect = new Rect(250.0f, 10.0f, Screen.width * 0.5f, 40.0f);
        m_cmd = "";
        m_callBack = callBack;
    }

    void OnEnable()
    {
#if UNITY_3 || UNITY_4
        Application.RegisterLogCallback(HandleLog);  
#else
        Application.logMessageReceived += HandleLog;
#endif
    }

    void OnDisable()
    {
#if UNITY_3 || UNITY_4
        Application.RegisterLogCallback(null);  
#else
        Application.logMessageReceived -= HandleLog;
#endif
    }

    void OnGUI()
    {
        GUI.skin = m_skin;
        if (GUI.Button(m_visibleBtn, "v"))
        {
            m_visible = !m_visible;
        }
        if (m_visible)
        {
            m_cmd = GUI.TextField(m_cmdRect, m_cmd, 50);//15为最大字符串长度  
            if (GUI.Button(m_cmdBtn, "cmd"))
            {
                if (m_callBack != null && !string.IsNullOrEmpty(m_cmd))
                {
                    m_callBack(m_cmd);
                }
            }
            m_windowRect = GUILayout.Window(123456, m_windowRect, DrawConsoleWindow, "Log");
        }
    }

    void DrawConsoleWindow(int windowID)
    {
        DrawLogsList();
        DrawToolbar();
        GUI.DragWindow(m_titleBarRect);
    }

    void DrawLogsList()
    {
        m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
        for (var i = 0; i < m_logs.Count; i++)
        {
            var log = m_logs[i];
            if (m_collapse && i > 0)
            {
                var previousMessage = m_logs[i - 1].message;
                if (log.message == previousMessage)
                {
                    continue;
                }
            }
            GUI.contentColor = m_logTypeColors[log.type];
            GUILayout.Label(log.message);
            GUILayout.Label(log.stackTrace);
        }
        GUILayout.EndScrollView();
        GUI.contentColor = Color.white;
    }

    void DrawToolbar()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(m_clearLabel))
        {
            m_logs.Clear();
        }
        m_collapse = GUILayout.Toggle(m_collapse, m_collapseLabel, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        LogContant log = new LogContant()
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        };

        m_logs.Add(log);
        message = message + "\n";
        stackTrace = stackTrace + "\n";
        m_filesSream.Write(System.Text.Encoding.Default.GetBytes(message), 0, message.Length);
        m_filesSream.Write(System.Text.Encoding.Default.GetBytes(stackTrace), 0, stackTrace.Length);
        m_filesSream.Flush();
        TrimExcessLogs();
    }

    void TrimExcessLogs()
    {
        if (m_restrictLogCount)
        {
            var amountToRemove = Mathf.Max(m_logs.Count - m_maxLogs, 0);
            if (amountToRemove != 0)
            {
                m_logs.RemoveRange(0, amountToRemove);
            }
        }
    }
}