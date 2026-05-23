using UnityEngine;
using TMPro;

public class GameplayTimer : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [SerializeField] private float timeRemaining = 300f; // 5 Minutos para la Jam
    [SerializeField] private TextMeshProUGUI timerText; // Arrastra un TextMeshPro aquí

    private bool isTimerRunning = false;

    private void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.eventGameStart += StartTimer;
            GameManager.instance.eventGameEnd += StopTimer;
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.eventGameStart -= StartTimer;
            GameManager.instance.eventGameEnd -= StopTimer;
        }
    }

    private void StartTimer() => isTimerRunning = true;
    private void StopTimer() => isTimerRunning = false;

    private void Update()
    {
        if (!isTimerRunning) return;

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

        // Feedback visual: Si queda menos de 1 minuto, poner texto en rojo
        if (timeToDisplay <= 60f)
        {
            timerText.color = Color.red;
        }
    }

    private void TriggerDefeat()
    {
        // Bloquea juego y manda el panel de Warning o un panel custom de Game Over
        GameManager.instance.GameEnd();
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowWarningPanelOn("¡EL NÚCLEO HA COLAPSADO!\nInstalación destruida.");
            UIManager.instance.ShowCursor(true);
        }
    }
}