using UnityEngine;
using System.Collections;

namespace PreServer
{
    public static class Layers
    {
        public static LayerMask ignoreLayersController;

        static Layers()
        {
            ignoreLayersController = ~(1 << 3 | 1 << 8);
        }
    }
}
