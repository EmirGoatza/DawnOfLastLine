using UnityEngine;

public abstract class AugmentEffect : ScriptableObject
{
    [TextArea]
    [SerializeField] private string descriptionTemplate;

    public abstract void Apply(GameObject player);

    protected abstract object[] GetDescriptionValues();

    public string GetDescription()
    {
        if (string.IsNullOrEmpty(descriptionTemplate))
            return "";

        return string.Format(descriptionTemplate, GetDescriptionValues());
    }
}