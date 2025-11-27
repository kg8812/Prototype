using System;
using Apis.BehaviourTreeTool;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : Node
{
    public Port input;
    public TreeNode node;
    public Action<NodeView> OnNodeSelected;
    public Port output;

    public NodeView(TreeNode node) : base("Assets/BehaviourTree/Editor/NodeView.uxml")
    {
        if (node == null) return;

        this.node = node;
        title = node.name;

        viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetUpClasses();

        var descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    private void SetUpClasses()
    {
        if (node is ActionNode)
            AddToClassList("action");
        else if (node is CompositeNode)
            AddToClassList("composite");
        else if (node is DecoratorNode)
            AddToClassList("decorator");
        else if (node is RootNode) AddToClassList("root");
    }

    private void CreateInputPorts()
    {
        if (node is ActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is CompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is DecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is RootNode)
        {
        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        if (node is ActionNode)
        {
        }
        else if (node is CompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (node is DecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (node is RootNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behaviour Tree (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();

        if (OnNodeSelected != null) OnNodeSelected.Invoke(this);
    }

    public void SortChildren()
    {
        if (node is CompositeNode composite) composite.children.Sort(SortByHorizontalPosition);
    }

    private int SortByHorizontalPosition(TreeNode left, TreeNode right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
            switch (node.state)
            {
                case TreeNode.State.Running:
                    if (node.isStarted) AddToClassList("running");
                    break;
                case TreeNode.State.Success:
                    AddToClassList("success");
                    break;
                case TreeNode.State.Failure:
                    AddToClassList("failure");
                    break;
            }
    }
}