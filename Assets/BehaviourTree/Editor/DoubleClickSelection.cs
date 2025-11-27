using UnityEditor;
using UnityEngine.UIElements;

namespace Apis.BehaviourTreeTool
{
    public class DoubleClickSelection : MouseManipulator
    {
        private readonly double doubleClickDuration = 0.3;
        private double time;

        public DoubleClickSelection()
        {
            time = EditorApplication.timeSinceStartup;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            var graphView = target as BehaviourTreeView;
            if (graphView == null)
                return;

            var duration = EditorApplication.timeSinceStartup - time;
            if (duration < doubleClickDuration) SelectChildren(evt);

            time = EditorApplication.timeSinceStartup;
        }

        private void SelectChildren(MouseDownEvent evt)
        {
            var graphView = target as BehaviourTreeView;
            if (graphView == null)
                return;

            if (!CanStopManipulation(evt))
                return;

            var clickedElement = evt.target as NodeView;
            if (clickedElement == null)
            {
                if (evt.target is VisualElement ve) clickedElement = ve.GetFirstAncestorOfType<NodeView>();
                if (clickedElement == null)
                    return;
            }

            // Add children to selection so the root element can be moved
            BehaviourTree.Traverse(clickedElement.node, node =>
            {
                var view = graphView.FindNodeView(node);
                graphView.AddToSelection(view);
            });
        }
    }
}