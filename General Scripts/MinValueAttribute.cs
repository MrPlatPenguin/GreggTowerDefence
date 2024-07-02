using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


[CustomPropertyDrawer(typeof(MinValueAttribute))]
public class MinValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the attribute
        MinValueAttribute minValueAttribute = (MinValueAttribute)attribute;

        // Clamp the value to the minimum value
        float minValue = minValueAttribute.minValue;
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            int value = Mathf.Max(property.intValue, (int)minValue);
            property.intValue = value;
        }
        else if (property.propertyType == SerializedPropertyType.Float)
        {
            float value = Mathf.Max(property.floatValue, minValue);
            property.floatValue = value;
        }

        // Draw the property as usual
        EditorGUI.PropertyField(position, property, label);
    }
}


#endif

public class MinValueAttribute : PropertyAttribute
{
    public readonly float minValue;

    public MinValueAttribute(float minValue)
    {
        this.minValue = minValue;
    }
}

