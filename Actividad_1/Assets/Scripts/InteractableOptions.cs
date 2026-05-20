using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Flags]
public enum InteractionType
{
    None = 0,
    InvokeEvent = 1 << 0,
}

public class InteractableOptions : MonoBehaviour
{
    [SerializeField] private InteractionType interactionTypes;
    [SerializeField] private bool justOneInteraction = false;
    [SerializeField] private bool canInteract = true;

    private PlayerController player;
    public UnityEvent onInteract;
    public UnityEvent endInteract;

    [Header("Configuración de Interacción")]
    public GameObject selectecObject;
    public string itemName;
    public int ID;

    [Tooltip("Si está en FALSE, el objeto no será destruido al seleccionar")]
    public bool destroyOnSelect = true;

    public InteractionType InteractionTypes => interactionTypes;
    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    public void TryInteract()
    {
        if (!canInteract) return;

        if (justOneInteraction)
        {
            canInteract = false;
        }
        ExecuteInteraction();
    }

    public void EndInteract()
    {
        EndInteraction();
    }

    public IEnumerable<InteractionType> GetActiveFlags()
    {
        foreach (InteractionType flag in System.Enum.GetValues(typeof(InteractionType)))
        {
            if (flag != InteractionType.None && interactionTypes.HasFlag(flag))
                yield return flag;
        }
    }

    private void ExecuteInteraction()
    {
        foreach (var type in GetActiveFlags())
        {
            switch (type)
            {
                case InteractionType.InvokeEvent:
                    onInteract?.Invoke();
                    break;
            }
        }


        if (destroyOnSelect)
        {
            Destroy(gameObject);
        }
    }

    private void EndInteraction()
    {
        foreach (var type in GetActiveFlags())
        {
            switch (type)
            {
                case InteractionType.InvokeEvent:
                    endInteract?.Invoke();
                    break;
            }
        }
    }

    public void EnableInteraction() => canInteract = true;
    public void DisableInteraction() => canInteract = false;
}
