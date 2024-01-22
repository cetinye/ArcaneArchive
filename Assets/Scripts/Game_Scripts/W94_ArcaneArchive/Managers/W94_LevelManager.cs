using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class W94_LevelManager : MonoBehaviour
{
    [Header("Score Variables")]
    public int score;
    public int totalMoves;
    public int totalBooksCleared;

    [Header("Level Variables")]
    public int levelIndex;
    [SerializeField] private W94_LevelSO levelSO;
    [SerializeField] private List<W94_LevelSO> levelSOList = new List<W94_LevelSO>();

    [Header("Level Objects")]
    [SerializeField] private List<W94_Shelf> frontShelves = new List<W94_Shelf>();
    [SerializeField] private List<RectTransform> frontSlots = new List<RectTransform>();
    [SerializeField] private List<RectTransform> backSlots = new List<RectTransform>();
    [SerializeField] private List<Sprite> bookImages = new List<Sprite>();
    [SerializeField] private List<GameObject> spawnedBooks = new List<GameObject>();

    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private Color grayColor;
    [SerializeField] private float endAnimParticleInterval;

    private int bookSpriteCount = 0;
    private int bookSpriteIndex = 0;

    public void GetLevel()
    {
        levelSO = levelSOList[levelIndex];
    }

    public W94_LevelSO GetLevelSO()
    {
        levelSO = levelSOList[levelIndex];
        return levelSO;
    }

    public void StartLevel()
    {
        W94_AudioManager.instance.Play("Background");

        GetLevel();
        SpawnBooks();
    }

    public void SpawnBooks()
    {
        ResetLevelVariables();

        List<RectTransform> frontSlotPositions = new List<RectTransform>(frontSlots);
        List<RectTransform> backSlotPositions = new List<RectTransform>(backSlots);
        frontSlotPositions.Shuffle();
        GameObject spawnedBook;
        int index;

        //spawn books in front shelf
        for (int i = 0; i < levelSO.frontShelfBookAmount; i++)
        {
            index = Random.Range(0, frontSlotPositions.Count);
            spawnedBook = Instantiate(bookPrefab, frontSlotPositions[index].position, frontSlotPositions[index].rotation, frontSlotPositions[index]);

            spawnedBook.GetComponent<Image>().sprite = bookImages[bookSpriteIndex];
            bookSpriteCount++;

            //if spawned 3 books with the same texture, change sprite
            if (bookSpriteCount == 3)
            {
                bookSpriteIndex++;
                bookSpriteCount = 0;
            }

            spawnedBooks.Add(spawnedBook);
            frontSlotPositions.RemoveAt(index);
        }

        //spawn books in back shelf
        for (int i = 0; i < levelSO.backShelfBookAmount; i++)
        {
            index = Random.Range(0, backSlotPositions.Count);
            spawnedBook = Instantiate(bookPrefab, backSlotPositions[index].position, backSlotPositions[index].rotation, backSlotPositions[index]);

            spawnedBook.GetComponent<Image>().sprite = bookImages[bookSpriteIndex];
            spawnedBook.GetComponent<Image>().color = grayColor;
            spawnedBook.GetComponent<CanvasGroup>().interactable = false;
            bookSpriteCount++;

            //if spawned 3 books with the same texture, change sprite
            if (bookSpriteCount == 3)
            {
                bookSpriteIndex++;
                bookSpriteCount = 0;
            };

            spawnedBooks.Add(spawnedBook);
            backSlotPositions.RemoveAt(index);
        }

        W94_GameManager.instance.state = W94_GameManager.GameState.playing;
    }

    private void ResetLevelVariables()
    {
        DestroyBooks();

        //reset book sprite index and count
        //shuffle image list for different books each generation
        bookSpriteCount = 0;
        bookSpriteIndex = 0;
        bookImages.Shuffle();

        totalBooksCleared = 0;
        totalMoves = 0;
        score = 0;
    }

    public void RemoveBook(GameObject bookToRemove)
    {
        spawnedBooks.Remove(bookToRemove);
        totalBooksCleared++;
    }

    public void IncreaseTotalMovesCounter()
    {
        totalMoves++;
    }

    public int GetActiveBookCount()
    {
        return spawnedBooks.Count;
    }

    private void DestroyBooks()
    {
        for (int i = 0; i < spawnedBooks.Count; i++)
        {
            Destroy(spawnedBooks[i]);
        }

        spawnedBooks.Clear();
    }

    public bool isShelvesFull()
    {
        for (int i = 0; i < frontSlots.Count; i++)
        {
            if (frontSlots[i].childCount == 0)
                return false;
        }

        return true;
    }

    public void StartEndAnim()
    {
        StartCoroutine(EndAnimRoutine());
    }

    IEnumerator EndAnimRoutine()
    {
        for (int i = 0; i < frontShelves.Count; i++)
        {
            frontShelves[i].PlayParticles();
            yield return new WaitForSeconds(endAnimParticleInterval);
        }

        W94_GameManager.instance.Finish();
    }
}
