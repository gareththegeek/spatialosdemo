using Improbable.Unity.Visualizer;
using UnityEngine;

public class EntityName : MonoBehaviour
{
    [Require]
    Improbable.Position.Reader positionReader;

    public void OnEnable()
    {
        if (positionReader == null) return;

        name = name.Replace("(Clone)", " " + gameObject.EntityId().Id.ToString());
    }
}
