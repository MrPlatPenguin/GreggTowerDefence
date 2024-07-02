using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(StructureSO))]
public class StructureSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the serialized properties for the DPS variable
        SerializedProperty DPS = serializedObject.FindProperty("DPS");

        // Get the Damage and TimeBetweenAttacks properties using reflection
        PropertyInfo DamageProperty = serializedObject.targetObject.GetType().GetProperty("Damage");
        PropertyInfo TimeBetweenAttacksProperty = serializedObject.targetObject.GetType().GetProperty("TimeBetweenAttacks");

        // Get the values of the Damage and TimeBetweenAttacks properties
        float Damage = (float)DamageProperty.GetValue(serializedObject.targetObject, null);
        float TimeBetweenAttacks = (float)TimeBetweenAttacksProperty.GetValue(serializedObject.targetObject, null);

        // Calculate the DPS and update the DPS property
        DPS.floatValue = Damage != 0 ? Damage / TimeBetweenAttacks : 0;

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
#endif