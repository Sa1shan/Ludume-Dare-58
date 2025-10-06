using UnityEngine;

public class QWEGame : MonoBehaviour
{
    [Header("Gameplay")]
    public int targetPoints = 15;
    public float baseTimeout = 2f;
    public float minTimeout = 0.3f;
    public float timeoutDecreasePerPoint = 0.12f;

    [Header("Penalties")]
    public int scorePenaltyOnWrong = 1;
    public float timePenaltySeconds = 3f;

    [Header("Timer")]
    public bool useLevelTimer = true;
    public float levelTimeSeconds = 60f;

    [Header("References")]
    public RectTransform playArea;
    [Tooltip("Сюда положи 3 разных префаба квадратов: Q, W, E.")]
    public GameObject[] squarePrefabs;

    [Header("Audio (optional)")]
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [HideInInspector] public int currentScore;
    [HideInInspector] public float currentTimeout;

    private GameObject currentSquare;
    private AudioSource audioSource;
    private float remainingTime;
    private bool running = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        ResetGame();
    }

    public void ResetGame()
    {
        currentScore = 0;
        remainingTime = levelTimeSeconds;
        currentTimeout = baseTimeout;
        running = false;

        if (currentSquare != null) Destroy(currentSquare);
        UIManager.Instance.UpdateScore(currentScore);
        UIManager.Instance.UpdateTimer(remainingTime);
    }

    public void StartGame()
    {
        ResetGame();
        UIManager.Instance.ShowGameplayUI(true);
        running = true;
        SpawnNext();
    }

    public void RestartGame()
    {
        ResetGame();
        StartGame();
    }

    void Update()
    {
        if (!running) return;

        if (useLevelTimer)
        {
            remainingTime -= Time.deltaTime;
            UIManager.Instance.UpdateTimer(Mathf.Max(0, remainingTime));
            if (remainingTime <= 0f)
            {
                EndGame(false);
                return;
            }
        }

        if (currentSquare != null)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E))
            {
                var ctrl = currentSquare.GetComponent<SquareController>();
                if (ctrl == null) return;

                KeyCode pressed = KeyCode.None;
                if (Input.GetKeyDown(KeyCode.Q)) pressed = KeyCode.Q;
                if (Input.GetKeyDown(KeyCode.W)) pressed = KeyCode.W;
                if (Input.GetKeyDown(KeyCode.E)) pressed = KeyCode.E;

                if (ctrl.IsCorrectKey(pressed))
                {
                    OnCorrect();
                }
                else
                {
                    OnWrong();
                }

                Destroy(currentSquare);
                currentSquare = null;

                if (running)
                    SpawnNext();
            }
        }
    }

    private void OnCorrect()
    {
        currentScore++;
        UpdateTimeout();
        UIManager.Instance.UpdateScore(currentScore);
        if (correctSound) audioSource.PlayOneShot(correctSound);

        if (currentScore >= targetPoints)
            EndGame(true);
    }

    private void OnWrong()
    {
        currentScore = Mathf.Max(0, currentScore - scorePenaltyOnWrong);
        if (useLevelTimer) remainingTime -= timePenaltySeconds;

        UIManager.Instance.UpdateScore(currentScore);
        UIManager.Instance.UpdateTimer(Mathf.Max(0, remainingTime));
        UIManager.Instance.ShowPenaltyFeedback(-scorePenaltyOnWrong, -timePenaltySeconds);

        if (wrongSound) audioSource.PlayOneShot(wrongSound);

        if (useLevelTimer && remainingTime <= 0f)
            EndGame(false);
    }

    private void UpdateTimeout()
    {
        currentTimeout = Mathf.Max(minTimeout, baseTimeout - currentScore * timeoutDecreasePerPoint);
    }

    public void SpawnNext()
    {
        if (squarePrefabs == null || squarePrefabs.Length == 0 || playArea == null) return;

        int index = Random.Range(0, squarePrefabs.Length);
        GameObject prefab = squarePrefabs[index];
        currentSquare = Instantiate(prefab, playArea);

        var ctrl = currentSquare.GetComponent<SquareController>();
        if (ctrl != null)
            ctrl.Initialize(this, currentTimeout);

        RectTransform rt = currentSquare.GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector2 size = playArea.rect.size;
            float x = Random.Range(-size.x / 2f + rt.rect.width / 2f, size.x / 2f - rt.rect.width / 2f);
            float y = Random.Range(-size.y / 2f + rt.rect.height / 2f, size.y / 2f - rt.rect.height / 2f);
            rt.anchoredPosition = new Vector2(x, y);
        }
    }

    public void OnSquareTimedOut(GameObject square)
    {
        if (!running) return;
        if (currentSquare == square)
        {
            Destroy(currentSquare);
            currentSquare = null;
            SpawnNext();
        }
    }

    private void EndGame(bool won)
    {
        running = false;
        if (currentSquare != null) Destroy(currentSquare);
        UIManager.Instance.ShowEnd(won);
    }
}
