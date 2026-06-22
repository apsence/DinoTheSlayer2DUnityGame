using UnityEngine;
using UnityEditor;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

[CustomPropertyDrawer(typeof(KeyCodeAlphaAttribute))]
public class KeyCodeAlphaDrawer : PropertyDrawer
{
    private static readonly string[] _labels;
    private static readonly KeyCode[] _keyCodes;

    static KeyCodeAlphaDrawer()
    {
        _keyCodes = new KeyCode[]
        {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
            KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
            KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
            KeyCode.Z, KeyCode.Space, KeyCode.Tab, KeyCode.Escape,
            KeyCode.Mouse0
        };

        _labels = System.Array.ConvertAll(_keyCodes, k => k.ToString());
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        KeyCode current = (KeyCode)property.intValue;

        int currentIndex = System.Array.IndexOf(_keyCodes, current);
        if (currentIndex < 0) currentIndex = 0;

        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, _labels);
        property.intValue = (int)_keyCodes[selectedIndex];

        EditorGUI.EndProperty();
    }
}