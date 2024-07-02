using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : SingleShot
{
    protected override void AnimationQueue()
    {
        animator.ClearQueue();
        animator.SetAnimation("Fire");
        animator.QueueAnimation("Idle");
    }
}
