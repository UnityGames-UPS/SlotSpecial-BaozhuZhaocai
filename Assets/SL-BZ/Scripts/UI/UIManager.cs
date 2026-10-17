using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] internal Button SpinButton;
    [SerializeField] internal Button StopSpinButton;
    [SerializeField] internal Button AutoSpinButton;
    [SerializeField] internal Button StopAutoSpinButton;

    [Header("Bet Area")]
    [SerializeField] private Button PlusBetButton;
    [SerializeField] private Button MinusBetButton;
    [SerializeField] private TMP_Text BetAmountText;

    [Header("Texts")]
    [SerializeField] internal TMP_Text BalanceText;
    [SerializeField] internal TMP_Text WinAmountText;
    [SerializeField] internal TMP_Text MiniText;
    [SerializeField] internal TMP_Text MinorText;
    [SerializeField] internal TMP_Text MajorText;
    [SerializeField] internal TMP_Text GrandText;

    [Header("Main Popus UI Object")]
    [SerializeField] private GameObject MainPopup_Object;

    [Header("Paytable Popup UI References")]
    [SerializeField] private GameObject[] Pages;
    [SerializeField] private Button Paytable_Button;
    [SerializeField] private GameObject PaytablePopup_Object;
    [SerializeField] private Button PaytableExit_Button;
    [SerializeField] private Button PaytableLeft_Button;
    [SerializeField] private Button PaytableRight_Button;
    [SerializeField] private TMP_Text InfoPageNumberText;

    [Header("Sound/Music UI References")]
    [SerializeField] private Button Sound_Button;
    [SerializeField] private Button SoundOff_Button;
    [SerializeField] private Button Music_Button;
    [SerializeField] private Button MusicOff_Button;

    [Header("Win Popup UI References")]

    [SerializeField] private GameObject BigWinObject;
    [SerializeField] private GameObject EpicWinObject;
    [SerializeField] private GameObject TotalWinObject;
    [SerializeField] private GameObject GlowObject;
    [SerializeField] private GameObject CoinObject;
    [SerializeField] private GameObject CrackerObject;

    [SerializeField] private GameObject WinPopup_Object;
    [SerializeField] private Transform WinPopupParent;
    [SerializeField] private TMP_Text Win_Text;
    [SerializeField] private Button SkipWinAnimation;

    [Header("Disconnection Popup UI References")]
    [SerializeField] private Button CloseDisconnect_Button;
    [SerializeField] private GameObject DisconnectPopup_Object;

    [Header("Reconnection Popup")]
    [SerializeField] private GameObject ReconnectPopup_Object;

    [Header("AnotherDevice Popup UI References")]
    [SerializeField] private Button CloseAD_Button;
    [SerializeField] private GameObject ADPopup_Object;

    [Header("LowBalance Popup UI References")]
    [SerializeField] private Button LBExit_Button;
    [SerializeField] private GameObject LBPopup_Object;

    [Header("Quit Popup UI References")]
    [SerializeField] private GameObject QuitPopup_Object;
    [SerializeField] private Button YesQuit_Button;
    [SerializeField] private Button NoQuit_Button;
    [SerializeField] private Button CrossQuit_Button;
    [SerializeField] private Button GameExit_Button;

    [Header("References")]
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private AudioController _audioController;

    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;
    private Tween WinPopupTextTween;
    private Tween ClosePopupTween;
    private Tween WinTextScaleTween;
    private Tween WinImageScaleTween;
    internal int betCounter = 1;
    private int paytablePageCounter;
    internal double currentTotalBet;
    private Tweener TextTween;

    private void Awake()
    {
        JSFunctCalls jsFunctCalls = FindObjectOfType<JSFunctCalls>();
        if (jsFunctCalls != null)
            jsFunctCalls.RegisterVisibilityListener(gameObject.name);
    }

    public void OnFocusChanged(string value)
    {
        bool focused = value == "1";
        Debug.Log("UNITY FOCUS CHANGED: " + value + " (focused: " + focused + ")");
        _audioController?.SetMuteAll(!focused);
        socketManager?.HandleFocusChange(focused);
    }

    internal void UpdateBalanceDisplay(double newBalance)
    {
        if (BalanceText) BalanceText.text = newBalance.ToString();
        slotManager.currentBalance = newBalance;
        if (newBalance < currentTotalBet)
            LowBalPopup();
    }

    private void Start()
    {

        isMusic = true;
        isSound = true;
        if (PlusBetButton) PlusBetButton.onClick.AddListener(() => ChangeBet(true));
        if (MinusBetButton) MinusBetButton.onClick.AddListener(() => ChangeBet(false));
        if (SpinButton) SpinButton.onClick.AddListener(SpinButtonPressed);
        if (AutoSpinButton) AutoSpinButton.onClick.AddListener(AutoSpin);
        if (StopAutoSpinButton) StopAutoSpinButton.onClick.AddListener(AutoSpinStop);

        // ─────────────────────────────────────────────────────────────────────
        // CHANGED: Wire the Stop Spin button to TriggerStopSpin on SlotManager
        // so pressing it during a spin stops all reels simultaneously.
        // Previously this listener was missing, so StopSpinToggle was never set.
        // ─────────────────────────────────────────────────────────────────────
        if (StopSpinButton) StopSpinButton.onClick.RemoveAllListeners();
        if (StopSpinButton) StopSpinButton.onClick.AddListener(() =>
        {
            _audioController.PlayUIButton();
            slotManager.TriggerStopSpin();
        });

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(delegate { OpenPaytable(); });

        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

        if (PaytableLeft_Button) PaytableLeft_Button.onClick.RemoveAllListeners();
        if (PaytableLeft_Button) PaytableLeft_Button.onClick.AddListener(() => { SwitchPages(false); });

        if (PaytableRight_Button) PaytableRight_Button.onClick.RemoveAllListeners();
        if (PaytableRight_Button) PaytableRight_Button.onClick.AddListener(() => { SwitchPages(true); });

        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate
        {
            OpenPopup(QuitPopup_Object);
        });

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup(QuitPopup_Object);
            }
        });

        if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
        if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup(QuitPopup_Object);
            }
        });

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate
        {
            CallOnExitFunction();
        });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        if (SkipWinAnimation) SkipWinAnimation.onClick.RemoveAllListeners();
        if (SkipWinAnimation) SkipWinAnimation.onClick.AddListener(SkipWin);
    }

    internal void InitializeUIData()
    {
        if (BetAmountText) BetAmountText.text = socketManager.initialData.bets[betCounter].ToString();
        currentTotalBet = socketManager.initialData.bets[betCounter];
        if (BalanceText) BalanceText.text = socketManager.playerdata.balance.ToString();
        slotManager.currentBalance = socketManager.playerdata.balance;
        if (MiniText) MiniText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.MINI[0] * socketManager.initialData.bets[betCounter]).ToString();
        if (MiniText) MinorText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.MINOR[0] * socketManager.initialData.bets[betCounter]).ToString();
        if (MiniText) MajorText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.MAJOR[0] * socketManager.initialData.bets[betCounter]).ToString();
        if (MiniText) GrandText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.GRAND[0] * socketManager.initialData.bets[betCounter]).ToString();
        //slotManager.shuffleInitialMatrix();
        slotManager.shuffleSlotImages();
    }

    private void ChangeBet(bool IncDec)
    {
        if (_audioController) _audioController.PlayUIButton();
        if (IncDec)
        {
            betCounter++;
            if (betCounter >= socketManager.initialData.bets.Count)
            {
                betCounter = 0; // Loop back to the first bet
            }
        }
        else
        {
            betCounter--;
            if (betCounter < 0)
            {
                betCounter = socketManager.initialData.bets.Count - 1; // Loop to the last bet
            }
        }
        if (BetAmountText) BetAmountText.text = socketManager.initialData.bets[betCounter].ToString();
        currentTotalBet = socketManager.initialData.bets[betCounter];
        if (MiniText) MiniText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.MINI[0] * socketManager.initialData.bets[betCounter]).ToString();
        if (MiniText) MinorText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.MINOR[0] * socketManager.initialData.bets[betCounter]).ToString();
        if (MiniText) MajorText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.MAJOR[0] * socketManager.initialData.bets[betCounter]).ToString();
        if (MiniText) GrandText.text = ((float)socketManager.features.jackpots.jackpotMultipliers.GRAND[0] * socketManager.initialData.bets[betCounter]).ToString();
    }

    private void SpinButtonPressed()
    {
        _audioController.PlayUIButton();
        SpinButton.interactable = false;
        //SpinButton.gameObject.GetComponent<Image>().sprite = StopButtonSprite;
        StopSpinButton.gameObject.SetActive(true);
        StopSpinButton.interactable = true;
        AutoSpinButton.interactable = false;
        SetBetButtonsInteractable(false);   // lock bet buttons for the entire spin + bonus
        slotManager.StartSlots();
    }

    internal void OnSpinEnd()
    {
        SpinButton.interactable = true;
        //SpinButton.gameObject.GetComponent<Image>().sprite = SpinButtonSprite;
        StopSpinButton.gameObject.SetActive(false);
        AutoSpinButton.interactable = true;
        SetBetButtonsInteractable(true);    // re-enable bet buttons when spin fully ends
    }

    // NEW: single place to lock/unlock bet +/- buttons.
    // Called false on spin/autospin start, true on spin end (only when not autospinning).
    internal void SetBetButtonsInteractable(bool value)
    {
        if (PlusBetButton)  PlusBetButton.interactable  = value;
        if (MinusBetButton) MinusBetButton.interactable = value;
    }

    private void AutoSpin()
    {
        _audioController.PlayUIButton();
        SpinButton.interactable = false;
        StopAutoSpinButton.interactable = true;
        StopAutoSpinButton.gameObject.SetActive(true);
        AutoSpinButton.gameObject.SetActive(false);
        StopSpinButton.gameObject.SetActive(true);
        StopSpinButton.interactable = true;
        SpinButton.gameObject.SetActive(false);
        SetBetButtonsInteractable(false);   // lock bet buttons for entire auto-spin session
        slotManager.AutoSpin();
    }

    private void AutoSpinStop()
    {
        _audioController.PlayUIButton();
        StopAutoSpinButton.interactable = false;
        //SpinButton.interactable = false;
        // StopAutoSpinButton.gameObject.SetActive(true);
        // StopSpinButton.gameObject.SetActive(true);
        slotManager.StopAutoSpin();
    }

    internal void StartTextAnim(double initAmount, double amount, TMP_Text winText, float duration)
    {
        //double initAmount = 0;
        TextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, duration).OnUpdate(() =>
        {
            winText.text = initAmount.ToString("F3");
        });
    }

    private void OpenPaytable()
    {
        _audioController.PlayUIButton();
        foreach (GameObject gameObject in Pages)
        {
            gameObject.SetActive(false);
        }
        paytablePageCounter = 0;
        Pages[0].SetActive(true);
        MainPopup_Object.SetActive(true);
        PaytablePopup_Object.SetActive(true);
    }

    private void SwitchPages(bool IncDec)
    {
        _audioController.PlayUIButton();
        if (IncDec)
        {
            paytablePageCounter++;
            if (paytablePageCounter == Pages.Length)
            {
                paytablePageCounter = 0;
            }
            foreach (GameObject gameObject in Pages)
            {
                gameObject.SetActive(false);
            }
            Pages[paytablePageCounter].SetActive(true);
            InfoPageNumberText.text = (paytablePageCounter + 1).ToString();
        }
        else
        {
            paytablePageCounter--;
            if (paytablePageCounter == -1)
            {
                paytablePageCounter = Pages.Length - 1;
            }
            foreach (GameObject gameObject in Pages)
            {
                gameObject.SetActive(false);
            }
            Pages[paytablePageCounter].SetActive(true);
            InfoPageNumberText.text = (paytablePageCounter + 1).ToString();
        }
    }


    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup()
    {
        if (!isExit)
        {
            isExit = true;
            OpenPopup(DisconnectPopup_Object);
        }
    }
    internal void ReconnectionPopup()
    {
        OpenPopup(ReconnectPopup_Object);
    }

    internal void PopulateWin(int value, double amount)
    {
        slotManager.CheckPopups = true;
        BigWinObject.SetActive(false);
        EpicWinObject.SetActive(false);
        TotalWinObject.SetActive(false);
        switch (value)
        {
            case 1:
                //Win_Image = BigWin_Image;
                BigWinObject.SetActive(true);
                BigWinObject.GetComponent<ImageAnimation>().ResetImageState();
                BigWinObject.GetComponent<ImageAnimation>().StartAnimation();
                break;
            case 2:
                //Win_Image = MegaWin_Image;
                EpicWinObject.SetActive(true);
                EpicWinObject.GetComponent<ImageAnimation>().ResetImageState();
                EpicWinObject.GetComponent<ImageAnimation>().StartAnimation();
                break;
            case 3:
                //Win_Image = HugeWin_Image;
                TotalWinObject.SetActive(true);
                TotalWinObject.transform.GetChild(0).GetComponent<ImageAnimation>().ResetImageState();
                TotalWinObject.transform.GetChild(0).GetComponent<ImageAnimation>().StartAnimation();
                break;
        }
        //_audioController.PlayWLAudio("megaWin");
        StartPopupAnim(amount);
    }

    void SkipWin()
    {
        Debug.Log("Skip win called");
        if (ClosePopupTween != null)
        {
            ClosePopupTween.Kill();
            ClosePopupTween = null;
        }
        if (WinPopupTextTween != null)
        {
            WinPopupTextTween.Kill();
            WinPopupTextTween = null;
            // Win_Text.text = _socketManager.resultData.payload.winAmount.ToString("F3");
        }

        if (WinImageScaleTween != null)
        {
            WinImageScaleTween.Kill();
            WinImageScaleTween = null;
        }

        if (WinTextScaleTween != null)
        {
            WinTextScaleTween.Kill();
            WinTextScaleTween = null;
        }
        EndPopupAnim();
    }

    private void StartPopupAnim(double amount)
    {
        _audioController.PlayWin();
        double initAmount = 0;
        WinPopupParent.localScale = Vector3.zero;

        //if (Win_Image) Win_Image.gameObject.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        GlowObject.GetComponent<ImageAnimation>().ResetImageState();
        GlowObject.GetComponent<ImageAnimation>().StartAnimation();
        CoinObject.GetComponent<ImageAnimation>().ResetImageState();
        CoinObject.GetComponent<ImageAnimation>().StartAnimation();
        CrackerObject.GetComponent<ImageAnimation>().ResetImageState();
        CrackerObject.GetComponent<ImageAnimation>().StartAnimation();
        WinImageScaleTween = WinPopupParent.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        WinPopupTextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, 1.5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString("F3");
        });

        ClosePopupTween = DOVirtual.DelayedCall(4f, () =>
        {
            SkipWin();
        });
    }

    void EndPopupAnim()
    {
        WinPopupParent.DOScale(Vector3.zero, 0.5f)
        .SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            if (WinPopup_Object) WinPopup_Object.SetActive(false);
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
            slotManager.CheckPopups = false;
        });
    }

    private void CallOnExitFunction()
    {
        isExit = true;
        //_audioController.PlayButtonAudio();
        StartCoroutine(socketManager.CloseSocket());
    }

    private void OpenPopup(GameObject Popup)
    {
        if (_audioController) _audioController.PlayUIButton();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {
        if (_audioController) _audioController.PlayUIButton();
        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf)
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    internal void CheckAndClosePopups()
    {
        if (ReconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(ReconnectPopup_Object);
        }
        if (DisconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(DisconnectPopup_Object);
        }
    }

}