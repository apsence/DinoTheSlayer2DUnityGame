using UnityEngine;

public class GlobalOpenedGUI : MonoBehaviour
{
    
    public static GlobalOpenedGUI Instance { get; private set; }
    public MonoBehaviour CurrentGui { get; private set; }
    public bool IsAnyGuiOpen => CurrentGui != null;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Пытаемся открыть новый GUI. Если уже существует открытый GUI - новый не появится.
    /// Некоторые GUI открываются в обход (пример: меню).
    /// </summary>
    /// <returns>bool.</returns>
    /// <param name="MonoBehavior">Обьект GUI.</param>
    public bool TryOpen(MonoBehaviour gui)
    {
        if (CurrentGui != null)
            return false;

        CurrentGui = gui;
        return true;
    }

    public void Close(MonoBehaviour gui)
    {
        if (CurrentGui == gui)
            CurrentGui = null;
    }
}