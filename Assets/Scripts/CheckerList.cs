using UnityEngine;

public class CheckerList : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public void TransitionList()
    {
        if (animator.GetBool("Opened"))
        {
            animator.SetBool("Opened", false);
        }
        else
        {
            animator.SetBool("Opened", true);
        }

    }
    
    public void TransitionJoyStick()
    {
        if (animator.GetBool("Opened"))
        {
            animator.SetBool("Opened", false);
        }
        else
        {
            animator.SetBool("Opened", true);
        }
    }
}
