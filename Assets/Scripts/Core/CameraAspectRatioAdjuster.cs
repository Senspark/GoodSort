using UnityEngine;

namespace Core
{
    /// <summary>
    /// Tự động điều chỉnh Camera orthographic size dựa trên aspect ratio của thiết bị
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class CameraAspectRatioAdjuster : MonoBehaviour
    {
        // Hard-coded camera size values
        private const float IPAD_OR_LESS_THAN_15_9_SIZE = 11.5f;
        private const float RATIO_15_9_SIZE = 10f;
        private const float RATIO_16_9_SIZE = 10f;
        private const float RATIO_18_9_SIZE = 11f;
        private const float RATIO_20_9_SIZE = 12f;

        private Camera _camera;
        private float _lastAspectRatio;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (_camera.orthographic == false)
            {
                Debug.LogWarning($"[CameraAspectRatioAdjuster] Camera '{gameObject.name}' is not Orthographic! This script only works with Orthographic cameras.");
            }
        }

        private void Start()
        {
            AdjustCameraSize();
        }

#if UNITY_EDITOR
        private void Update()
        {
            // Trong Editor, kiểm tra liên tục để hỗ trợ thay đổi Game View resolution
            float currentRatio = (float)Screen.width / Screen.height;
            if (Mathf.Abs(currentRatio - _lastAspectRatio) > 0.01f)
            {
                AdjustCameraSize();
            }
        }
#endif

        private void AdjustCameraSize()
        {
            if (_camera == null) return;

            float aspectRatio = (float)Screen.width / Screen.height;
            _lastAspectRatio = aspectRatio;

            float newSize = CalculateCameraSize(aspectRatio, out string deviceType);
            _camera.orthographicSize = newSize;

            Debug.Log($"[CameraAspectRatioAdjuster] Screen: {Screen.width}x{Screen.height}, " +
                      $"Aspect Ratio: {aspectRatio:F3}, Device: {deviceType}, Camera Size: {newSize}");
        }

        private float CalculateCameraSize(float aspectRatio, out string deviceType)
        {
            // Tính toán aspect ratio thresholds
            float ratio15_9 = 15f / 9f;  // ≈ 1.667
            float ratio16_9 = 16f / 9f;  // ≈ 1.778
            float ratio18_9 = 18f / 9f;  // = 2.0
            float ratio20_9 = 20f / 9f;  // ≈ 2.222

            // iPad aspect ratio (4:3 ≈ 1.333)
            float ipadRatio = 4f / 3f;

            // Tolerance cho so sánh float
            const float tolerance = 0.05f;

            // iPad hoặc < 15:9
            if (aspectRatio <= ratio15_9 - tolerance)
            {
                deviceType = aspectRatio <= ipadRatio + tolerance ? "iPad (4:3)" : "< 15:9";
                return IPAD_OR_LESS_THAN_15_9_SIZE;
            }
            // 15:9
            else if (Mathf.Abs(aspectRatio - ratio15_9) < tolerance)
            {
                deviceType = "15:9";
                return RATIO_15_9_SIZE;
            }
            // 16:9
            else if (Mathf.Abs(aspectRatio - ratio16_9) < tolerance)
            {
                deviceType = "16:9";
                return RATIO_16_9_SIZE;
            }
            // 18:9
            else if (Mathf.Abs(aspectRatio - ratio18_9) < tolerance)
            {
                deviceType = "18:9";
                return RATIO_18_9_SIZE;
            }
            // 20:9 hoặc lớn hơn
            else if (aspectRatio >= ratio20_9 - tolerance)
            {
                deviceType = "20:9 or wider";
                return RATIO_20_9_SIZE;
            }
            // Giữa các ratio, interpolate
            else
            {
                // Giữa 15:9 và 16:9
                if (aspectRatio > ratio15_9 && aspectRatio < ratio16_9)
                {
                    deviceType = "Between 15:9 and 16:9";
                    float t = (aspectRatio - ratio15_9) / (ratio16_9 - ratio15_9);
                    return Mathf.Lerp(RATIO_15_9_SIZE, RATIO_16_9_SIZE, t);
                }
                // Giữa 16:9 và 18:9
                else if (aspectRatio > ratio16_9 && aspectRatio < ratio18_9)
                {
                    deviceType = "Between 16:9 and 18:9";
                    float t = (aspectRatio - ratio16_9) / (ratio18_9 - ratio16_9);
                    return Mathf.Lerp(RATIO_16_9_SIZE, RATIO_18_9_SIZE, t);
                }
                // Giữa 18:9 và 20:9
                else if (aspectRatio > ratio18_9 && aspectRatio < ratio20_9)
                {
                    deviceType = "Between 18:9 and 20:9";
                    float t = (aspectRatio - ratio18_9) / (ratio20_9 - ratio18_9);
                    return Mathf.Lerp(RATIO_18_9_SIZE, RATIO_20_9_SIZE, t);
                }
            }

            // Fallback
            deviceType = "Unknown";
            return RATIO_16_9_SIZE;
        }

        /// <summary>
        /// Gọi method này để force update camera size (ví dụ khi orientation thay đổi)
        /// </summary>
        public void ForceUpdate()
        {
            AdjustCameraSize();
        }

#if UNITY_EDITOR
        // Hiển thị gizmo trong Scene view
        private void OnDrawGizmos()
        {
            if (_camera == null || !_camera.orthographic) return;

            // Vẽ camera bounds
            float height = _camera.orthographicSize * 2f;
            float width = height * _camera.aspect;

            Gizmos.color = Color.yellow;
            Vector3 center = transform.position;
            Gizmos.DrawWireCube(center, new Vector3(width, height, 0));
        }
#endif
    }
}

