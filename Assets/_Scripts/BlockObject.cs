using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
    public class BlockObject : ScriptableObject
    {
        public Sprite[] texture; 
    }
}