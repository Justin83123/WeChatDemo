using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using WeChatWASM;

// ÓÎÏ·×ÊÔ´CDN:
// https://assetstreaming-content.unity.cn/client_api/v1/buckets/2f1f8329-ed84-4085-b0be-be5b571f9e66/release_by_badge/latest/content/
public class Launcher : MonoBehaviour
{
    public GUISkin m_skin;
    public CoroutineMgr m_coroutine;
    private AsyncOperationHandle<SceneInstance> m_loadHandle;
    private AsyncOperationHandle m_downloadHandle;
    private float m_startTime;
    void Start()
    {
        Debug.Log("Launcher Start");
        m_coroutine.WaitForFrames(5, OnLastFrame);
    }

    private void OnLastFrame()
    {
        m_startTime = Time.timeSinceLevelLoad;
        WX.SetGameStage(0);
        WX.ReportGameStageCostTime(0, "Launcher Scene Start");
        if (m_skin != null)
        {
            ScreenLog screenLog = gameObject.AddComponent<ScreenLog>();
            screenLog.Initiate(m_skin, CmdMsg);
        }
        m_loadHandle = Addressables.LoadSceneAsync("Assets/Scenes/Field.unity");
        m_loadHandle.Completed += OnLoadCompleted;
    }
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        Debug.Log("OnLoadCompleted");
        float deltaTime = Mathf.Max(0.0f, Time.timeSinceLevelLoad - m_startTime);
        int costTime = Mathf.CeilToInt(deltaTime * 1000f);
        WX.SetGameStage(1);
        WX.ReportGameStageCostTime(costTime, "OnLoadCompleted");
        WX.ReportGameStart();
        if (handle.IsDone)
        {
            Debug.Log("handle.IsDone");
            SceneInstance scene = handle.Result;
        }
        else if (handle.OperationException != null)
        {
            Debug.LogError("handle.OperationException: " + handle.OperationException.Message);
            WX.ReportGameStageError((int)handle.Status, handle.OperationException.Message, string.Empty);
        }
    }

    private void CmdMsg(string cmd)
    {
        //Debug.Log("CmdMsg: " + cmd);
    }
}
