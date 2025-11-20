using System.Collections;
using UnityEngine;

public class AI_common : MonoBehaviour
{
    [Header("Refs")]
    public GameObject player;
    public GameObject projectilePrefab;
    public float projectileSpeed;

    [Header("Speeds & distances")]
    public float normalSpeed = 3f;
    public float slowSpeed = 1.5f;
    public float safeDistance = 5f;       // зона осторожности
    public float minSafeDistance = 2f;    // критическая близость
    public float approachThreshold = 0.5f;
    public float zoneOfAggression = 1750f;

    [Header("Attack")]
    public float attackCooldown = 1f;

    [Header("Arena")]
    public LayerMask borderMask;          // слой для границ арены

    [Header("Stuck check")]
    public float stuckCheckInterval = 1f;

    [Header("Debug")]
    public bool enableDebug = true;
    public float maxSpeed = 6f;

    private Rigidbody2D rb;
    private Rigidbody2D _playerRigidbody;
    private Vector2 _moveDirection;
    private float _moveSpeed;

    private float _lastDistance;
    private float _stuckTimer;

    void Start()
    {

        if (player == null)
            player = GameObject.FindWithTag("Player");

        rb = GetComponent<Rigidbody2D>();
        _playerRigidbody = player.GetComponent<Rigidbody2D>();
        _lastDistance = Vector2.Distance(transform.position, player.transform.position);

        if (enableDebug)
            Debug.Log($"[AI] Start: pos={transform.position}, playerPos={player.transform.position}, lastDist={_lastDistance}");

        StartCoroutine(AttackRoutine(attackCooldown));
    }

    void Update()
    {
        HandleMovementLogic();
        CheckIfStuckTooClose();
    }

    void FixedUpdate()
    {
        Move(_moveDirection, _moveSpeed);
    }

    // ---------------- Логика движения ----------------


    public void AttackPlayer()
    {
        if (projectilePrefab == null || player == null)
        {
            Debug.LogWarning("[AI] AttackPlayer: нет префаба или игрока");
            return;
        }

        // направление к игроку
        Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;

        // точка спавна: чуть впереди ИИ, на offset
        float spawnOffset = 100f; // расстояние от центра ИИ
        Vector2 spawnPos = (Vector2)transform.position + dirToPlayer * spawnOffset;

        // создаём объект префаба в позиции spawnPos
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, transform.rotation);

        OnCollisionProjectile proj = projectile.GetComponent<OnCollisionProjectile>();
        if (proj != null)
        {
            proj.damage = GetComponent<UnitStats>().damage; // передаём урон врага
            proj.owner = gameObject; // можно сохранить ссылку на владельца
        }

        float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);

        // если у префаба есть Rigidbody2D — задаём скорость
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dirToPlayer * projectileSpeed;
        }

        if (enableDebug)
            Debug.Log($"[AI] AttackPlayer: spawn={spawnPos}, dir={dirToPlayer}, speed={projectileSpeed}");
    }

    void HandleMovementLogic()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        if (enableDebug)
            Debug.Log($"[AI] HandleMovement: distance={distance:F2}, safe={safeDistance}, minSafe={minSafeDistance}");

        if (distance > safeDistance)
        {
            if (enableDebug)
                Debug.Log("[AI] State: CHASE (player far)");
            MoveTowardsPlayer(direction);
        }
        else if (distance > minSafeDistance)
        {
            if (enableDebug)
                Debug.Log("[AI] State: WANDER (near but not critical)");
            WanderNearPlayer(direction);
        }
        else
        {
            if (enableDebug)
                Debug.Log("[AI] State: RETREAT (too close)");
            RetreatFromPlayer(direction);
        }
    }

    void MoveTowardsPlayer(Vector2 direction)
    {
        Vector2 targetPoint = player.transform.position;
        _moveDirection = SafeNormalize(direction);
        _moveSpeed = normalSpeed;

        if (enableDebug)
            Debug.Log($"[AI] CHASE: target={targetPoint}, dist={Vector2.Distance(transform.position, targetPoint):F2}, dir={_moveDirection}, speed={_moveSpeed}");
    }

    // Блуждание рядом с игроком: точки только "от игрока"
    void WanderNearPlayer(Vector2 direction)
    {
        bool approaching = IsPlayerApproaching(_playerRigidbody.linearVelocity, direction);
        Vector2 selfPos = (Vector2)transform.position;
        Vector2 dirToPlayer = SafeNormalize(direction);
        Vector2 away = -dirToPlayer;

        if (approaching)
        {
            Vector2 targetPoint = selfPos + away * safeDistance * 0.8f;
            _moveDirection = SafeNormalize(targetPoint - selfPos);
            _moveSpeed = Mathf.Min(slowSpeed * 1.5f, normalSpeed);

            if (enableDebug)
                Debug.Log($"[AI] WANDER->RETREATISH: target={targetPoint}, dist={Vector2.Distance(selfPos, targetPoint):F2}, dir={_moveDirection}, speed={_moveSpeed}, reason=playerApproaching");
        }
        else
        {
            // выбираем случайную точку в полукруге "от игрока"
            Vector2 randomOffset = Quaternion.Euler(0, 0, Random.Range(-90f, 90f)) * away;
            Vector2 targetPoint = selfPos + randomOffset * safeDistance;

            // фильтруем: точка не должна быть ближе к игроку
            if (Vector2.Distance(targetPoint, player.transform.position) < Vector2.Distance(selfPos, player.transform.position))
            {
                targetPoint = selfPos + away * safeDistance; // fallback
            }

            _moveDirection = SafeNormalize(targetPoint - selfPos);
            _moveSpeed = slowSpeed;

            if (enableDebug)
                Debug.Log($"[AI] WANDER->ROAM: target={targetPoint}, dist={Vector2.Distance(selfPos, targetPoint):F2}, dir={_moveDirection}, speed={_moveSpeed}, reason=random away");
        }
    }



    // Отступление: точка позади себя
    void RetreatFromPlayer(Vector2 direction)
    {
        Vector2 selfPos = (Vector2)transform.position;
        Vector2 currentDir = _moveDirection.sqrMagnitude > 0.01f ? _moveDirection : (Vector2)transform.up;

        Vector2 retreatDir = SafeNormalize(-currentDir);
        Vector2 sideStep = Vector2.Perpendicular(retreatDir) * Random.Range(-0.3f, 0.3f);
        Vector2 finalDir = SafeNormalize(retreatDir + sideStep);

        Vector2 targetPoint = selfPos + finalDir * minSafeDistance;

        if (!IsDirectionFree(finalDir, normalSpeed))
        {
            finalDir = ChooseAlternativeDirection(finalDir, normalSpeed);
            targetPoint = selfPos + finalDir * minSafeDistance;
        }

        _moveDirection = finalDir;
        _moveSpeed = Mathf.Min(normalSpeed, maxSpeed);

        if (enableDebug)
            Debug.Log($"[AI] RETREAT: target={targetPoint}, dist={Vector2.Distance(selfPos, targetPoint):F2}, dir={_moveDirection}, speed={_moveSpeed}, reason=too close");
    }

    // ---------------- Проверка "застревания" ----------------

    void CheckIfStuckTooClose()
    {
        float currentDistance = Vector2.Distance(transform.position, player.transform.position);
        _stuckTimer += Time.deltaTime;

        if (enableDebug)
            Debug.Log($"[AI] StuckCheck: timer={_stuckTimer:F2}/{stuckCheckInterval}, currDist={currentDistance:F2}, lastDist={_lastDistance:F2}");

        if (_stuckTimer >= stuckCheckInterval)
        {
            bool nearCrit = currentDistance < minSafeDistance;
            bool noProgress = Mathf.Abs(currentDistance - _lastDistance) < 0.1f;

            if (enableDebug)
                Debug.Log($"[AI] StuckCheck eval: nearCrit={nearCrit}, noProgress={noProgress}");

            if (noProgress && nearCrit)
            {
                FindNewPointBehindSelf();
            }


            _lastDistance = currentDistance;
            _stuckTimer = 0f;
        }
    }

    void FindNewPointBehindSelf()
    {
        // Берём текущее направление движения или "вперёд" объекта
        Vector2 currentDir = _moveDirection.sqrMagnitude > 0.01f
            ? _moveDirection
            : (Vector2)transform.up;

        // Точка позади ИИ
        Vector2 retreatDir = -currentDir.normalized;

        // Добавляем небольшой боковой разброс
        retreatDir += Random.insideUnitCircle * 0.5f;

        _moveDirection = SafeNormalize(retreatDir);
        _moveSpeed = normalSpeed;

        if (enableDebug)
            Debug.Log($"[AI] FindNewPointBehindSelf: retreatDir={retreatDir}, finalDir={_moveDirection}, speed={_moveSpeed}");
    }


    // ---------------- Вспомогательные методы ----------------


    void Move(Vector2 direction, float speed)
    {
        if (speed <= 0f || direction == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            if (enableDebug)
                Debug.Log($"[AI] Move: STOP (dir={direction}, speed={speed})");
            return;
        }

        float clamped = Mathf.Min(speed, maxSpeed);
        Vector2 safeDir = SafeNormalize(direction);
        rb.linearVelocity = safeDir * clamped;

        if (enableDebug)
            Debug.Log($"[AI] Move: vel={rb.linearVelocity}, dir={safeDir}, speed={clamped}");
    }


    Vector2 ChooseAlternativeDirection(Vector2 currentDir, float speed)
    {
        Vector2 dirA = SafeNormalize(Vector2.Perpendicular(currentDir));
        Vector2 dirB = -dirA;

        bool freeA = IsDirectionFree(dirA, speed);
        bool freeB = IsDirectionFree(dirB, speed);

        if (enableDebug)
            Debug.Log($"[AI] ChooseAlt: current={currentDir}, dirA={dirA}, freeA={freeA}, dirB={dirB}, freeB={freeB}");

        if (freeA) return dirA;
        if (freeB) return dirB;

        Vector2 fallback = -currentDir;

        if (enableDebug)
            Debug.Log($"[AI] ChooseAlt: fallback={fallback}");
        return fallback;
    }

    // Проверка свободного направления: учитываем и границы, и игрока
    bool IsDirectionFree(Vector2 direction, float speed)
    {
        float radius = 0.5f;
        float distance = speed * Time.fixedDeltaTime;

        // проверяем столкновения с границами и игроком
        int mask = borderMask | LayerMask.GetMask("Player");
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, direction, distance, mask);
        bool free = hit.collider == null;


        if (enableDebug)
            Debug.Log($"[AI] Raycast: dir={direction}, dist={distance:F3}, radius={radius}, free={free}, hit={(hit.collider ? hit.collider.name : "none")}");

        return free;
    }



    Vector2 SafeNormalize(Vector2 v)
    {
        if (v.sqrMagnitude > 0.0001f) return v.normalized;
        return Vector2.zero;
    }

    bool IsPlayerApproaching(Vector2 playerVelocity, Vector2 directionToAI)
    {
        if (playerVelocity.sqrMagnitude < 0.0001f)
        {
            if (enableDebug)
                Debug.Log("[AI] Approaching: player stationary => false");
            return false;
        }

        Vector2 safeVel = playerVelocity.normalized;
        float dot = Vector2.Dot(safeVel, SafeNormalize(directionToAI));
        bool approaching = dot > approachThreshold;

        if (enableDebug)
            Debug.Log($"[AI] Approaching: dot={dot:F3}, threshold={approachThreshold}, result={approaching}");

        return approaching;
    }

    IEnumerator AttackRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < zoneOfAggression) // атакуем только если игрок рядом
            {
                AttackPlayer();
            }
        }
    }
}
