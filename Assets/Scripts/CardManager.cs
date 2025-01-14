using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private TMP_Text scoreText;
    private int score = 0;
    private List<Sprite> cardPairs;

    private Card firstSelected, secondSelected;

    private void Start()
    {
        PrepareSprites();
        InstantiateCards();
    }
    private void PrepareSprites()
    {
        cardPairs = new List<Sprite>();
       
        for(int i=0; i< cardSprites.Length; i++)
        {
            cardPairs.Add(cardSprites[i]);
            cardPairs.Add(cardSprites[i]);
        }

        ShuffleCards(cardPairs);
    }

    private void ShuffleCards(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }
    }

    private void InstantiateCards()
    {
        for (int i = 0; i < cardPairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, cardContainer);
            card.SetFrontSide(cardPairs[i]);
            card.cardManager = this;
        }
    }

    public void SetSelected(Card card)
    {
        if (card.isSlected == false)
        {
            card.Show();
            if (firstSelected == null)
            {
                firstSelected = card;
                return;
            }

            if (secondSelected == null)
            {
                secondSelected = card;
                StartCoroutine(Matching(firstSelected, secondSelected));
                firstSelected = null;
                secondSelected = null;
            }
        }
    }

    IEnumerator Matching(Card a, Card b)
    {
        yield return new WaitForSeconds(0.5f);
        if (a.frontSide == b.frontSide)
        {
            score += 10;
            scoreText.text = $"Score: {score}";
            a.SetOpacity();
            b.SetOpacity();
        }
        else
        {
            a.Hide();
            b.Hide();
        }
    }
}
