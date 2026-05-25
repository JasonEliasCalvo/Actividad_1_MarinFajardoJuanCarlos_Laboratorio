using System;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    [Header("Estado de la Misión")]
    [SerializeField] private int totalTanksToRestore = 3; // En image_f781a6.png se ven 3 rojos y 2 azules de base
    private int currentTanksRestored = 0;

    public UnityEvent TankRestored;
    public UnityEvent StationCalibrated;
    public UnityEvent CoreStabilized;

    private bool stationCalibrated = false;
    private bool coreStabilized = false;

    [Header("Tarjet")]
    public GameObject cardPrefab;
    public GameObject playerPrefab;
    public GameObject cardInteractCollider;

    [Header("Referencias de la Escena")]
    public ParticleSystem coreParticles;
    [SerializeField] private Renderer coreRenderer; // El MeshRenderer del tanque completo
    [SerializeField] private Material blueMaterial;       // El material que quieres aplicar
    [SerializeField] private int materialIndexToReplace = 1;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void Update()
    {
        if (cardPrefab != null && playerPrefab != null && cardInteractCollider != null && currentTanksRestored >= totalTanksToRestore)
        {
            if (cardPrefab.activeInHierarchy && cardPrefab.transform.IsChildOf(playerPrefab.transform))
            {
                cardInteractCollider.SetActive(true);
            }
            else
            {
                cardInteractCollider.SetActive(false);
            }
        }
    }

    public void RestoreTank()
    {
        currentTanksRestored++;
        Debug.Log($"Tanque restaurado: {currentTanksRestored}/{totalTanksToRestore}");

        if (currentTanksRestored >= totalTanksToRestore)
        {
            if (UIManager.instance != null)
                UIManager.instance.ShowConfirmPanelOn("¡Tanques restaurados! La tarjeta de calibrado ha aparecido en la consola del Laboratorio.");

            TankRestored?.Invoke();
            if (UIManager.instance != null)
                UIManager.instance.ShowInteractPanel(false);
        }
    }

    // Llamado por el evento OnInteract al resolver la consola de la Sala 2
    public void CalibrateStation()
    {
        stationCalibrated = true;

        if (UIManager.instance != null)
            UIManager.instance.ShowConfirmPanelOn("¡Estacion calibrada puedes estabilizar el nucleo!");

        Debug.Log("Estación Maestra Calibrada Correctamente.");
    }

    // Llamado por el evento OnInteract del panel del Núcleo de Sabiduría (Sala 3)
    public void TryStabilizeCore()
    {
        if (currentTanksRestored >= totalTanksToRestore && stationCalibrated)
        {
            coreStabilized = true;
            if (coreParticles != null)
            {
                var main = coreParticles.main;
                main.startColor = Color.cyan;
            }

            if (coreRenderer != null && blueMaterial != null)
            {
                Material[] currentMaterials = coreRenderer.materials;

                if (materialIndexToReplace >= 0 && materialIndexToReplace < currentMaterials.Length)
                {
                    currentMaterials[materialIndexToReplace] = blueMaterial;
                    coreRenderer.materials = currentMaterials;
                    Debug.Log($"Material del núcleo estabilizado reemplazado correctamente en el índice {materialIndexToReplace}.");
                }
                else
                {
                    Debug.LogError($"El índice {materialIndexToReplace} está fuera de los límites en el MeshRenderer de {gameObject.name}. Comprueba cuántos materiales tiene asignados.");
                }
            }

            if (UIManager.instance != null)
                UIManager.instance.ShowConfirmPanelOn("¡Núcleo Estabilizado! La puerta de escape de Caprine Labs está abierta. ¡CORRE!");

            CoreStabilized?.Invoke();
        }
        else
        {
            string missingConfig = "";
            if (currentTanksRestored < totalTanksToRestore) missingConfig += $"- Faltan restaurar {totalTanksToRestore - currentTanksRestored} Tanques de Cultivo.\n";
            if (!stationCalibrated) missingConfig += "- Estación Maestra Descalibrada.\n";

            if (UIManager.instance != null)
                UIManager.instance.ShowWarningPanelOn($"Acceso Denegado:\n{missingConfig}");
        }
    }

    public void EscapeSuccess()
    {
        GameManager.instance.GameEnd();
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowConfirmPanelOn("¡VICTORIA!\nHas escapado del laboratorio a salvo.");
            UIManager.instance.ShowCursor(true);
        }
    }
}