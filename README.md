# Photo search sample project with UniTask
Unitask 를 사용하여 API 호출하는 간단한 예제 입니다. Unitask 와 try-catch 를 실제 프로젝트에 어떻게 사용해야 할지에 초점을 맞췄습니다.

## Overview
![overview](https://user-images.githubusercontent.com/23477337/103743406-6f930580-503f-11eb-90fe-c898b383c4f4.png)
![image](https://user-images.githubusercontent.com/23477337/103743496-9b15f000-503f-11eb-9bcd-8576210b7afa.png)

## Use In Project
- UniTask
- TotalJson
- Unsplash API

## Code
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;
using Leguar.TotalJSON;

public class Example : MonoBehaviour {
    public InputField textInput;
    public Button searchButton;
    public Transform content;
    public GameObject photoContainer;

    private void Awake() {
        searchButton.onClick.AddListener(() => OnClickSearch(textInput.text).Forget());
    }

    private async UniTaskVoid OnClickSearch(string term) {
        // 사진 목록 제거
        foreach (Transform photo in content) {
            Destroy(photo.gameObject);
        }

        // API 로부터 사진 목록 정보 받기
        var photosData = await GetPhotosData(term);
        if (photosData == null) {
            Debug.LogWarning("photos data is null");
            return;
        }

        // 사진 목록 보여주기 비동기 병렬 처리
        var photos = JSON.ParseString(photosData).GetJArray("results").AsJSONArray();
        foreach (var photo in photos) {
            var url = photo.GetJSON("urls").GetString("regular");
            LoadAndShowPhoto(url).Forget();
        }
    }

    private async UniTask<string> GetPhotosData(string term) {
        using (var req = UnityWebRequest.Get($"https://api.unsplash.com/search/photos?query={term}")) {
            try {
                req.SetRequestHeader("Authorization", "Client-ID S2F1QMER5i40nOj6vV_JUrGfK7e2l7Ue0f_MLUX5STQ");
                await req.SendWebRequest();
                return req.downloadHandler.text;
            }
            catch (Exception err) {
                Debug.LogError("GetPhotosData: " + err);
                return null;
            }
        }
    }

    private async UniTask LoadAndShowPhoto(string url) {
        // url 로부터 텍스쳐 가져오기
        var texture = await GetTexture(url);
        if (texture == null) {
            Debug.LogWarning("texture is null");
            return;
        }

        // 사진 컨테이너 생성
        var obj = await GetPhotoContainerInstance();
        if (obj == null) {
            Debug.LogWarning("photo container instance is null");
            return;
        }
        var photoContainer = Instantiate(obj, content);
        photoContainer.GetComponent<RawImage>().texture = texture;
    }

    private async UniTask<Texture> GetTexture(string url) {
        using (var req = UnityWebRequestTexture.GetTexture(url)) {
            try {
                await req.SendWebRequest();
                return DownloadHandlerTexture.GetContent(req);
            }
            catch (Exception err) {
                Debug.LogError("GetTexture error: " + err);
                return null;
            }
        }
    }

    private async UniTask<GameObject> GetPhotoContainerInstance() {
        try {
            var obj = await Resources.LoadAsync("PhotoContainer") as GameObject;
            return obj;
        }
        catch (Exception err) {
            Debug.LogError("GetPhotoContainerInstance error: " + err);
            return null;
        }
    }
}

```
