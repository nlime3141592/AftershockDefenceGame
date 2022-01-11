using UnityEngine;

using DefenceGameSystem.OS.Kernel;

namespace DefenceGameSystem.OS.API
{
    public class CameraController : MonoBehaviour
    {
        public float minX;
        public float maxX;
        public float speed;
        private Camera cam;
        private Vector3 direction;
        private Vector3 position;

        private void Awake()
        {
            cam = GetComponent<Camera>();

            direction = Vector3.zero;
            position = cam.transform.position;
        }

        private void Update()
        {
            if(
                (InputModule.GetKey(KeyType.Button000) ^ InputModule.GetKey(KeyType.Button001)) ||
                (Input.GetKey(KeyCode.LeftArrow) ^ Input.GetKey(KeyCode.RightArrow))
            )
            {
                direction.x = Time.deltaTime * speed * Mathf.Clamp((float)InputModule.GetAxis("Camera Horizontal") + (float)Input.GetAxisRaw("Horizontal"), -1, 1);
                position.x = Mathf.Clamp(position.x + direction.x, minX, maxX);

                transform.position = position;
            }
        }
    }
}