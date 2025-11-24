using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

namespace Apis.BehaviourTreeTool
{
    [CreateAssetMenu(menuName = "Scriptable/BehaviourTree")]
    public class BehaviourTree : ScriptableObject
    {
        public TreeNode rootNode; // 루트 노드
        public TreeNode.State treeState = TreeNode.State.Running; // 현재 행동트리 상태
        [ReadOnly] public List<TreeNode> nodes = new (); // 모든 노드 리스트
        public BlackBoard blackboard = new BlackBoard (); // 블랙보드 (상태창)
        bool isRepeat; // 반복여부, False시 한번만 실행
        [HideInInspector] public Actor actor; // 액터

        public void Init(Actor actor, bool isRepeat) // 행동트리 초기화
        {
            this.isRepeat = isRepeat;
            
            this.actor = actor;           
            Traverse(rootNode,(a) => { a.blackBoard = blackboard; a.SetActor(actor); a.tree = this; });
            Traverse(rootNode, (a) => a.Init());
            if (!isRepeat)
            {
                treeState = rootNode.Update();
            }
        }
        public TreeNode.State Update() // 행동트리 업데이트
        {           
            if (isRepeat)
            {
                treeState = rootNode.Update();
            }
            else if (rootNode.state == TreeNode.State.Running)
            {
                treeState = rootNode.Update();
            }
            return treeState;
        }

        public void CancelCurrentNode()
        {
            if(blackboard.currentNode != null)
            {
                blackboard.currentNode.OnSkip();
                blackboard.currentNode = null;
            }
            rootNode._actor.transform.DOKill();
            rootNode._actor.Rb.DOKill();
        }

#if UNITY_EDITOR

        public TreeNode CreateNode(System.Type type) // 노드 생성
        {
            TreeNode node = CreateInstance(type) as TreeNode;
            if (node != null)
            {
                node.name = type.Name;
                node.guid = GUID.Generate().ToString();

                Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
                nodes.Add(node);

                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }

                Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
                AssetDatabase.SaveAssets();
                return node;
            }

            return null;
        }

        public void DeleteNode(TreeNode node) // 노드 제거
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(TreeNode parent, TreeNode child) // 부모에 자식노드 추가
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            CompositeNode composite = parent as CompositeNode;

           if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }

            RootNode root = parent as RootNode;

            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (AddChild)");
                root.child = child;
                EditorUtility.SetDirty(root);
            }
        }

        public void RemoveChild(TreeNode parent, TreeNode child) // 부모에 자식 제거
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            CompositeNode composite = parent as CompositeNode;

            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }

            RootNode root = parent as RootNode;

            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");
                root.child = null;
                EditorUtility.SetDirty(root);
            }
        }
#endif

        public static List<TreeNode> GetChildren(TreeNode parent) // 자식노드 목록 가져오기
        {
            List<TreeNode> children = new();

            DecoratorNode decorator = parent as DecoratorNode;

            if (decorator && decorator.child != null)
            {
                children.Add(decorator.child);
            }
            else if (parent is CompositeNode)
            {
                return (parent as CompositeNode).children;
            }

            RootNode rootNode = parent as RootNode;
            if(rootNode && rootNode.child != null)
            {
                children.Add(rootNode.child);
            }

            return children;
        }
        public static void Traverse(TreeNode node, System.Action<TreeNode> visiter) // 행동트리 순회
        {
            if (node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }
        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<TreeNode>();

            Traverse(tree.rootNode, (n) =>
            {
                tree.nodes.Add(n);
            });

            return tree;
        }

    }
}