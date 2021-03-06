﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

// Custom drawer for EditCondition property
[CustomPropertyDrawer(typeof(EditCondition))]
public class EditConditionDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditCondition editCondition = attribute as EditCondition;

		//-- Get edit condition property
		object parent = GetParent(property);
		bool isOpenForEdit = AssetDatabase.IsOpenForEdit((UnityEngine.Object)parent);
		string conditionName = editCondition.ConditionName;
		int numNegations = 0;
		while (conditionName[0] == '!')
		{
			numNegations++;
			conditionName = conditionName.Substring(1);
		}

		object condValue = GetValue(parent, conditionName);
		if (condValue == null)
		{
			GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
			labelStyle.normal.textColor = new Color(1f, 0.5f, 0.5f);
			EditorGUI.LabelField(position, "Condition " + editCondition.ConditionName + " not found in " + property.propertyPath, labelStyle);
			return;
		}
		bool condition = (bool)condValue;
		if ((numNegations & 1) == 1)
		{
			condition = !condition;
		}

		if (editCondition.ShowCheckbox)
		{
			//-- Toggle for edit condition
			// Adjust rectangle for checkbox
			Rect toggleRect = position;
			float toggleWidth = GUI.skin.toggle.CalcSize(new GUIContent("")).x;
			toggleRect.xMax = toggleRect.xMin + toggleWidth + 8f;
			bool newCondValue = EditorGUI.Toggle(toggleRect, condition);
			// Set new condition value
			SetValue(parent, editCondition.ConditionName, newCondValue);
			// Adjust value rectangle
			position.xMin = toggleRect.xMax;
		}

		//-- Property
		GUI.enabled = condition && isOpenForEdit;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = isOpenForEdit;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label);
	}

	private object GetParent(SerializedProperty prop)
	{
		var path = prop.propertyPath.Replace(".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
		var elements = path.Split('.');
		foreach (var element in elements.Take(elements.Length - 1))
		{
			if (element.Contains("["))
			{
				var elementName = element.Substring(0, element.IndexOf("["));
				var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
				obj = GetValue(obj, elementName, index);
			}
			else
			{
				obj = GetValue(obj, element);
			}
		}
		return obj;
	}

	private object GetValue(object source, string name)
	{
		if (source == null)
		{
			return null;
		}
		var type = source.GetType();

		var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		if (field != null)
		{
			return field.GetValue(source);
		}

		var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
		if (property != null)
		{
			return property.GetValue(source, null);
		}

		return null;
	}

	private object GetValue(object source, string name, int index)
	{
		var enumerable = GetValue(source, name) as IEnumerable;
		var enm = enumerable.GetEnumerator();
		while (index-- >= 0)
		{
			enm.MoveNext();
		}
		return enm.Current;
	}

	private void SetValue(object source, string name, object value)
	{
		if (source == null)
		{
			return;
		}
		var type = source.GetType();

		var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		if (field != null)
		{
			field.SetValue(source, value);
			return;
		}

		var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
		if (property != null)
		{
			property.SetValue(source, value, null);
			return;
		}
	}

}
