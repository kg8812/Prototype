using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    private Editor editor;

    internal void UpdateSelection(NodeView nodeView)
    {
        Clear();

        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        var container = new IMGUIContainer(() =>
        {
            if (editor.target) editor.OnInspectorGUI();
        });
        Add(container);
    }

    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
    {
    }
}