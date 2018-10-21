using System.Collections.Generic;
using UnityEngine;
namespace StrangeEngine
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Worlds")]
    public class Worlds : ScriptableObject
    {
        public List<World> worlds;
    }
}