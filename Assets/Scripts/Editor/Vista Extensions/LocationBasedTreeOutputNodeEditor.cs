#if VISTA
using UnityEngine;
using Pinwheel.VistaEditor.Graph;
using Pinwheel.Vista.Graph;
using Pinwheel.Vista;
using UnityEditor;

namespace Game.VistaExtensions.Editor
{
	[NodeEditor(typeof(LocationBasedTreeOutputNode))]
    public class LocationBasedTreeOutputNodeEditor : InstanceOutputNodeEditorBase, INeedUpdateNodeVisual
    {
        private static readonly GUIContent TREE_TEMPLATE = new GUIContent("Tree Template", "Template for the tree");

        private Texture GetPreviewTexture(INode node)
        {
            LocationBasedTreeOutputNode n = node as LocationBasedTreeOutputNode;
            if (n.treeTemplate != null && n.treeTemplate.prefab != null)
            {
                return AssetPreview.GetAssetPreview(n.treeTemplate.prefab);
            }
            else
            {
                return null;
            }
        }

        public void UpdateVisual(INode node, NodeView nv)
        {
            nv.SetPreviewImage(GetPreviewTexture(node));
        }

        public override void OnGUI(INode node)
        {
            LocationBasedTreeOutputNode n = node as LocationBasedTreeOutputNode;
            EditorGUI.BeginChangeCheck();
            TreeTemplate template = EditorGUILayout.ObjectField(TREE_TEMPLATE, n.treeTemplate, typeof(TreeTemplate), false) as TreeTemplate;
            if (EditorGUI.EndChangeCheck())
            {
                m_graphEditor.RegisterUndo(n);
                n.treeTemplate = template;
            }
            base.OnGUI(node);
        }
    }
}
#endif
