using UnityEngine;
using TMPro;

public class GameplayTimer : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [SerializeField] private float timeRemaining = 300f; // 5 Minutos para la Jam
    [SerializeField] private TextMeshProUGUI timerText; // Arrastra un TextMeshPro aquí

    public bool isTimerRunning = false;

    private void StartTimer() => isTimerRunning = true;
    private void StopTimer() => isTimerRunning = false;

    private void Update()
    {
        if(PlayerController.instance == null) return;

        isTimerRunning = PlayerController.instance.movementState;

        if (!isTimerRunning) return;

        if (UIManager.instance != null && UIManager.instance.IsPanelActive()) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            isTimerRunning = false;
            UpdateTimerDisplay(timeRemaining);
            TriggerDefeat();
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeToDisplay <= 60f)
        {
            timerText.color = Color.red;
        }
    }

    private void TriggerDefeat()
    {
        GameManager.instance.GameEnd();
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowWarningPanelOn("¡EL NÚCLEO HA COLAPSADO!\nInstalación destruida.");
            Invoke(nameof(GameEnd), 0.2f);
            UIManager.instance.ShowCursor(true);
        }
    }

    public void GameEnd()
    {
        GameManager.instance.LoadScene(0);
    }
}