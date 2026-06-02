using UnityEngine;

public class AnimateHammer : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void playAnim()
    {   
        animator.SetTrigger("swing");
    }

void Update()
    {
        
    }
}

