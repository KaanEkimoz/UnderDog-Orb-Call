using UnityEngine;

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
        if (canInteract && Input.GetKeyDown(KeyCode.H))
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
