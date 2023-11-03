
public enum ReadOnlyTypes {
    Disable,
    EditableInRuntime,
    EditableInEditor,
}

public class ReadOnlyAttribute : UnityEngine.PropertyAttribute {
        
    public readonly ReadOnlyTypes ReadOnlyType;

    public ReadOnlyAttribute(ReadOnlyTypes readOnlyType = ReadOnlyTypes.Disable)
    {
        ReadOnlyType = readOnlyType;
    }
}