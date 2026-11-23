using UnityEngine;

public class RatFollower : MonoBehaviour
{
    [Header("Visual 2D")]
    public SpriteRenderer spriteRenderer;

    [Header("Ajustes de Seguimiento")]
    public float recruitDistance = 2.5f;
    public float followSpeed = 6.5f;

    private Transform playerTransform;
    private bool isFollowing = false;
    private float facingDir = 1f;
    private Vector3 idlePos;

    private void Start()
    {
        idlePos = transform.position;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void Update()
    {
        if (playerTransform == null || PiperQuestManager.Instance == null) return;

        // Antes de tocar la melodía, las ratas se mueven ligeramente en su sitio
        if (PiperQuestManager.Instance.CurrentStep != PiperQuestManager.Step.HerdRatsToForest)
        {
            float wiggle = Mathf.Sin(Time.time * 5f + idlePos.x) * 0.15f;
            transform.position = new Vector3(idlePos.x + wiggle, idlePos.y, idlePos.z);
            if (spriteRenderer != null) spriteRenderer.flipX = Mathf.Cos(Time.time * 5f + idlePos.x) < 0f;
            return;
        }

        // Comprobar si el jugador se acerca para reclutarla
        if (!isFollowing)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist <= recruitDistance)
            {
                isFollowing = true;
                PiperQuestManager.Instance.RegisterFollowingRat(this);
            }
            return;
        }

        // Detectar dirección del jugador
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX > 0.1f) facingDir = 1f;
        else if (moveX < -0.1f) facingDir = -1f;

        // Marchar en fila detrás del jugador
        int index = PiperQuestManager.Instance.GetFollowIndex(this);
        if (index < 0) index = 0;

        float targetX = playerTransform.position.x - facingDir * (1.2f + index * 0.85f);
        float groundY = Mathf.Max(0.75f, playerTransform.position.y - 0.6f);
        float hopOffset = Mathf.Abs(Mathf.Sin(Time.time * 12f + index)) * 0.18f;

        float deltaX = targetX - transform.position.x;
        if (spriteRenderer != null && Mathf.Abs(deltaX) > 0.02f)
        {
            spriteRenderer.flipX = deltaX < 0f;
        }

        float newX = Mathf.MoveTowards(transform.position.x, targetX, followSpeed * Time.deltaTime);
        float newY = Mathf.MoveTowards(transform.position.y, groundY + hopOffset, followSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, newY, 0f);
    }

    public void EnterForest()
    {
        gameObject.SetActive(false);
    }
}