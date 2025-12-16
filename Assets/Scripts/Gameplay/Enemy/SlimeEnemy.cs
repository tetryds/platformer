using Platformer.Mechanics;
using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class SlimeEnemy : AEnemy
    {
        [SerializeField] protected PatrolPath path;
        [SerializeField] Animator animator;
        [SerializeField] float maxSpeed;
        [SerializeField] SpriteRenderer sprite;

        [SerializeField] new Rigidbody2D rigidbody;

        Collider2D[] colliders;

        public override void Initialize()
        {
            colliders = GetComponentsInChildren<Collider2D>();
            if (!rigidbody.TryGetClosestHit(Vector2.down, out RaycastHit2D hit))
                IsAlive = false;

            rigidbody.position = hit.centroid;
        }

        public override void Tick(float deltaTime, float elapsed)
        {
            Vector2 target = path ? path.GetTarget(elapsed * maxSpeed) : (Vector2)transform.position;

            float direction = Mathf.Clamp(target.x - transform.position.x, -1f, 1);

            var velocity = new Vector2(direction * maxSpeed, 0f);

            rigidbody.MovePosition(rigidbody.position + velocity * deltaTime);

            animator.SetBool("grounded", IsGrounded());
            animator.SetFloat("velocityX", Mathf.Abs(direction));

            sprite.flipX = direction > 0f;
        }

        public bool IsGrounded()
        {
            var contacts = ArrayCache<ContactPoint2D>.Get();
            int contactCount = rigidbody.GetContacts(contacts);

            for (int i = 0; i < contactCount; i++)
            {
                var contact = contacts[i];
                var angle = Vector2.Angle(Vector2.up, contact.normal);
                if (angle < 60f)
                    return true;
            }

            return false;
        }

        public override void Despawn()
        {
            animator.SetTrigger("death");
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            Destroy(gameObject, 1f);
        }
    }
}
