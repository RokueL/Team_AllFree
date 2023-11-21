using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// UI 관리를 위한 캔버스 스크립트이다
/// </summary>
public class JCanvas : MonoBehaviour
{
    [Header("메인화면 UI들")]
    /// <summary> [게임 오브젝트] 메인 캔버스 </summary>
    public GameObject MainCanvas;
    /// <summary> [게임 오브젝트] 로고 오브젝트 </summary>
    public GameObject MainLogo;
    /// <summary> [게임 오브젝트] 시작 버튼 오브젝트 </summary>
    public GameObject StartBtn;
    /// <summary> [게임 오브젝트] 설정 버튼 오브젝트 </summary>
    public GameObject SettingBtn;
    /// <summary> [게임 오브젝트] 엔드 버튼 오브젝트 </summary>
    public GameObject EndBtn;


    public void OnStartBtnEvent()
    {
        MainCanvas.GetComponent<CanvasGroup>().DOFade(0, 2f);
        Invoke("MainCanvasActiveFalse",2f);
    }

    private void MainCanvasActiveFalse()
    {
        MainCanvas.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
