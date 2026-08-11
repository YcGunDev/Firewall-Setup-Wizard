using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public Block A;
    public Block B;
    public void ProcessCollision(int type)
    {
        if (type == 0) //damage
        {
            int healthA = A.health.Value;
            int healthB = B.health.Value;
            A.TakeDamage(healthB);
            B.TakeDamage(healthA);
        }
        else if (type == 1)//bounce -- i might want to rework this later but its fine for now i think
        {
            int polarity = 1;
            if (A.speed > B.speed)//the lower speed should increase, the higher in speed should decrease
            {
                polarity = -1;
            }

            Vector2 resA = (A.speed * A.direction) + (B.speed * B.direction * polarity);
            A.speed = resA.magnitude * 0.9f;
            A.direction = resA.normalized;

            Vector2 resB = (A.speed * A.direction) - (B.speed * B.direction * polarity);
            B.speed = resB.magnitude * 0.9f;
            B.direction = resB.normalized;
        }
        Destroy(this.gameObject);
    }
}
