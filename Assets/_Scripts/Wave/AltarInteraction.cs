using UnityEngine;
using UnityEngine.InputSystem;

public class AltarInteraction : MonoBehaviour
{
    [SerializeField] private bool canInteract = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    void Update()
    {
        if (canInteract && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    void Interact()
    {
        //GameManager.Instance.SetState(GameState.BetweenWaves);
        Debug.Log("shop acildi");
    }
}
