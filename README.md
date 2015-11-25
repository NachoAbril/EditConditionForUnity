# EditConditionForUnity
EditCondition property for Unity. Similar to EditCondition in Unreal Engine

Inside your class:

	[HideInInspector]
	public bool myEditCondition = true;
	[EditCondition("myEditCondition")]
	public float anyVariable;

In inspector you will see a checkbox in front of your variable ("anyVariable" in this case). Unchecking the check box, the variable will be readonly in the inspector.
