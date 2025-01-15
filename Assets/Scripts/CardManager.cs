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
    [SerializeField] private TMP_Text timerText;
    private float timer;
    [SerializeField] private float timeLeft = 60;
    private bool isGameOver;
    public GameObject gameOverPanel;


    private Card firstSelected, secondSelected;

    private void Start()
    {
        if (PlayerPrefs.GetInt("LoadGame") == 1)
            LoadProgress();

        UpdateUI();
        PrepareSprites();
        InstantiateCards();
        SetInitialCellSize();
        ResetTimer();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                isGameOver = true;
                gameOverPanel.SetActive(true);
            }
        }
    }

    private void SetInitialCellSize()
    {
        GridLayoutGroup gridLayout = cardContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.cellSize = new Vector2(200, 300);
        }
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

            UpdateCardContainer();
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
                //score += 50; // bonus points for completing the level
                ShowLevelCompleted();
                IncreaseDifficulty();
                ResetBoard();
                StopCoroutine("LoseTime");
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
        if (columns >= 6 && rows >= 6)
        {

        }
        else if (levelNumber % 2 == 0)
        {
            if (columns > rows)
                rows++;
            else
                columns++;
        }

        if ((rows * columns) % 2 != 0)
        {
            if (columns > rows)
                rows++;
            else columns++;
        }
        ResetTimer();
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

    private void UpdateCardContainer()
    {
        GridLayoutGroup gridLayout = cardContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.padding = new RectOffset(30, 30, 50, 50);
            // Calculate available width and height by subtracting spacing
            RectTransform containerRect = cardContainer.GetComponent<RectTransform>();
            float totalSpacingX = gridLayout.spacing.x * (columns - 1);
            float totalSpacingY = gridLayout.spacing.y * (rows - 1);

            float availableWidth = containerRect.rect.width - totalSpacingX - gridLayout.padding.left - gridLayout.padding.right;
            float availableHeight = containerRect.rect.height - totalSpacingY - gridLayout.padding.top - gridLayout.padding.bottom;

            // Calculate cell size proportional to the grid size
            float cellWidth = availableWidth / columns;
            float cellHeight = availableHeight / rows;

            // Maintain aspect ratio of 200x300 for initial size
            float aspectRatio = 200f / 300f;
            if (cellWidth / cellHeight > aspectRatio)
            {
                cellWidth = cellHeight * aspectRatio;
            }
            else
            {
                cellHeight = cellWidth / aspectRatio;
            }

            // Set the calculated cell size
            gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        }
    }

    private void ResetTimer()
    {
        timer = timeLeft + (levelNumber - 1) * 10f;
        isGameOver = false;
    }

    public void RestartLevel()
    {
        gameOverPanel.SetActive(false);
        ResetBoard();
        ResetTimer(); 
        UpdateUI(); 
        isGameOver = false; 
    }

}
