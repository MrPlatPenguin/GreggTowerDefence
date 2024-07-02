using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    Vector3 prevPos;
    bool _facingRight;
    public float DISTANCE_BEFORE_FLIP = 0.001f;

    void Awake()
    {
        prevPos = transform.position;
    }

    void Update()
    {
        // Only execute the flip if the distance is greater than the threshold
        if (Mathf.Abs(prevPos.x - transform.position.x) >= DISTANCE_BEFORE_FLIP)
        {
            Vector2 direction = transform.position.x - prevPos.x > 0 ? Vector2.left : Vector2.right;

            if (_facingRight != (direction == Vector2.right))
            {
                // Flip the object by changing its local scale
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                _facingRight = direction == Vector2.right;
            }
        }

        // Update the previous position for the next frame
        prevPos = transform.position;
    }
}