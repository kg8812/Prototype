using System;
using Apis.BehaviourTreeTool;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : Node
{
    // 마지막 평가로부터 이 시간이 지난 노드는 색을 지운다. 에디터 갱신 주기(약 0.1초)보다 넉넉해야
    // 매 틱 평가되는 노드가 깜빡이지 않는다.
    private const float StaleThreshold = 0.2f;

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

        if (!Application.isPlaying) return;

        // 루트는 항상 실행 경로 위에 있어 늘 색이 켜진다. 정보가 없는데 눈만 끌어서 제외한다.
        if (node is RootNode) return;

        // node.state는 "마지막 실행 결과"라서 이번 틱에 지나가지 않은 노드에도 그대로 남는다.
        // 그대로 칠하면 중단됐거나 한참 전에 끝난 브랜치까지 계속 색이 박혀 있으므로,
        // 최근에 평가된 노드만 칠한다. 에디터 갱신은 OnInspectorUpdate(약 10Hz)라 여유를 둔다.
        if (Time.time - node.lastEvaluatedTime > StaleThreshold) return;

        switch (node.state)
        {
            case TreeNode.State.Running:
                AddToClassList("running");
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