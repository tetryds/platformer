using UnityEngine;

namespace Platformer.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        PlayerController player;

        public void AssignPlayer(PlayerController player)
        {
            this.player = player;
            player.Teleported += Update;
        }

        private void Update()
        {
            if (player)
                transform.position = player.transform.position;
        }
    }
}