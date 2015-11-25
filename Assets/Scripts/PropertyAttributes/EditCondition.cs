using UnityEngine;


// Property attribute to add edit condition
public class EditCondition : PropertyAttribute
{
	public string ConditionName { get; private set; }

	public EditCondition(string conditionName)
	{
		this.ConditionName = conditionName;
	}
}
