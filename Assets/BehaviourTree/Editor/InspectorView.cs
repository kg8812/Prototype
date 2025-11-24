using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    Editor editor;

    internal void UpdateSelection(NodeView nodeView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (editor.target)
            {
                editor.OnInspectorGUI();
            }
        });
        Add(container);

    }

    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
    {

    }
}
