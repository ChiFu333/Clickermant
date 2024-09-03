using System.Collections.Generic;
using UnityEngine;
using SFH;
using UnityEngine.UI;

public class ResearchCardsManager : MonoSingleton<ResearchCardsManager> {
    [SerializeField] private List<ResearchSO> allResearchCard;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsHolder;
    [SerializeField] private AudioQuerySO winSound;
    [SerializeField] private Image researchRadialImage;
    private ResearchCard currentCard;
    private List<ResearchCard> cardsInPanel;
    private int researchShowed = 0;
    private void Start() {
        //create first 3 research cards
        cardsInPanel = new List<ResearchCard>();
        for (int i = 0; i < 3; i++) {
            GameObject card = Instantiate(cardPrefab);
            card.GetComponent<RectTransform>().SetParent(cardsHolder, false);
            card.GetComponent<ResearchCard>().Init(allResearchCard[i]);
            cardsInPanel.Add(card.GetComponent<ResearchCard>());
            researchShowed += 1;
        }
        researchRadialImage.fillAmount = 0;

    }
    public void ChangeActiveClickedCard(ResearchCard myCard) {
        for (int i = 0; i < cardsInPanel.Count; i++) {
            if (cardsInPanel[i].researchData != myCard.researchData) {
                cardsInPanel[i].ChangeToggleActive(false);
            } else {
                if (cardsInPanel[i].toggled) {
                    currentCard = null;
                } else {
                    currentCard = cardsInPanel[i];
                }
                cardsInPanel[i].ChangeToggleActive(!cardsInPanel[i].toggled);
            }
        }
    }
    public void CompleteResearch(ResearchCard card) {
        //присваиваем бонусы
        for (int i = 0; i < card.researchData.bonuses.Count; i++) {
            switch (card.researchData.bonuses[i].type) {
                case Research.BonusType.OpenUnit:
                    BottomInterfaceManager.inst.AddNewUnit(card.researchData.bonuses[i].unitData);
                    break;
                case Research.BonusType.OpenBuilding:
                    BottomInterfaceManager.inst.AddNewBuilding(card.researchData.bonuses[i].buildingData);
                    break;
                case Research.BonusType.PlusBonus:
                    BonusStorage.inst.addBonuses[card.researchData.bonuses[i].plusBonusCode] += card.researchData.bonuses[i].plusBonus;
                    ManaManager.inst.UpdateUI();
                    break;
                case Research.BonusType.MultiBonus:
                    BonusStorage.inst.multiBonuses[card.researchData.bonuses[i].multiBonusCode] += card.researchData.bonuses[i].multiBonus;
                    break;
            }
        }
        //можно какие-то эффекты изучиения добавить
        AudioManager.inst.PlayQuery(winSound);

        GameObject c = card.gameObject;
        cardsInPanel.Remove(card);
        Destroy(c);

        AddNewResearch();
        if(cardsInPanel.Count != 0) ChangeActiveClickedCard(cardsInPanel[0]);
    }
    public void AddNewResearch() {
        if (cardsInPanel.Count == 3) {
            print("Кто-то попытался добавить исследования при максимальных 3 штук >:(");
        } else {
            if (researchShowed != allResearchCard.Count) {
                GameObject card = Instantiate(cardPrefab);
                card.GetComponent<RectTransform>().SetParent(cardsHolder, false);
                card.GetComponent<ResearchCard>().Init(allResearchCard[researchShowed]);
                cardsInPanel.Add(card.GetComponent<ResearchCard>());
                researchShowed += 1;
            }

        }
    }
    public void AddScienceInCurrentCard() {
        if (currentCard == null) {
            print("Кто-то попытался изучить того, чего нет >:(");
        } else {
            currentCard.AddScience(1);
            researchRadialImage.fillAmount = currentCard.GetProgressPercent();
        }
    }
    public bool TryScience() {
        return currentCard != null;
    }
}