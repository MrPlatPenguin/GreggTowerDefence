using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Catapult : Mortar
{

    [Header("Wheels")]
    [SerializeField] SpriteAnimator wheelAnimation;

    float rotationThisFrame;

    protected override void Update()
    {
        base.Update();

        if (wheelAnimation != null && attackTarget == null)
            wheelAnimation.QueueAnimation("Idle Wheels");
    }


    protected override void RotateTurret(Enemy target)
    {
        Vector3 prevForward = rotatingPart.up;

        base.RotateTurret(target);
        Physics.SyncTransforms();
        rotationThisFrame = Vector3.SignedAngle(prevForward, rotatingPart.up, Vector3.forward);
        string animationName;

        if (rotationThisFrame > 0f)
            animationName = "Turn Right";
        else if (rotationThisFrame < 0f)
            animationName = "Turn Left";
        else
            animationName = "Idle Wheels";

        if (wheelAnimation != null && wheelAnimation.GetCurrentAnimationName() != animationName)
            wheelAnimation.SetAnimation(animationName);
    }
}
