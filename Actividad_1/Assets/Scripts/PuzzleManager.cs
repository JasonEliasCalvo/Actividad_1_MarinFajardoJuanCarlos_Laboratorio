using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    [Header("Estado de la Misión")]
    [SerializeField] private int totalGearsRequired = 3;
    private int currentGearsCollected = 0;
    private bool stationCalibrated = false;
    private bool coreStabilized = false;

    [Header("Referencias de la Escena")]
    public GameObject escapeDoorCollider;
    public ParticleSystem coreParticles;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // Llamado por el evento OnInteract de los engranajes recolectables en Sala 1
    public void CollectGear()
    {
        currentGearsCollected++;
        Debug.Log($"Engranaje recolectado: {currentGearsCollected}/{totalGearsRequired}");

        if (currentGearsCollected >= totalGearsRequired)
        {
            if (UIManager.instance != null)
                UIManager.instance.ShowInteractPanel(false); // Feedback visual rápido
        }
    }

    // Llamado por el evento OnInteract al resolver la consola de la Sala 2
    public void CalibrateStation()
    {
        stationCalibrated = true;
        Debug.Log("Estación Maestra Calibrada Correctamente.");
    }

    // Llamado por el evento OnInteract del panel del Núcleo de Sabiduría (Sala 3)
    public void TryStabilizeCore()
    {
        if (currentGearsCollected >= totalGearsRequired && stationCalibrated)
        {
            coreStabilized = true;
            if (coreParticles != null)
            {
                var main = coreParticles.main;
                main.startColor = Color.cyan; // El núcleo cambia de color peligroso (naranja) a estable (azul)
            }

            // Abrir salida
            if (escapeDoorCollider != null)
                escapeDoorCollider.SetActive(false);

            if (UIManager.instance != null)
                UIManager.instance.ShowConfirmPanelOn("¡Núcleo Estabilizado! La puerta de escape está abierta. ¡CORRE!");
        }
        else
        {
            // Si el jugador intenta activarlo sin cumplir los requisitos
            string missingConfig = "";
            if (currentGearsCollected < totalGearsRequired) missingConfig += "- Faltan Engranajes de Inteligencia.\n";
            if (!stationCalibrated) missingConfig += "- Estación Maestra Descalibrada.\n";

            if (UIManager.instance != null)
                UIManager.instance.ShowWarningPanelOn($"Acceso Denegado:\n{missingConfig}");
        }
    }

    // Llamado por el Trigger final al cruzar la puerta de salida
    public void EscapeSuccess()
    {
        if (!coreStabilized) return;

        GameManager.instance.GameEnd();
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowConfirmPanelOn("¡VICTORIA!\nHas escapado de Caprine Labs a salvo.");
            UIManager.instance.ShowCursor(true);
        }
    }
}