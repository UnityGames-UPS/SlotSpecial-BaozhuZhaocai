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
    [SerializeField] private Button SpinButton;
    [SerializeField] private Button StopSpinButton;
    [SerializeField] private Button AutoSpinButton;
    [SerializeField] private Button StopAutoSpinButton;

    [Header("Sprites")]
    [SerializeField] private Sprite SpinButtonSprite;
    [SerializeField] private Sprite StopButtonSprite;

    [Header("Bet Area")]
    [SerializeField] private Button PlusBetButton;
    [SerializeField] private Button MinusBetButton;
    [SerializeField] private TMP_Text BetAmountText;

    [Header("InfoPanel")]
    [SerializeField] private Button InfoButton;
    [SerializeField] private GameObject InfoPanel;
    [SerializeField] private Button CloseInfoPanelButton;
    [SerializeField] private Button NextInfoPageButton;
    [SerializeField] private Button BackInfoPageButton;
    [SerializeField] private TMP_Text InfoPageNumberText;

    [Header("Texts")]
    [SerializeField] internal TMP_Text BalanceText;
    [SerializeField] private TMP_Text WinAmountText;

    [Header("References")]
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private SlotManager slotManager;
    internal int betCounter = 1;
    internal double currentTotalBet;
    private Tweener TextTween;

    private void Start()
    {
        if (PlusBetButton) PlusBetButton.onClick.AddListener(() => ChangeBet(true));
        if (MinusBetButton) MinusBetButton.onClick.AddListener(() => ChangeBet(false));
        if (SpinButton) SpinButton.onClick.AddListener(SpinButtonPressed);
        // InitializeUIData();        
    }

    internal void InitializeUIData()
    {
        if (BetAmountText) BetAmountText.text = socketManager.initialData.bets[betCounter].ToString();
        currentTotalBet = socketManager.initialData.bets[betCounter];
        if (BalanceText) BalanceText.text = socketManager.playerdata.balance.ToString();
        slotManager.currentBalance = socketManager.playerdata.balance;
        //slotManager.shuffleInitialMatrix();
        slotManager.shuffleSlotImages();
    }

    private void ChangeBet(bool IncDec)
    {
        // if (audioController) audioController.PlayButtonAudio();
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
    }

    private void SpinButtonPressed()
    {
        SpinButton.interactable = false;
        //SpinButton.gameObject.GetComponent<Image>().sprite = StopButtonSprite;
        StopSpinButton.gameObject.SetActive(true);
        slotManager.StartSlots();
    }

    internal void OnSpinEnd()
    {
        SpinButton.interactable = true;
        //SpinButton.gameObject.GetComponent<Image>().sprite = SpinButtonSprite;
        StopSpinButton.gameObject.SetActive(false);
    }

    internal void StartTextAnim(double initAmount , double amount , TMP_Text winText , float duration )
    {
        //double initAmount = 0;
        TextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, duration).OnUpdate(() =>
        {
            winText.text = initAmount.ToString("F3");
        });
    }

    internal void TotalWinPopup(double winAmount)
    {
        
    }

}