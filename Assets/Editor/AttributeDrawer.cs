[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute), true)]
public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
{

    public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label)
    {
        bool disabled = true;
        switch (((ReadOnlyAttribute)attribute).ReadOnlyType)
        {
            case ReadOnlyTypes.Disable:
                disabled = true;
                break;
            case ReadOnlyTypes.EditableInRuntime:
                disabled = !UnityEngine.Application.isPlaying;
                break;
            case ReadOnlyTypes.EditableInEditor:
                disabled = UnityEngine.Application.isPlaying;
                break;
        }
            
        using (var scope = new UnityEditor.EditorGUI.DisabledGroupScope(disabled))
        {
            UnityEditor.EditorGUI.PropertyField(position, property, label, true);
        }
    }

}