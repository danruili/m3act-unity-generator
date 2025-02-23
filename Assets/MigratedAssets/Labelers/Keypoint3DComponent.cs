using System;
using UnityEngine.Perception.GroundTruth.DataModel;


public struct Keypoint3DComponent : IMessageProducer
{
    ///// <summary>
    ///// The label id of the entity
    ///// </summary>
    //public int labelId { get; set; }

    /// <summary>
    /// The instance id of the entity
    /// </summary>
    public uint instanceId { get; set; }

    /// <summary>
    /// Array of all of the keypoints
    /// </summary>
    public Keypoint3DValue[] keypoints3d;// { get; set; }

    /// <inheritdoc/>
    public void ToMessage(IMessageBuilder builder)
    {
        builder.AddInt("instanceId", (int)instanceId);
        //builder.AddInt("labelId", labelId);
        foreach (var keypoint3d in keypoints3d)
        {
            var nested = builder.AddNestedMessageToVector("keypoints3D");
            keypoint3d.ToMessage(nested);
        }
    }
}
