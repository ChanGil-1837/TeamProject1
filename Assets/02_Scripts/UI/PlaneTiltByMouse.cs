using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Collider))] // 실제 충돌 감지용
public class PlaneTiltByMouse : MonoBehaviour
{
    [SerializeField] private float tiltAmount = 10f;  // 최대 기울기 각도
    [SerializeField] private float tiltSpeed = 5f;    // 회전 속도 (LERP용)
    [SerializeField] private Camera mainCamera;

    private Quaternion _initialRotation;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        _initialRotation = transform.rotation;
    }

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo);

        if (isHit && hitInfo.collider.gameObject == gameObject)
        {
            // 마우스가 실제로 이 plane 오브젝트에 닿았을 때만
            Vector3 hitPoint = hitInfo.point;
            Vector3 vectorToHit = hitPoint - transform.position;

            float localX = Vector3.Dot(vectorToHit, transform.right);   // 로컬 X (좌우)
            float localZ = Vector3.Dot(vectorToHit, transform.forward); // 로컬 Z (상하)

            Vector3 localDir = new Vector3(localX, 0, localZ);

            float tiltX = Mathf.Clamp(-localDir.z * tiltAmount, -tiltAmount, tiltAmount);
            float tiltZ = Mathf.Clamp(localDir.x * tiltAmount, -tiltAmount, tiltAmount);

            Quaternion targetRotation = _initialRotation * Quaternion.Euler(tiltX, 0, -tiltZ);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
        else
        {
            // 마우스가 닿지 않으면 원래 회전으로 복귀
            transform.rotation = Quaternion.Lerp(transform.rotation, _initialRotation, Time.deltaTime * tiltSpeed);
        }
    }
}
