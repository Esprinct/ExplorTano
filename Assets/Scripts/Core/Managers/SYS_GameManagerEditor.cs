using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SYS_GameManager))]
public class SYS_GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SYS_GameManager gameManager = (SYS_GameManager)target;

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool inspectorChanged = EditorGUI.EndChangeCheck();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Les profils IA sont synchronisés automatiquement depuis 'Personnalite IA'.",
            MessageType.Info
        );

        if (GUILayout.Button("Synchroniser les profils IA"))
        {
            Undo.RecordObject(gameManager, "Synchroniser profils IA");
            gameManager.SynchroniserProfilsIADansInspecteur();
            EditorUtility.SetDirty(gameManager);
        }

        if (inspectorChanged)
        {
            gameManager.SynchroniserProfilsIADansInspecteur();
            EditorUtility.SetDirty(gameManager);
        }

        serializedObject.ApplyModifiedProperties();
    }
}