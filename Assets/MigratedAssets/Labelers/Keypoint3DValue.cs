using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.UIElements;


/// <summary>
/// The value of an individual keypoint on a keypoint component.
/// </summary>
public class Keypoint3DValue : IMessageProducer, ICloneable
{
    /// <summary>
    /// The index of the keypoint in the template file
    /// </summary>
    public int index { get; set; }

    /// <summary>
    /// The 3D location of the keypoint
    /// </summary>
    public Vector3 wPosition;// { get; set; }
    /// <summary>
    /// The state of the point,
    /// 0 = not present,
    /// 1 = keypoint is present but not visible,
    /// 2 = keypoint is present and visible
    /// </summary>
    //public int state { get; set; }

    public Vector3 lPosition;
    public Vector3 lRotation;
    public Quaternion lQuaternion;


    /// <inheritdoc/>
    public void ToMessage(IMessageBuilder builder)
    {
        builder.AddInt("index", index);
        builder.AddFloatArray("wPosition", MessageBuilderUtils.ToFloatVector(wPosition));
        builder.AddFloatArray("lPosition", MessageBuilderUtils.ToFloatVector(lPosition));
        builder.AddFloatArray("lRotation", MessageBuilderUtils.ToFloatVector(lRotation));
        builder.AddFloatArray("lQuaternion", MessageBuilderUtils.ToFloatVector(lQuaternion));
        //builder.AddInt("state", state);
    }

    public object Clone()
    {
        return new Keypoint3DValue
        {
            index = index,
            wPosition = wPosition,
            lPosition = lPosition,
            lRotation = lRotation,
            lQuaternion = lQuaternion,
            //state = state
        };
    }
}

