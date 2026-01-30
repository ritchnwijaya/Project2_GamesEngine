using UnityEngine;

public class AnimEventRelay : MonoBehaviour
{
    private PlayerInteraction interaction;

    void Awake()
    {
        // sucht PlayerInteraction irgendwo im Parent/Root (bei dir: root/interactor)
        interaction = GetComponentInParent<PlayerInteraction>();
        if (interaction == null)
            interaction = FindFirstObjectByType<PlayerInteraction>();
    }

    // Diese Methode rufst du per Animation Event auf
    public void DoLandToolAction()
    {
        if (interaction != null)
            interaction.DoLandToolAction();
        else
            Debug.LogWarning("AnimEventRelay: No PlayerInteraction found!");
    }

    // Optional: wenn du später movement lock/unlock machst
    public void EndToolAction()
    {
        // Hier könntest du später z.B. PlayerMovement wieder freigeben
    }
}
