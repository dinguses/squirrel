using UnityEngine;
using System.Collections;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Variables/PlayerManager")]
    public class PlayerManagerVariable : ScriptableObject
    {
        public PlayerManager value;

        public void Set(PlayerManager s)
        {
            value = s;
        }
    }
}