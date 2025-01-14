using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    public Sprite backSide, frontSide;

    public bool isSlected;
    public CardManager cardManager;

    public void SetFrontSide(Sprite sp)
    {
        frontSide = sp;
    }

    public void Show()
    {
        cardImage.sprite = frontSide;
        isSlected = true;
    }

    public void Hide()
    {
        cardImage.sprite = backSide;
        isSlected = false;
    }

    public void OnCardClick()
    {
        cardManager.SetSelected(this);
    }
}
