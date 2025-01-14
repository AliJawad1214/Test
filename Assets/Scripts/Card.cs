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
    public float flipDuration = 0.3f;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetFrontSide(Sprite sp)
    {
        frontSide = sp;
    }

    public void Show()
    {
        StartCoroutine(FlipForward());
        cardImage.sprite = frontSide;
        isSlected = true;
    }

    public void Hide()
    {
        StartCoroutine(FlipBack());
        cardImage.sprite = backSide;
        isSlected = false;
    }

    public void OnCardClick()
    {
        cardManager.SetSelected(this);
    }

    private IEnumerator FlipForward()
    {
        for (float t = 0; t < flipDuration; t += Time.deltaTime)
        {
            float rotationY = Mathf.Lerp(0, 180, t / flipDuration);
            cardImage.rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
            yield return null;
        }
        cardImage.rectTransform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    private IEnumerator FlipBack()
    {
        for (float t = 0; t < flipDuration; t += Time.deltaTime)
        {
            float rotationY = Mathf.Lerp(180, 0, t / flipDuration);
            cardImage.rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
            yield return null;
        }
        cardImage.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void SetOpacity()
    {
        Color color = image.color;
        color.a = 0.1f;
        image.color = color;
    }
}
