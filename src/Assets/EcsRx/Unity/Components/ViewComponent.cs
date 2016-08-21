using EcsRx;
using UnityEngine;

namespace EcsRx.Unity
{
    public class ViewComponent : IComponent
    {
        public bool DestroyWithView { get; set; }
        public GameObject View { get; set; }
    }
}