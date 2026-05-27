using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private Animator shadowAnimator;
    [SerializeField] private float fadeDuration = 0.1f;
    [SerializeField] private int countOfAttackAnimations;
    private AnimationClip[] allClips;
    private string[] attackAnimations;

    private PlayerState currentState;
    public PlayerState CurrentState => currentState;

    void Awake()
    {
        allClips = modelAnimator.runtimeAnimatorController.animationClips;
    
        // Фильтруем только анимации атак
        List<string> attackList = new List<string>();
        foreach (AnimationClip clip in allClips)
        {
            if (clip.name.StartsWith("Attack"))
            {
                attackList.Add(clip.name);
            }
        }
        attackAnimations = attackList.ToArray();
    }

    public void ChangeState(PlayerState newState, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
    {
        Debug.Log(currentState);
        if (currentState == newState)
            return;
        if(currentState == PlayerState.Dead)
            return;
        
        //* Idle
        if(currentState == PlayerState.Idle && newState == PlayerState.Dash)
            return;
        
        //* Move
        
        //* Dash
        if(currentState == PlayerState.Dash && newState == PlayerState.Move)
            return;
        if(currentState == PlayerState.Dash && newState == PlayerState.Hurt)
            return;

        //* Attack
        // ? Специфичная проверка - "костыль" caller. Поскольку начало атаки приводит к Attack -> атака скипается в начале
        // ? и дальше персонаж застывает, т.к. вызов не доходит дальше базовых проверок. Прямое изменение стейта персонажа
        // ? нецелесообразно, т.к. ChangeState() управляет анимациями -> обход вызовет баги. Менять ее руками -> дублирование кода.
        if(currentState == PlayerState.Attack && (newState == PlayerState.Idle || newState == PlayerState.Move) && caller != "Attack")
            return;
        

        currentState = newState;

        switch (currentState)
        {
            case PlayerState.Idle:
                modelAnimator.CrossFade("Idle", fadeDuration);
                shadowAnimator.CrossFade("Idle", fadeDuration);
                break;
            
            case PlayerState.Move:
                modelAnimator.CrossFade("Move", fadeDuration);
                shadowAnimator.CrossFade("Move", fadeDuration);
                break;
            
            case PlayerState.Dash:
                modelAnimator.CrossFade("Dash", fadeDuration);
                shadowAnimator.CrossFade("Dash", fadeDuration);
                break;

            case PlayerState.Attack:
                int count = attackAnimations.Length;
                int attackIndex = Random.Range(1, count + 1);
                modelAnimator.CrossFade($"Attack_{attackIndex}", fadeDuration);
                shadowAnimator.CrossFade($"Attack_{attackIndex}", fadeDuration);
                break;

            case PlayerState.Hurt:
                modelAnimator.CrossFade("Hurt", fadeDuration);
                shadowAnimator.CrossFade("Hurt", fadeDuration);
                break;

            case PlayerState.Dead:
                modelAnimator.CrossFade("Dead", fadeDuration);
                shadowAnimator.CrossFade("Dead", fadeDuration);
                break;
        }
    }
}