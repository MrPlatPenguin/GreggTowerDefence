using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class TipsPopUp : MonoBehaviour
{
    static TipsPopUp instance;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] HorizontalLayoutGroup hlg;
    [SerializeField] RectTransform rectTransform;

    [TextArea]
    [SerializeField] string atStart, uponOpeningTheBuildMenuForTheFirstTime, uponFirstWoodPickup, uponFirstStructurePlaced, firstNight, dayTwoMorning, upgradeHint;

    bool _showToolTips = true;

    bool tooltipActive;

    private void Awake()
    {
        instance = this;

        SubscribeEvents();

        HideMessage();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            HideMessage();
        }
    }

    public static void ShowMessage(string message)
    {
        if (!OptionsSave.Save.ShowTooltips) return;

        instance.gameObject.SetActive(true);

        instance.text.text = message;

        LayoutRebuilder.ForceRebuildLayoutImmediate(instance.rectTransform);
        GameManager.PauseGame();
    }

    public static bool TooltipActive()
    {
        return instance.gameObject.activeInHierarchy;
    }

    public static void HideMessage()
    {
        instance.gameObject.SetActive(false);
        GameManager.UnpauseGame();
    }

    void SubscribeEvents()
    {

        PlayerEvents.OnPlayerSpawn += (Player player) => ShowMessage(atStart);
        //BuilderUI.OnNextOpen += () => ShowMessage(uponOpeningTheBuildMenuForTheFirstTime);
        ItemResource.OnFirstPickUp += () => ShowMessage(uponFirstWoodPickup);
        Builder.OnNextBuild += () => ShowMessage(uponFirstStructurePlaced);
        GameManager.OnFirstNight += () => ShowMessage(firstNight);
        GameManager.OnDayFinish += () => ShowMessage(dayTwoMorning);
        GameManager.OnDayFinish += () => ShowMessage(dayTwoMorning);
        GameManager.OnDayFinish += DisableTips;
        Builder.UpgradeHint+= () => ShowMessage(upgradeHint);
    }

    void DisableTips()
    {
        OptionsSave.Save.ShowTooltips = false;
        OptionsSave.SaveData();
    }
}
