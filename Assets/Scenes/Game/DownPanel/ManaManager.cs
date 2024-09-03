using TMPro;
using UnityEngine;
using SFH;
using DG.Tweening;

public class ManaManager : MonoSingleton<ManaManager> {
    [SerializeField] private TMP_Text manaAmountText;
    [SerializeField] private TMP_Text manaPerSecondText;
    [SerializeField] private TMP_Text manaPerClickText;
    [SerializeField] private ResourceData manaResource;
    [SerializeField] private Transform manaButton;
    [Header("Effects")]
    [SerializeField] private ParticleFXSO manaFx;
    [SerializeField] private Transform fxPosition;
    //Slider
    [SerializeField] private RectTransform manaBackgroundTransform;
    [SerializeField] private RectTransform manaAmountTransform;
    private int manaAmount;
    private int maxMana = 1000;
    private int manaPerSecond = 0;
    public void Clicked() {
        manaAmount += 1 + BonusStorage.inst.addBonuses["ManaClick"];
        manaAmount = Mathf.Clamp(manaAmount, 0, maxMana);
        UpdateUI();
        //Tween button
        manaButton.DOScale(.9f,.125f/2.0f).OnComplete(()=>manaButton.DOScale(1.0f,0.125f / 2.0f));
        PFXManager.inst.Emit(manaFx, fxPosition.position);
        
    }

    public void AddMana(int amount) {
        if (manaAmount + amount >= maxMana) {
            manaAmount = maxMana;
        } else {
            manaAmount += amount;
        }
        UpdateUI();
    }
    public bool TryConsumeMana(int amount) {
        if (manaAmount - amount >= 0) {
            manaAmount -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void UpdateUI() {
        manaAmountText.SetText($"{manaAmount}/{maxMana}");
        float percent = manaAmount/ (float)maxMana;
        manaAmountTransform.SetWidth(percent * manaBackgroundTransform.GetWidth());
        if (manaPerSecond > 0) {
            manaPerSecondText.SetText($"{manaPerSecond}/s");
        } else {
            manaPerSecondText.SetText("");
        }
        manaPerClickText.SetText($"+{1 + BonusStorage.inst.addBonuses["ManaClick"]}");
    }

    #region Internal

    private void Start() {
        UpdateUI();
    }

    #endregion
}
