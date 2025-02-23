using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[Serializable]
public class LabeledObjectInfoLabeler : CameraLabeler
{
    AnnotationDefinition m_AnnotationDefinition;
    public string annotationId = "group activity label";
    public override string description => "Group activity labeling for each agent.";
    public override string labelerId => "GroupInfo";
    public UnityEngine.Perception.GroundTruth.LabelManagement.IdLabelConfig IdLabelConfig;
    protected override bool supportsVisualization => false;

    class LabeledObjectInfoAnnotationDefinition : AnnotationDefinition
    {
        public LabeledObjectInfoAnnotationDefinition(string id)
            : base(id) { }

        public override string modelType => "LabeledObjectInfoAnnotationDefinition";
        public override string description => "Group activity labeling for each agent.";
    }

    [Serializable]
    class LabeledObjectInfoAnnotation : Annotation
    {
        public LabeledObjectInfoAnnotation(AnnotationDefinition definition, string sensorId, List<ObjectInfo> groupLabel)
            : base(definition, sensorId)
        {
            this.groupLabel = groupLabel;
        }

        public List<ObjectInfo> groupLabel;
        public override bool IsValid() => true;
    }

    public class ObjectInfo
    {
        public uint groupId;
        public uint activityId;
        public string groupActivity;
        public uint instanceId;
        public string actionName;
        public int actionID;
    }

    protected override void Setup()
    {
        m_AnnotationDefinition = new LabeledObjectInfoAnnotationDefinition(annotationId);
        DatasetCapture.RegisterAnnotationDefinition(m_AnnotationDefinition);
    }

    protected override void OnBeginRendering(ScriptableRenderContext scriptableRenderContext)
    {
        if (sensorHandle.ShouldCaptureThisFrame)
        {
            var infoList = new List<ObjectInfo>();
            var labelings = Object.FindObjectsOfType<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>();

            foreach (var labeling in labelings)
            {
                if (labeling.enabled && labeling.gameObject.activeSelf)
                {
                    var gameObject = labeling.gameObject;
                    IdLabelConfig.TryGetLabelEntryFromInstanceId(labeling.instanceId, out _, out int actionID);

                    infoList.Add(new ObjectInfo
                    {
                        instanceId = labeling.instanceId,
                        actionName = labeling.labels[0],
                        actionID = actionID,
                        groupId = gameObject.GetComponentInParent<GroupID>().groupID,
                        groupActivity = gameObject.GetComponentInParent<GroupID>().activityName,
                        activityId = gameObject.GetComponentInParent<GroupID>().activityID
                    });
                }
            }

            var annotation = new LabeledObjectInfoAnnotation(m_AnnotationDefinition, sensorHandle.Id, infoList);
            sensorHandle.ReportAnnotation(m_AnnotationDefinition, annotation);
        }
    }
}
