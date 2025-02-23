using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.Rendering;

public class CustomLabeler : CameraLabeler
{
    public override string description => "Demo labeler";
    public override string labelerId => "Demo labeler";
    protected override bool supportsVisualization => false;

    public GameObject targetLight;
    public GameObject target;

    MetricDefinition lightMetricDefinition;
    AnnotationDefinition targetPositionDef;

    class TargetPositionDef : AnnotationDefinition
    {
        public TargetPositionDef(string id)
            : base(id) { }

        public override string modelType => "targetPosDef";
        public override string description => "The position of the target in the camera's local space";
    }

    [Serializable]
    class TargetPosition : Annotation
    {
        public TargetPosition(AnnotationDefinition definition, string sensorId, Vector3 pos)
            : base(definition, sensorId)
        {
            position = pos;
        }

        public Vector3 position;

        public override void ToMessage(IMessageBuilder builder)
        {
            base.ToMessage(builder);
            builder.AddFloatArray("position", MessageBuilderUtils.ToFloatVector(position));
        }

        public override bool IsValid() => true;

    }

    protected override void Setup()
    {
        lightMetricDefinition =
            new MetricDefinition(
                "LightMetric",
                "lightMetric1",
                "The world-space position of the light");
        DatasetCapture.RegisterMetric(lightMetricDefinition);

        targetPositionDef = new TargetPositionDef("target1");
        DatasetCapture.RegisterAnnotationDefinition(targetPositionDef);
    }

    protected override void OnBeginRendering(ScriptableRenderContext scriptableRenderContext)
    {
        //Report the light's position by manually creating the json array string.
        var lightPos = targetLight.transform.position;
        var metric = new GenericMetric(new[] { lightPos.x, lightPos.y, lightPos.z }, lightMetricDefinition);
        DatasetCapture.ReportMetric(lightMetricDefinition, metric);

        //compute the location of the object in the camera's local space
        Vector3 targetPos = perceptionCamera.transform.worldToLocalMatrix * target.transform.position;

        //Report using the PerceptionCamera's SensorHandle if scheduled this frame
        var sensorHandle = perceptionCamera.SensorHandle;

        if (sensorHandle.ShouldCaptureThisFrame)
        {
            var annotation = new TargetPosition(targetPositionDef, sensorHandle.Id, targetPos);
            sensorHandle.ReportAnnotation(targetPositionDef, annotation);
        }
    }
}

// Example metric that is added each frame in the dataset:
// {
//   "capture_id": null,
//   "annotation_id": null,
//   "sequence_id": "9768671e-acea-4c9e-a670-0f2dba5afe12",
//   "step": 1,
//   "metric_definition": "lightMetric1",
//   "values": [
//      96.1856,
//      192.675964,
//      -193.838638
//    ]
// },

// Example annotation that is added to each capture in the dataset:
// {
//     "annotation_id": "target1",
//     "model_type": "targetPosDef",
//     "description": "The position of the target in the camera's local space",
//     "sensor_id": "camera",
//     "id": "target1",
//     "position": [
//         1.85350215,
//         -0.253945172,
//         -5.015307
//     ]
// }
