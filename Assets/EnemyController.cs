using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public enum EnemyState
{
   Patrolling,
   Following
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float attackRange;

    private float patrolWaitTime = 3f;
    private float stopAtDistance = 1f;
    public float detectionRange = 5f;
    public float viewAngle = 90f;
    private float lostPlayerTime = 3f;

    private NavMeshAgent _agent;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private EnemyState _state = EnemyState.Patrolling;
    private float _timeSinceLostPlayer;
    public LayerMask layers;
    private Coroutine _waitCoroutine;
    private Quaternion _startRotation; // Store the starting rotation at the patrol point

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;
        
        // Store the starting rotation when we begin waiting at this patrol point
        _startRotation = transform.rotation;
    
        float elapsedTime = 0f;
        float lookSpeed = 90f; // Degrees per second
        float maxAngle = 90f; // Maximum angle to look left/right
        
        while (elapsedTime < patrolWaitTime)
        {
            // Check for player EVERY frame during the wait
            if (CanSeePlayer())
            {
                print("following");
                _state = EnemyState.Following;
                _isWaiting = false;
                _agent.isStopped = false;
                _waitCoroutine = null;
                yield break;
            }
    
            // Smoothly look left and right - FIXED: Use a proper sine wave for rotation
            // This creates a smooth back-and-forth motion from -maxAngle to +maxAngle
            float angle = Mathf.Sin(elapsedTime * Mathf.PI * 0.5f) * maxAngle; // Adjust speed with multiplier
            // Alternative: use this for faster looking: float angle = Mathf.Sin(elapsedTime * 2f) * maxAngle;
            
            // Apply rotation relative to the starting rotation
            Quaternion targetRotation = _startRotation * Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        // Smoothly return to original rotation
        float resetTime = 0f;
        float resetDuration = 0.5f;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = _startRotation;
    
        while (resetTime < resetDuration)
        {
            // Check for player during reset rotation
            if (CanSeePlayer())
            {
                _state = EnemyState.Following;
                _isWaiting = false;
                _agent.isStopped = false;
                _waitCoroutine = null;
                yield break;
            }
    
            transform.rotation = Quaternion.Slerp(startRot, targetRot, resetTime / resetDuration);
            resetTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we end at exactly the start rotation
        transform.rotation = _startRotation;
    
        _agent.isStopped = false;
        GoToNextPatrolPoint();
        _isWaiting = false;
        _waitCoroutine = null;
    }

    private void Patrol()
    {
        if (_isWaiting) return;
        if (patrolPoints.Length == 0) return;
        if (!_agent.pathPending && _agent.remainingDistance <= stopAtDistance)
        {
            // Stop any existing coroutine before starting a new one
            if (_waitCoroutine != null)
            {
                StopCoroutine(_waitCoroutine);
                _waitCoroutine = null;
            }
            _waitCoroutine = StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (patrolPoints.Length > 0)
        {
            GoToNextPatrolPoint();
        }
    }

    private void Update()
    {
        if (player == null) return;
        
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    _state = EnemyState.Following;
                    print("following");
                }

                break;

            case EnemyState.Following:
                FollowPlayer();
                if (distanceToPlayer <= attackRange)
                {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                }
                if (!CanSeePlayer())
                {
                    _timeSinceLostPlayer += Time.deltaTime;
                    if (_timeSinceLostPlayer > lostPlayerTime)
                    {
                        _state = EnemyState.Patrolling;
                        _isWaiting = false;
                        
                        // Stop any active waiting coroutine
                        if (_waitCoroutine != null)
                        {
                            StopCoroutine(_waitCoroutine);
                            _waitCoroutine = null;
                        }
                        
                        print("patrolling");
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    _timeSinceLostPlayer = 0;
                }
                break;
        }

    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    private bool IsFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        if (player == null) return false;
        
        var dirToPlayer = player.position - transform.position;
        float maxDistance = dirToPlayer.magnitude;
        
        if (maxDistance <= 0) return false;
        
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, maxDistance, layers))
        {
            if (hit.transform == player)
                return true;
            return false;
        }
        
        return true;
    }
    
    private void FollowPlayer()
    {
        if (player == null) return;
        _agent.SetDestination(player.position);
    }

    private void GoToClosestPatrolPoint()
    {
        if(patrolPoints.Length == 0) return;
        
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }
        _currentPatrolIndex = closestIndex;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
    }

    private void OnDisable()
    {
        // Clean up coroutine if enemy is disabled
        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }
    }
    
    private void OnEnable()
    {
        // Reset state when re-enabled
        _isWaiting = false;
        _timeSinceLostPlayer = 0;
        if (_agent != null)
        {
            _agent.isStopped = false;
        }
    }
}