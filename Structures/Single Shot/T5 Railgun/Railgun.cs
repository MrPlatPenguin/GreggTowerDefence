using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : SingleShot
{
    [SerializeField] SpriteAnimator lightAnimator;
    [SerializeField] SpriteAnimator projAnimator;

    protected override void AnimationQueue()
    {
        animator.ClearQueue();
        animator.SetAnimation("Gun_Unloaded");
        animator.QueueAnimation("Gun_Charging");
        animator.QueueAnimation("Gun_Loaded");

        lightAnimator.SetAnimation("Lights_Uncharged");
        lightAnimator.QueueAnimation("Lights_Charging");
        lightAnimator.QueueAnimation("Ligts_Charged");

        projAnimator.SetAnimation("Proj_Fired");
        projAnimator.QueueAnimation("Proj_Reload");
        projAnimator.QueueAnimation("Proj_Idle");
    }
}
