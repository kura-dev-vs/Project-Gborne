using UnityEngine;
using UnityEditor;
namespace Mebiustos.MMD4MecanimFaciem {
    public class ShowWindowEx : EditorWindow {
        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/My Window")]
        static void Init() {
            // Get existing open window or if none, make a new one:
            ShowWindowEx window = (ShowWindowEx)EditorWindow.GetWindow(typeof(ShowWindowEx));
            window.Show();
        }

        void OnGUI() {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();
        }
    }
}