using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelText;
    private int levelNumber = 1; 
    private int score = 0; 
    private int rows = 2; 
    private int columns = 2; 
    private int maxPairs = 10; 
    private int totalPairs; 
    private int matchedPairs = 0; 
    private List<Sprite> cardPairs;

    private Card firstSelected, secondSelected;

    private void Start()
    {
        if (PlayerPrefs.GetInt("LoadGame") == 1)
            LoadProgress();

        UpdateUI();
        PrepareSprites();
        InstantiateCards();
    }
    private void PrepareSprites()
    {
        totalPairs = (rows * columns) / 2; // Calculate pairs needed for current grid
        matchedPairs = 0; // Reset matched pairs for the new grid

        cardPairs = new List<Sprite>();
        List<Sprite> selectedSprites = new List<Sprite>();

        // Randomly select the required number of sprites
        while (selectedSprites.Count < totalPairs)
        {
            Sprite randomSprite = cardSprites[Random.Range(0, cardSprites.Length)];
            if (!selectedSprites.Contains(randomSprite))
            {
                selectedSprites.Add(randomSprite);
            }
        }

        for(int i=0; i<selectedSprites.Count; i++)
        {
            cardPairs.Add(selectedSprites[i]);
            cardPairs.Add(selectedSprites[i]);
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
        GridLayoutGroup gridLayout = cardContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
        }

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
            SoundManager.Instance.PlaymatchSound();
            matchedPairs++; 
            score += 10; 
            UpdateUI();

           
            if (matchedPairs == totalPairs)
            {
                //score += 50; // Add bonus points for completing the level
                ShowLevelCompleted();
                IncreaseDifficulty();
                ResetBoard();
            }

            a.SetOpacity();
            b.SetOpacity();
        }
        else
        {
            SoundManager.Instance.PlaymissMatchSound();
            a.Hide();
            b.Hide();
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("Level", levelNumber);
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("Rows", rows);
        PlayerPrefs.SetInt("Columns", columns);
        PlayerPrefs.Save();
        Debug.Log("Game Progress Saved!");
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            levelNumber = PlayerPrefs.GetInt("Level");
            score = PlayerPrefs.GetInt("Score");
            rows = PlayerPrefs.GetInt("Rows");
            columns = PlayerPrefs.GetInt("Columns");
            Debug.Log("Game Progress Loaded!");
        }
        else
        {
            Debug.Log("No Saved Progress Found. Starting New Game.");
        }
    }

    private void UpdateUI()
    {
        levelText.text = $"Level: {levelNumber}"; 
        scoreText.text = $"Score: {score}"; 
    }

    private void IncreaseDifficulty()
    {
        levelNumber++; // Increment the level number

        // Increase grid size every 2 levels
        if (levelNumber % 2 == 0)
        {
            if (rows * columns / 2 < maxPairs) 
            {
                if (columns > rows) rows++;
                else columns++;
            }
        }

        SaveProgress(); 
        UpdateUI(); 
    }

    private void ShowLevelCompleted()
    {
        Debug.Log($"Level {levelNumber} Completed!");
    }

    private void ResetBoard()
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject); 
        }

        PrepareSprites(); 
        InstantiateCards(); 
    }
}
