using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopKeepar : MonoBehaviour
{
    public int count;
    public List<AnimationClip> items;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            count++;
            if(count > items.Count)
            {
                count = 0;
            }
                animator.Play(items[count % items.Count].name);
        }
    }
}
