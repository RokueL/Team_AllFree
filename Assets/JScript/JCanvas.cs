using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

/// <summary>
/// UI 관리를 위한 캔버스 스크립트이다
/// </summary>
public class JCanvas : MonoBehaviour
{
    /// <summary> [게임 오브젝트] 메인 캔버스 </summary>
    [Header("메인화면 캔버스")]
    public GameObject MainCanvas;
    /// <summary> [게임 오브젝트] 로고 오브젝트 </summary>
    [Header("메인화면 로고")]
    public GameObject MainLogo;
    /// <summary> [게임 오브젝트] 시작 버튼 오브젝트 </summary>
    [Header("메인화면 시작 버튼")]
    public GameObject StartBtn;
    /// <summary> [게임 오브젝트] 설정 버튼 오브젝트 </summary>
    [Header("메인화면 설정 버튼")]
    public GameObject MainSettingBtn;
    /// <summary> [게임 오브젝트] 엔드 버튼 오브젝트 </summary>
    [Header("메인화면 끝 버튼")]
    public GameObject EndBtn;

    [Header("==============================================")] [Space(1f)]
    private int a;
    
    /// <summary> [게임 오브젝트] 인 게임 캔버스 </summary>
    [Header("인 게임 캔버스")]
    public GameObject InGameCanvas;
    /// <summary> [게임 오브젝트] 로고 오브젝트 </summary>
    [Header("인 게임 인벤토리 버튼")]
    public GameObject InvenBtn;
    /// <summary> [게임 오브젝트] 시작 버튼 오브젝트 </summary>
    [Header("인 게임 설정 버튼")]
    public GameObject InGameSettingBtn;
    /// <summary> [게임 오브젝트] 엔드 버튼 오브젝트 </summary>
    [Header("인 게임 끝 버튼")]
    public GameObject InGameEndBtn;

    [Header("==============================================")] [Space(1f)]
    private bool isOpen;
    
    /// <summary> [게임 오브젝트] 엔드 버튼 오브젝트 </summary>
    [Header("창 인벤토리")]
    public GameObject InventoryWindow;
    
    /// <summary> [게임 오브젝트] 엔드 버튼 오브젝트 </summary>
    [Header("창 세팅")]
    public GameObject SettingWindow;

    /// <summary> [게임 오브젝트] 플레이어 </summary>
    [Header("[게임 오브젝트] 플레이어")]
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& Player.GetComponent<Player>().isStart)
        {
            if (!isOpen)
            {
                if(!InGameCanvas.activeSelf)
                    InGameCanvas.SetActive(true);
                InGameCanvas.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
                isOpen = true;
            }
            else
            {
                InGameCanvas.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
                InGameCanvas.SetActive(false);
                isOpen = false;
            }
        }
    } 
    
//==================================================================================
//==================================================================================
//==================================================================================
//               < 사용자 지정 함수 >
//==================================================================================
//==================================================================================
//==================================================================================
    
    
    
    
//==================================================================================
//==================================================================================
//==================================================================================
//               < 이벤트 함수 >
//==================================================================================
//==================================================================================
//==================================================================================

    /// <summary> 시작 버튼 이벤트 </summary>
    public void OnStartBtnEvent()
    {
        MainCanvas.GetComponent<CanvasGroup>().DOFade(0, 2f);
        Invoke("MainCanvasActiveFalse",2f);
    }
    
    /// <summary> 메인 화면 캔버스 끄기 </summary>
    private void MainCanvasActiveFalse()
    {
        MainCanvas.SetActive(false);
    }
    /// <summary> 버튼 올리면 켜지기 </summary>
    public void ESCButtonEnterEvent(GameObject Btn)
    {
        Btn.GetComponent<Image>().DOFade(0.2f, 0.3f);
    }
    /// <summary> 버튼 벗어나면 끄기 </summary>
    public void ESCButtonExitEvent(GameObject Btn)
    {
        Btn.GetComponent<Image>().DOFade(0f, 0.3f);
    }

    public void WindowOpenEvent(GameObject Canvas)
    {
        InventoryWindow.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
        SettingWindow.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
        Canvas.SetActive(true);
        Canvas.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
    }
    
    public void WindowCloseEvent(GameObject Canvas)
    {
        Canvas.GetComponent<CanvasGroup>().DOFade(0f, 0.3f);
        Canvas.SetActive(false);
    }
    
    
    
}
