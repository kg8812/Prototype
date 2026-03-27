using System;
using System.Collections.Generic;
using System.Linq;
using Apis.BehaviourTreeTool;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    private BehaviourTree tree;

    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new DoubleClickSelection());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        var styleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    public NodeView FindNodeView(TreeNode node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());

        graphViewChanged += OnGraphViewChanged;

        if (tree != null && tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        tree.nodes.ForEach(CreateNodeView);

        tree.nodes.ForEach(n =>
        {
            var children = BehaviourTree.GetChildren(n);
            children.ForEach(c =>
            {
                var parentView = FindNodeView(n);
                var childView = FindNodeView(c);

                var edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList()
            .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                if (elem is NodeView nodeView) tree.DeleteNode(nodeView.node);

                if (elem is Edge edge)
                {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;
                    if (parentView == null || childView == null) return;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });

        if (graphViewChange.edgesToCreate != null)
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                var parentView = edge.output.node as NodeView;
                var childView = edge.input.node as NodeView;
                if (parentView == null || childView == null) return;
                tree.AddChild(parentView.node, childView.node);
                parentView.SortChildren();
            });

        if (graphViewChange.movedElements != null)
            nodes.ForEach(n =>
            {
                var view = n as NodeView;
                view?.SortChildren();
            });
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        var nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();

            List<Type> baseTypes = new();
            foreach (var type in types)
                if (type.BaseType == typeof(ActionNode))
                    baseTypes.Add(type);

            foreach (var type in baseTypes)
            {
                types = TypeCache.GetTypesDerivedFrom(type);

                foreach (var t in types)
                    evt.menu.AppendAction($"Action/{type.Name}/{t.Name}", _ => { CreateNode(t, nodePosition); });

                evt.menu.AppendAction($"Action/{type.Name}/Create New Script", _ =>
                    {
                        var window = EditorWindow.GetWindow(typeof(ActionNodeWindow)) as ActionNodeWindow;
                        if (window != null) window.Init(type.Name);
                    }
                );
            }

            evt.menu.AppendAction("Action/Create New Type", _ => { EditorWindow.GetWindow(typeof(ActionTypeWindow)); });
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();

            List<Type> baseTypes = new();
            foreach (var type in types)
                if (type.BaseType == typeof(CompositeNode))
                    baseTypes.Add(type);

            foreach (var type in baseTypes)
            {
                types = TypeCache.GetTypesDerivedFrom(type);

                foreach (var t in types)
                    evt.menu.AppendAction($"Composite/{type.Name}/{t.Name}", _ => { CreateNode(t, nodePosition); });

                evt.menu.AppendAction($"Composite/{type.Name}/Create New Script", _ =>
                    {
                        var window = EditorWindow.GetWindow(typeof(CompositeNodeWindow)) as CompositeNodeWindow;
                        if (window != null) window.Init(type.Name);
                    }
                );
            }

            evt.menu.AppendAction("Composite/Create New Type",
                _ => { EditorWindow.GetWindow(typeof(CompositeTypeWindow)); });
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();

            List<Type> baseTypes = new();
            foreach (var type in types)
                if (type.BaseType == typeof(DecoratorNode))
                    baseTypes.Add(type);

            foreach (var type in baseTypes)
            {
                types = TypeCache.GetTypesDerivedFrom(type);

                foreach (var t in types)
                    evt.menu.AppendAction($"Decorator/{type.Name}/{t.Name}", _ => { CreateNode(t, nodePosition); });

                evt.menu.AppendAction($"Decorator/{type.Name}/Create New Script", _ =>
                    {
                        var window = EditorWindow.GetWindow(typeof(DecoratorNodeWindow)) as DecoratorNodeWindow;
                        if (window != null) window.Init(type.Name);
                    }
                );
            }

            evt.menu.AppendAction("Decorator/Create New Type",
                _ => { EditorWindow.GetWindow(typeof(DecoratorTypeWindow)); });
        }
    }

    private void CreateNode(Type type, Vector2 pos)
    {
        if (tree == null) return;

        var node = tree.CreateNode(type);
        node.position = pos;
        CreateNodeView(node);
    }

    private void CreateNodeView(TreeNode node)
    {
        NodeView nodeView = new(node)
        {
            OnNodeSelected = OnNodeSelected
        };
        AddElement(nodeView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            var view = n as NodeView;
            view?.UpdateState();
        });
    }

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits>
    {
    }
}