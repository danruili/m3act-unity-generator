using System;
using System.Collections.Generic;
using UnityEngine.Perception.GroundTruth.DataModel;


/// <summary>
/// The product of the keypoint labeler
/// </summary>
public class Keypoint3DAnnotation : Annotation
{
    internal Keypoint3DAnnotation(AnnotationDefinition def, string sensorId, string templateId, List<Keypoint3DComponent> entries)
        : base(def, sensorId)
    {
        this.templateId = templateId;
        this.entries = entries;
    }

    /// <summary>
    /// The template that the points are based on
    /// </summary>
    public string templateId { get; set; }
    public IEnumerable<Keypoint3DComponent> entries { get; set; }

    /// <inheritdoc/>
    public override void ToMessage(IMessageBuilder builder)
    {
        base.ToMessage(builder);
        builder.AddString("templateId", templateId);
        foreach (var entry in entries)
        {
            var nested = builder.AddNestedMessageToVector("values");
            entry.ToMessage(nested);
        }
    }
}
