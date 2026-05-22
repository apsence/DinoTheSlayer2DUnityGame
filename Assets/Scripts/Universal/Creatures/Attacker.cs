using UnityEngine;
using UnityEngine.Events;

public class Attacker : MonoBehaviour
{
    [Header("Атака")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _attackCooldown = 0.2f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private int _minDamage = 1;
    [SerializeField] private int _maxDamage = 3;
    [SerializeField] private float _nextAttackTime;

    private PlayerGUI _playerGUI;

    private float _lastAttackTime;
    
    public int Damage {get {return _damage;} set {_damage = value; }}
    public float NextAttackTime {get {return _nextAttackTime;} set {_nextAttackTime = value;} }
    public float AttackColdown => _attackCooldown;
    public float AttackRange => _attackRange;
    public int MinDamage => _minDamage;
    public int MaxDamage => _maxDamage;
    public float LastAttackTime {get; set;}

    void Awake()
    {
        _playerGUI = GameObject.FindAnyObjectByType<PlayerGUI>();
    }

    public void UpgradeDamage(int value)
    {
        Damage += value;

        _playerGUI.RefreshPlayerHUDDamage(Damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}