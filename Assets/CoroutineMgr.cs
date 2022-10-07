using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CoroutineMgr : MonoBehaviour
{
    public void WaitForFrames(int frameCnt, Action finish)
    {
        if (frameCnt <= 0)
        {
            if (finish != null)
            {
                finish();
            }
        }
        else
        {
            StartCoroutine(WaitForFramesAsync(frameCnt, finish));
        }
    }
    private IEnumerator WaitForFramesAsync(int frameCnt, Action finish)
    {
        for(int i = 0; i < frameCnt; ++i)
        {
            yield return null;
        }
        if (finish != null)
        {
            finish();
        }
    }
}
