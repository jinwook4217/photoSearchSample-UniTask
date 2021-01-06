using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UniTaskDemo : MonoBehaviour {

    /// <summary>
    /// 리소스 로드 Async
    /// </summary>
    public Image spriteRender;

    public void OnClickLoadSprite() {
        LoadSpriteAsync("hello").Forget();
    }

    public async UniTaskVoid LoadSpriteAsync(string path) {
        var resource = await Resources.LoadAsync<Sprite>(path);
        spriteRender.sprite = resource as Sprite;
    }

    /// <summary>
    /// 기본적인 딜레이
    /// </summary>
    public void OnClickDelaySecond() {
        Debug.Log("delay start");
        // Forget() 으로 비동기 병렬처리
        Delay(1000).Forget();
        Debug.Log("execute immediately");
    }

    public async UniTaskVoid Delay(int ms) {
        await UniTask.Delay(ms);
        Debug.Log($"delay {ms} ms!");
    }

    /// <summary>
    /// Try-catch
    /// </summary>
    public void OnClickGetTextFromGoogle() {
        GetTextFromGoogleAsync().Forget();
    }

    private async UniTaskVoid GetTextFromGoogleAsync() {
        var uri = "http://google.com";
        try {
            var result = await GetTextAsync(uri);
            Debug.Log(result);
        }
        catch (Exception e) {
            Debug.LogException(e);
        }
    }

    private async UniTask<string> GetTextAsync(string uri) {
        var uwr = UnityWebRequest.Get(uri);

        await uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError) {
            // 실패시 예외 throw
            throw new Exception(uwr.error);
        }

        return uwr.downloadHandler.text;
    }
}
