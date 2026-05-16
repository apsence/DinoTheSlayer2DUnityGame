using Unity.VisualScripting;
using UnityEngine;

public class Init : MonoBehaviour
{
    private PlayerGUI _playerGUI;
    private Attacker _attacker;
    private Health _health;
    void Awake()
    {
        _attacker = GameObject.FindGameObjectWithTag("Player").GetComponent<Attacker>();
        _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        _playerGUI = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGUI>();
    }
    void Start()
    {
        _playerGUI.RefreshPlayerHUDDamage(_attacker.Damage);
        _playerGUI.RefreshPlayerHUDHealthBar(_health.MaxHealth, _health.MaxHealth);
    }
}
