using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactionPrompt = "Press E to interact";

    // Virtual method for interacting with an object
    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    // Optional method for displaying prompts in UI
    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt;
    }
}
