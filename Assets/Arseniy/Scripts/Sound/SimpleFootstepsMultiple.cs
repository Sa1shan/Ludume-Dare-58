using UnityEngine;

public class FootstepController_Rigidbody : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Rigidbody игрока (можно оставить пустым, если он на этом же объекте)")]
    [SerializeField] private Rigidbody rb;

    [Header("Footstep Sounds")]
    [Tooltip("Список звуков шагов (2 и больше)")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Settings")]
    [Tooltip("Расстояние между шагами (в метрах)")]
    [SerializeField] private float stepDistance = 2f;
    [Tooltip("Минимальная скорость для начала шагов")]
    [SerializeField] private float minMoveSpeed = 0.1f;
    [Tooltip("Использовать случайные звуки (true) или по порядку (false)")]
    [SerializeField] private bool useRandom = true;
    [Tooltip("Множитель скорости (увеличь если шаги слишком редкие при беге)")]
    [SerializeField] private float speedMultiplier = 1f;

    [SerializeField] private AudioSource audioSource;
    private Vector3 lastPosition;
    private float distanceAccumulated = 0f;
    private int lastClipIndex = -1;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D звук
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        // Проверяем, стоит ли игрок на земле
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f);

        // Считаем скорость
        float speed = rb.linearVelocity.magnitude;

        if (isGrounded && speed > minMoveSpeed)
        {
            // накапливаем пройденное расстояние
            distanceAccumulated += speed * Time.deltaTime * speedMultiplier;

            if (distanceAccumulated >= stepDistance)
            {
                PlayFootstep();
                distanceAccumulated = 0f;
            }
        }
        else
        {
            distanceAccumulated = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index;
        if (useRandom)
        {
            if (footstepClips.Length == 1)
                index = 0;
            else
            {
                index = Random.Range(0, footstepClips.Length);
                if (index == lastClipIndex)
                    index = (index + 1) % footstepClips.Length;
            }
        }
        else
        {
            index = (lastClipIndex + 1) % footstepClips.Length;
        }

        lastClipIndex = index;

        // небольшое рандомное отклонение питча, чтобы звук не был одинаковый
        audioSource.PlayOneShot(footstepClips[index]);
    }
}
