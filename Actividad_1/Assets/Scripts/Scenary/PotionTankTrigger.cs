using UnityEngine;

public class PotionTankTrigger : MonoBehaviour
{
    [Header("Configuración del Filtro")]
    [SerializeField] private string potionTag = "Potion"; // Tag de las pociones

    [Header("Referencias Visuales")]
    [SerializeField] private Renderer tankLiquidRenderer; // El MeshRenderer del tanque completo
    [SerializeField] private Material blueMaterial;       // El material que quieres aplicar
    [Tooltip("El índice del material en el MeshRenderer. 0 es el primero, 1 el segundo, 2 el tercero, etc.")]
    [SerializeField] private int materialIndexToReplace = 2; 

    [Header("Estado")]
    private bool isRestored = false;

    private void OnTriggerEnter(Collider other)
    {
        // Si ya está restaurado o no es una poción, ignoramos
        if (isRestored || !other.CompareTag(potionTag)) return;

        // 1. Validar si el jugador sostiene el objeto para soltarlo de la mano antes de destruirlo
        PlayerController player = PlayerController.instance;
        if (player != null && player.CurrentGrab == other.gameObject)
        {
            player.CurrentGrab = null;
            player.isGrabbed = false;
        }

        // 2. Destruir la poción que chocó
        Destroy(other.gameObject);

        // 3. Cambiar ESPECÍFICAMENTE el material seleccionado
        if (tankLiquidRenderer != null && blueMaterial != null)
        {
            // Obtenemos una copia del array de materiales actual del Renderer
            Material[] currentMaterials = tankLiquidRenderer.materials;

            // Validamos que el índice configurado exista dentro del array para evitar errores de consola
            if (materialIndexToReplace >= 0 && materialIndexToReplace < currentMaterials.Length)
            {
                currentMaterials[materialIndexToReplace] = blueMaterial;
                tankLiquidRenderer.materials = currentMaterials; // Reasignamos el array modificado
            }
            else
            {
                Debug.LogError($"El índice {materialIndexToReplace} está fuera de los límites en el MeshRenderer de {gameObject.name}. Comprueba cuántos materiales tiene asignados.");
            }
        }

        // 4. Registrar el progreso en el PuzzleManager
        isRestored = true;
        if (PuzzleManager.instance != null)
        {
            PuzzleManager.instance.RestoreTank();
        }
    }
}