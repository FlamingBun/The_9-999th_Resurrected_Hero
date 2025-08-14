using UnityEngine;

public class QuestArrowIndicator : MonoBehaviour
{
    [SerializeField] private Transform npcTransform;
    [SerializeField] private Camera mainCamera;
    [SerializeField, Min(0.1f)] private float distanceFromCamera;
    [SerializeField, Range(0f, 0.1f)] private float edgeBufferNormalized;

    [Header("화살표 크기 조절 범위")]
    [SerializeField] private float minScale;    // 최대 거리일 때 최소 크기
    [SerializeField] private float maxScale;      // 가까울 때 원래 크기
    [SerializeField] private float maxDistance;  // 이 거리 이상이면 최소 크기

    private static Camera cachedMainCamera;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = FindMainCamera();
        }

        if (mainCamera == null)
        {
            Debug.LogError("[OffscreenArrowSprite] Main Camera not found. Disabling script.");
            enabled = false;
        }

        // distanceFromCamera 최소값 보장 (0.1 이상)
        distanceFromCamera = Mathf.Max(distanceFromCamera, 0.1f);
    }

    private void Update()
    {
        if (mainCamera == null || npcTransform == null)
        {
            spriteRenderer.enabled = false;
            return;
        }

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(npcTransform.position);

        // NPC가 카메라 뒤에 있으면 좌우 반전 처리
        if (viewportPos.z < 0)
        {
            viewportPos.x = 1f - viewportPos.x;
            viewportPos.y = 1f - viewportPos.y;
            viewportPos.z = 0;
        }

        bool isOffscreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

        // NPC와 카메라 간 거리 계산
        float distanceToNPC = Vector3.Distance(mainCamera.transform.position, npcTransform.position);

        // 거리에 따라 목표 스케일 계산
        float targetScale = Mathf.Lerp(maxScale, minScale, Mathf.InverseLerp(0, maxDistance, distanceToNPC));
        Vector3 desiredScale = Vector3.one * targetScale;

        // 스케일 부드럽게 보간
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 5f);

        if (isOffscreen)
        {
            float clampedX = Mathf.Clamp(viewportPos.x, edgeBufferNormalized, 1f - edgeBufferNormalized);
            float clampedY = Mathf.Clamp(viewportPos.y, edgeBufferNormalized, 1f - edgeBufferNormalized);

            Vector3 arrowPos = mainCamera.ViewportToWorldPoint(new Vector3(clampedX, clampedY, distanceFromCamera));
            transform.position = arrowPos;

            Vector3 dirToNPC = (npcTransform.position - transform.position).normalized;

            // +90도 보정: 화살표 기본 방향이 위쪽(+)이라서 각도를 맞춰줌
            float angle = Mathf.Atan2(dirToNPC.y, dirToNPC.x) * Mathf.Rad2Deg;

            // 회전 부드럽게 보간
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            Vector3 arrowPos = mainCamera.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, distanceFromCamera));
            transform.position = arrowPos;

            // 회전 부드럽게 0으로 보간
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }


    private Camera FindMainCamera()
    {
        if (cachedMainCamera != null) return cachedMainCamera;

        Camera cam = Camera.main;
        if (cam != null)
        {
            cachedMainCamera = cam;
            return cam;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cam = player.GetComponentInChildren<Camera>();
            cachedMainCamera = cam;
        }

        return cam;
    }
}
