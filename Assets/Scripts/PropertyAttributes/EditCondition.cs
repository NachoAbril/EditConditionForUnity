using UnityEngine;


// Property attribute to add edit condition
public class EditCondition : PropertyAttribute
{
	public string ConditionName { get; private set; }
	public bool ShowCheckbox { get; private set; }

	public EditCondition(string conditionName, bool showCheckbox = true)
	{
		this.ConditionName = conditionName;
		this.ShowCheckbox = showCheckbox;
	}
}
