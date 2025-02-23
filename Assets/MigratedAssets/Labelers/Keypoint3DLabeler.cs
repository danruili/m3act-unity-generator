using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class Keypoint3DLabeler : CameraLabeler
{

    public KeypointTemplate activeTemplate;
    public UnityEngine.Perception.GroundTruth.LabelManagement.IdLabelConfig idLabelConfig;

    public override string description => "Produces annotations for 3D keypoint positions for all visible labeled objects that have a humanoid animation avatar component.";
    protected override bool supportsVisualization => false;
    public string annotationId = "keypoints3D";
    public override string labelerId => annotationId;

    AnnotationDefinition m_AnnotationDefinition;

    struct CachedData
    {
        public bool status;
        public Animator animator;
        public Keypoint3DComponent keypoints;
        public List<(JointLabel, int)> overrides;
        public float occlusionScalar;
    }

    Dictionary<uint, CachedData> m_KnownStatus;


    public Keypoint3DLabeler() { }
    public Keypoint3DLabeler(UnityEngine.Perception.GroundTruth.LabelManagement.IdLabelConfig config, KeypointTemplate template)
    {
        this.idLabelConfig = config;
        this.activeTemplate = template;
    }

    /// <inheritdoc/>
    protected override void Setup()
    {
        if (idLabelConfig == null)
            throw new InvalidOperationException($"{nameof(Keypoint3DLabeler)}'s idLabelConfig field must be assigned");
        m_KnownStatus = new Dictionary<uint, CachedData>();

        m_AnnotationDefinition = new Keypoint3DAnnotationDefinition(annotationId, TemplateToJson(activeTemplate, idLabelConfig));
        DatasetCapture.RegisterAnnotationDefinition(m_AnnotationDefinition);   
    }


    /// <inheritdoc/>
    protected override void OnBeginRendering(ScriptableRenderContext scriptableRenderContext)
    {
        var keypointEntries = new List<Keypoint3DComponent>();

        foreach (var label in UnityEngine.Perception.GroundTruth.LabelManagement.LabelManager.singleton.registeredLabels)
            ProcessLabel(label, keypointEntries);

        var annotation = new Keypoint3DAnnotation(m_AnnotationDefinition, sensorHandle.Id, activeTemplate.templateID, keypointEntries);
        perceptionCamera.SensorHandle.ReportAnnotation(m_AnnotationDefinition, annotation);
    }



    bool TryToGetTemplateIndexForJoint(KeypointTemplate template, JointLabel joint, out int index)
    {
        index = -1;

        foreach (var label in joint.labels)
        {
            for (var i = 0; i < template.keypoints.Length; i++)
            {
                if (template.keypoints[i].label == label)
                {
                    index = i;
                    return true;
                }
            }
        }

        return false;
    }

    void ProcessLabel(UnityEngine.Perception.GroundTruth.LabelManagement.Labeling labeledEntity, List<Keypoint3DComponent> keypointEntries)
    {
        if (!idLabelConfig.TryGetLabelEntryFromInstanceId(labeledEntity.instanceId, out var labelEntry))
            return;

        if (!m_KnownStatus.ContainsKey(labeledEntity.instanceId))
        {
            var cached = new CachedData()
            {
                status = false,
                animator = null,
                keypoints = new Keypoint3DComponent(),
                overrides = new List<(JointLabel, int)>(),
                occlusionScalar = 1.0f
            };

            var entityGameObject = labeledEntity.gameObject;

            cached.keypoints.instanceId = labeledEntity.instanceId;
            cached.keypoints.keypoints3d = new Keypoint3DValue[activeTemplate.keypoints.Length];
            for (var i = 0; i < cached.keypoints.keypoints3d.Length; i++)
            {
                cached.keypoints.keypoints3d[i] = new Keypoint3DValue { index = i };
            }

            var animator = entityGameObject.transform.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                cached.animator = animator;
                cached.status = true;
            }

            foreach (var joint in entityGameObject.transform.GetComponentsInChildren<JointLabel>())
            {
                if (TryToGetTemplateIndexForJoint(activeTemplate, joint, out var idx))
                {
                    cached.overrides.Add((joint, idx));
                    cached.status = true;
                }
            }

            var occlusionOverrider = labeledEntity.GetComponentInParent<UnityEngine.Perception.GroundTruth.Labelers.KeypointOcclusionOverrides>();
            if (occlusionOverrider != null)
            {
                cached.occlusionScalar = occlusionOverrider.distanceScale;
            }

            m_KnownStatus[labeledEntity.instanceId] = cached;
        }

        var cachedData = m_KnownStatus[labeledEntity.instanceId];
        if (cachedData.status)
        {
            var animator = cachedData.animator;

            //var listStart = checkLocations.Length;
            //checkLocations.Resize(checkLocations.Length + activeTemplate.keypoints.Length, NativeArrayOptions.ClearMemory);
            //grab the slice of the list for the current object to assign positions in
            //var checkLocationsSlice = new NativeSlice<float3>(checkLocations, listStart);

            var cameraPosition = perceptionCamera.transform.position;
            var cameraforward = perceptionCamera.transform.forward;

            // Go through all of the rig keypoints and get their location
            for (var i = 0; i < activeTemplate.keypoints.Length; i++)
            {
                var pt = activeTemplate.keypoints[i];
                if (pt.associateToRig)
                {
                    var bone = animator.GetBoneTransform(pt.rigLabel);
                    if (bone != null)
                    {
                        var keypoints3d = cachedData.keypoints.keypoints3d;
                        keypoints3d[i].wPosition = bone.position;
                        keypoints3d[i].lPosition = bone.localPosition;
                        keypoints3d[i].lQuaternion = bone.localRotation;
                        keypoints3d[i].lRotation = bone.localRotation.eulerAngles;
                    }
                }
            }

            // Go through all of the additional or override points defined by joint labels and get
            // their locations
            foreach (var (joint, templateIdx) in cachedData.overrides)
            {
                var jointTransform = joint.transform;
                var jointPosition = jointTransform.position;
                //float resolvedSelfOcclusionDistance;
                //if (joint.overrideSelfOcclusionDistance)
                //    resolvedSelfOcclusionDistance = joint.selfOcclusionDistance;
                //else
                //    resolvedSelfOcclusionDistance = activeTemplate.keypoints[templateIdx].selfOcclusionDistance;

                //resolvedSelfOcclusionDistance *= cachedData.occlusionScalar;
                //var jointSelfOcclusionDistance = JointSelfOcclusionDistance(joint.transform, jointPosition, cameraPosition, cameraforward, resolvedSelfOcclusionDistance);
                var keypoints3d = cachedData.keypoints.keypoints3d;
                keypoints3d[templateIdx].wPosition = joint.transform.position;
                keypoints3d[templateIdx].lPosition = joint.transform.localPosition;
                keypoints3d[templateIdx].lQuaternion = joint.transform.localRotation;
                keypoints3d[templateIdx].lRotation = joint.transform.localRotation.eulerAngles;
                //InitKeypoint(jointPosition, cachedData, templateIdx);
            }

            var cachedKeypointEntry = cachedData.keypoints;
            var keypointEntry = new Keypoint3DComponent
            {
                instanceId = cachedKeypointEntry.instanceId,
                keypoints3d = DeepCopyKeypoints(cachedKeypointEntry.keypoints3d),
            };
            keypointEntries.Add(keypointEntry);
        }
    }

    Keypoint3DValue[] DeepCopyKeypoints(IReadOnlyList<Keypoint3DValue> values)
    {
        var copied = new Keypoint3DValue[values.Count];
        for (var i = 0; i < values.Count; i++)
        {
            copied[i] = values[i].Clone() as Keypoint3DValue;
        }

        return copied;
    }

    //void InitKeypoint(Vector3 position, CachedData cachedData, int idx)
    //{
    //    var keypoints3d = cachedData.keypoints.keypoints3d;
    //    keypoints3d[idx].position = position;
    //}



    Keypoint3DAnnotationDefinition.Template TemplateToJson(KeypointTemplate input, UnityEngine.Perception.GroundTruth.LabelManagement.IdLabelConfig labelConfig)
    {
        var json = new Keypoint3DAnnotationDefinition.Template
        {
            templateId = input.templateID,
            templateName = input.templateName,
            keyPoints = new Keypoint3DAnnotationDefinition.JointDefinition[input.keypoints.Length],
            skeleton = new Keypoint3DAnnotationDefinition.SkeletonDefinition[input.skeleton.Length]
        };

        for (var i = 0; i < input.keypoints.Length; i++)
        {
            json.keyPoints[i] = new Keypoint3DAnnotationDefinition.JointDefinition
            {
                label = input.keypoints[i].label,
                index = i,
                color = input.keypoints[i].color
            };
        }

        for (var i = 0; i < input.skeleton.Length; i++)
        {
            json.skeleton[i] = new Keypoint3DAnnotationDefinition.SkeletonDefinition
            {
                joint1 = input.skeleton[i].joint1,
                joint2 = input.skeleton[i].joint2,
                color = input.skeleton[i].color
            };
        }

        return json;
    }

}
