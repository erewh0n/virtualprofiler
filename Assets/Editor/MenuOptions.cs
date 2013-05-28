using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{

    public class MenuOptions : ScriptableObject
    {
        public void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Configure...", EditorStyles.toolbarButton))
            {
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("New Recording...", EditorStyles.toolbarButton))
            {
                GUIUtility.ExitGUI();
            }

            GUILayout.EndHorizontal();
        }
    }

}
