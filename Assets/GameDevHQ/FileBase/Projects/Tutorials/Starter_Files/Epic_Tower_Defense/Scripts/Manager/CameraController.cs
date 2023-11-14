using UnityEngine;
using UnityEngine.InputSystem;


namespace MetroMayhem.Manager
{

    public class CameraController : MonoSingleton<CameraController>
    {
        private Camera _camera;
        private MetroMayhemInputSystem _input;
        private float _cameraControlSpeed = 1f;

        private void OnEnable()
        {
            _input = new MetroMayhemInputSystem();
            _input.Camera.Enable();
            _camera =  GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            CameraMovement();
            CameraZoom();
        }

        private void CameraMovement()
        {
            Vector3 currentPosition = this.transform.position;
            float newCameraX = currentPosition.x + _input.Camera.Movement.ReadValue<Vector2>().y * 0.333f * _cameraControlSpeed;
            float newCameraZ = currentPosition.z - _input.Camera.Movement.ReadValue<Vector2>().x * 0.333f * _cameraControlSpeed;
            // Clamp the camera position to the bounds of the map.
            this.transform.position = new Vector3(Mathf.Clamp(newCameraX, -60, -22), currentPosition.y,
                Mathf.Clamp(newCameraZ, -14, 8));
        }

        private void CameraZoom()
        {
            _camera.fieldOfView += _input.Camera.Zoom.ReadValue<float>() *0.1f  * _cameraControlSpeed;
            if (_camera.fieldOfView < 10)
                _camera.fieldOfView = 10;
            if (_camera.fieldOfView > 40)
                _camera.fieldOfView = 40;
        }
        
        public float GetCameraControlSpeed() {
            return _cameraControlSpeed;
        }

        public void SetCameraSpeed(float cameraSpeed) {
            _cameraControlSpeed = cameraSpeed;
        }

        private void OnDisable() {
            _input.Camera.Disable();
        }
    }
}
