using UnityEngine;

namespace com.game
{
    public class GhostOrb : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float speedMultiplier = 1f;
        private Vector3 currentTargetPos;
        void Update()
        {
            MoveTargetPos();
        }
        public void SetNewDestination(Vector3 newPos)
        {
            currentTargetPos = newPos;
        }
        private void MoveTargetPos()
        {
            Vector3 posToMove = currentTargetPos;
            transform.position = Vector3.Lerp(transform.position, posToMove, Time.deltaTime * movementSpeed * speedMultiplier);
        }
    }
}
