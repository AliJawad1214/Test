using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Sprite[] cardSprites;
    private List<Sprite> cardPairs;

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
            Instantiate(cardPrefab, cardContainer);
        }
    }
}
