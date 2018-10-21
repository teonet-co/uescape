using System.Collections.Generic;
using UnityEngine;
namespace StrangeEngine
{
    [CreateAssetMenu(menuName = "Weapon")]
    public class Weapons : ScriptableObject
    {
        public List<string> wep;
        public List<string> toggles;
        public List<GameObject> prefabs;
        public Texture t;
        public Font MainFont;
    }
}

