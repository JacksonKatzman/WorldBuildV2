#if VISTA
using UnityEngine;
using System.Collections.Generic;
using Pinwheel.Vista.Graph;
using Pinwheel.Vista;
using System.Collections;

namespace Game.VistaExtensions
{
	[NodeMetadata(
        title = "Location Based Tree Output",
        path = "IO/Location Based Tree Output",
        icon = "",
        documentation = "https://docs.google.com/document/d/1zRDVjqaGY2kh4VXFut91oiyVCUex0OV5lTUzzCSwxcY/edit#heading=h.6xa0ivkuvpke",
        keywords = "",
        description = "Output tree instances of a prefab (and its variants) to the terrain with location bias.")]
    public class LocationBasedTreeOutputNode : TreeOutputNode, IHasAssetReferences
    {
        public readonly MaskSlot locationsInputSlot = new MaskSlot("Locations", SlotDirection.Input, 5);
        protected static readonly int LOCATIONS_MAP = Shader.PropertyToID("_LocationsMap");
        protected static readonly string HAS_LOCATIONS_MAP_KW = "HAS_LOCATIONS_MAP";
        new protected static readonly string COMPUTE_SHADER_NAME = "Vista/Shaders/Graph/LocationBasedInstanceOutput";
        private static readonly int BASE_INDEX = Shader.PropertyToID("_BaseIndex");
        private static readonly int THREAD_PER_GROUP = 8;
        private static readonly int MAX_THREAD_GROUP = 64000;

        [System.NonSerialized]
        private TreeTemplate m_treeTemplate;
    /*    
        public TreeTemplate treeTemplate
        {
            get
            {
                return m_treeTemplate;
            }
            set
            {
                m_treeTemplate = value;
            }
        }
       */ 

        [SerializeField]
        private string m_treeTemplateGuid;

        public override bool isBypassed
        {
            get
            {
                return false;
            }
            set
            {
                m_isBypassed = false;
            }
        }

        public LocationBasedTreeOutputNode() : base()
        {
            m_treeTemplate = null;
        }
        /*  
          public void OnSerializeAssetReferences(List<ObjectRef> refs)
          {
              if (m_treeTemplate != null)
              {
                  ObjectRef r = new ObjectRef(m_treeTemplate);
                  refs.Add(r);
                  m_treeTemplateGuid = r.guid;
              }
              else
              {
                  m_treeTemplateGuid = null;
              }
          }

          public void OnDeserializeAssetReferences(List<ObjectRef> refs)
          {
              if (!string.IsNullOrEmpty(m_treeTemplateGuid))
              {
                  ObjectRef templateRef = refs.Find(r => r.guid.Equals(m_treeTemplateGuid));
                  if (!templateRef.Equals(default))
                  {
                      m_treeTemplate = templateRef.target as TreeTemplate;
                  }
              }
              else
              {
                  m_treeTemplate = null;
              }
          }
         */

        public override IEnumerator Execute(GraphContext context)
        {
            ExecuteImmediate(context);
            yield return null;
        }

        public override void ExecuteImmediate(GraphContext context)
        {
            int baseResolution = context.GetArg(Args.RESOLUTION).intValue;
            SlotRef positionInputRefLink = context.GetInputLink(m_id, positionInputSlot.id);
            ComputeBuffer positionInputBuffer;
            if (positionInputRefLink.Equals(SlotRef.invalid))
            {
                return;
            }

            positionInputBuffer = context.GetBuffer(positionInputRefLink);
            if (positionInputBuffer == null)
                return;
            if (positionInputBuffer.count % PositionSample.SIZE != 0)
            {
                UnityEngine.Debug.LogError($"Cannot parse position input buffer. Node id: {m_id}");
                context.ReleaseReference(positionInputRefLink);
                return;
            }

            SlotRef densityMapRefLink = context.GetInputLink(m_id, densityInputSlot.id);
            Texture densityMap = context.GetTexture(densityMapRefLink);

            SlotRef verticalScaleMapRefLink = context.GetInputLink(m_id, verticalScaleInputSlot.id);
            Texture verticalScaleMap = context.GetTexture(verticalScaleMapRefLink);

            SlotRef horizontalScaleMapRefLink = context.GetInputLink(m_id, horizontalScaleInputSlot.id);
            Texture horizontalScaleMap = context.GetTexture(horizontalScaleMapRefLink);

            SlotRef rotationMapRefLink = context.GetInputLink(m_id, rotationInputSlot.id);
            Texture rotationMap = context.GetTexture(rotationMapRefLink);

            SlotRef locationsMapRefLink = context.GetInputLink(m_id, locationsInputSlot.id);
            Texture locationsMap = context.GetTexture(locationsMapRefLink);

            int instanceCount = positionInputBuffer.count / PositionSample.SIZE;
            SlotRef outputRef = new SlotRef(m_id, outputSlot.id);
            DataPool.BufferDescriptor desc = DataPool.BufferDescriptor.Create(instanceCount * InstanceSample.SIZE);
            ComputeBuffer treeSamplesBuffer = context.CreateBuffer(desc, outputRef);

            if (m_computeShader == null)
            {
                m_computeShader = Resources.Load<ComputeShader>(COMPUTE_SHADER_NAME);
            }
            m_computeShader.SetBuffer(KERNEL_INDEX, INSTANCE_SAMPLE, treeSamplesBuffer);
            m_computeShader.SetBuffer(KERNEL_INDEX, POSITIONS, positionInputBuffer);

            m_computeShader.SetFloat(DENSITY_MULTIPLIER, m_densityMultiplier);
            if (densityMap != null)
            {
                m_computeShader.SetTexture(KERNEL_INDEX, DENSITY_MAP, densityMap);
                m_computeShader.EnableKeyword(HAS_DENSITY_MAP_KW);
            }
            else
            {
                m_computeShader.DisableKeyword(HAS_DENSITY_MAP_KW);
            }

            m_computeShader.SetFloat(VERTICAL_SCALE_MULTIPLIER, m_verticalScaleMultiplier);
            if (verticalScaleMap != null)

            {
                m_computeShader.SetTexture(KERNEL_INDEX, VERTICAL_SCALE_MAP, verticalScaleMap);
                m_computeShader.EnableKeyword(HAS_VERTICAL_SCALE_MAP_KW);
            }
            else
            {
                m_computeShader.DisableKeyword(HAS_VERTICAL_SCALE_MAP_KW);
            }

            m_computeShader.SetFloat(HORIZONTAL_SCALE_MULTIPLIER, m_horizontalScaleMultiplier);
            if (horizontalScaleMap != null)
            {
                m_computeShader.SetTexture(KERNEL_INDEX, HORIZONTAL_SCALE_MAP, horizontalScaleMap);
                m_computeShader.EnableKeyword(HAS_HORIZONTAL_SCALE_MAP_KW);
            }
            else
            {
                m_computeShader.DisableKeyword(HAS_HORIZONTAL_SCALE_MAP_KW);
            }

            m_computeShader.SetFloat(MIN_ROTATION, m_minRotation * Mathf.Deg2Rad);
            m_computeShader.SetFloat(MAX_ROTATION, m_maxRotation * Mathf.Deg2Rad);
            m_computeShader.SetFloat(ROTATION_MULTIPLIER, m_rotationMultiplier);
            if (rotationMap != null)
            {
                m_computeShader.SetTexture(KERNEL_INDEX, ROTATION_MAP, rotationMap);
                m_computeShader.EnableKeyword(HAS_ROTATION_MAP_KW);
            }
            else
            {
                m_computeShader.DisableKeyword(HAS_ROTATION_MAP_KW);
            }

            if (locationsMap != null)
            {
                m_computeShader.SetTexture(KERNEL_INDEX, LOCATIONS_MAP, locationsMap);
                m_computeShader.EnableKeyword(HAS_LOCATIONS_MAP_KW);
            }
            else
            {
                m_computeShader.DisableKeyword(HAS_LOCATIONS_MAP_KW);
            }

            int baseSeed = context.GetArg(Args.SEED).intValue;
            System.Random rnd = new System.Random(m_seed ^ baseSeed);
            m_computeShader.SetVector(SEED, new Vector4((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

            int totalThreadGroupX = (instanceCount + THREAD_PER_GROUP - 1) / THREAD_PER_GROUP;
            int iteration = (totalThreadGroupX + MAX_THREAD_GROUP - 1) / MAX_THREAD_GROUP;
            for (int i = 0; i < iteration; ++i)
            {
                int threadGroupX = Mathf.Min(MAX_THREAD_GROUP, totalThreadGroupX);
                totalThreadGroupX -= MAX_THREAD_GROUP;
                int baseIndex = i * MAX_THREAD_GROUP * THREAD_PER_GROUP;
                m_computeShader.SetInt(BASE_INDEX, baseIndex);
                m_computeShader.Dispatch(KERNEL_INDEX, threadGroupX, 1, 1);
            }

            context.ReleaseReference(positionInputRefLink);
            context.ReleaseReference(densityMapRefLink);
            context.ReleaseReference(verticalScaleMapRefLink);
            context.ReleaseReference(horizontalScaleMapRefLink);
            context.ReleaseReference(rotationMapRefLink);

            context.ReleaseReference(locationsMapRefLink);
        }
    }
}
#endif
