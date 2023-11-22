using UnityEngine;

public class Target : MonoBehaviour
{

    [SerializeField] private Animator myAnimationController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimationController.SetBool("playZipline", true);
        }

    }
    
}
