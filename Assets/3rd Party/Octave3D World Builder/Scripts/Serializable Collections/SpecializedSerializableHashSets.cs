#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class SerializableGameObjectHashSet : SerializableHashSet<GameObject> { }

    [Serializable]
    public class SerializableStringHashSet : SerializableHashSet<string> { }

    [Serializable]
    public class SerializablePrefabTagHashSet : SerializableHashSet<PrefabTag> { }
}
#endif