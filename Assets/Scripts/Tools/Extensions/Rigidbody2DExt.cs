

using System.Linq;
using Platformer.Tools;
using UnityEngine;

public static class Rigidbody2DExt
{
    public const float GroundedDegreesThreshold = 60f;

    public static bool TryGetClosestHit(this Rigidbody2D rigidbody, Vector2 direction, out RaycastHit2D hit)
    {
        var hits = ArrayCache<RaycastHit2D>.Get();
        int hitCount = rigidbody.Cast(Vector2.down, ArrayCache<RaycastHit2D>.Get());

        hit = hits.Take(hitCount).MinBy(h => h.distance);
        return hitCount != 0;
    }

    public static bool IsGrounded(this Rigidbody2D rigidbody, float degrees = GroundedDegreesThreshold)
    {
        return rigidbody.IsContacted(Vector2.up, degrees);
    }

    public static bool IsCeiled(this Rigidbody2D rigidbody, float degrees = GroundedDegreesThreshold)
    {
        return rigidbody.IsContacted(Vector2.down, degrees);
    }

    public static bool IsContacted(this Rigidbody2D rigidbody, Vector2 reference, float degrees = GroundedDegreesThreshold)
    {
        var contacts = ArrayCache<ContactPoint2D>.Get();
        int contactCount = rigidbody.GetContacts(contacts);

        for (int i = 0; i < contactCount; i++)
        {
            var contact = contacts[i];
            var angle = Vector2.Angle(reference, contact.normal);
            if (angle < degrees)
                return true;
        }

        return false;
    }
}