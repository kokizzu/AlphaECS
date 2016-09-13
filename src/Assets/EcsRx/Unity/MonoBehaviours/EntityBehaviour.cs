using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using System;
using EcsRx;
using EcsRx.Json;

namespace EcsRx.Unity
{
    public class EntityBehaviour : ComponentBehaviour
	{
		public IPool Pool { get; set; }
		public IEntity Entity { get; set; }

		[SerializeField]
		public string PoolName;

		[SerializeField]
		public List<string> CachedComponents = new List<string>();

		[SerializeField]
		public List<string> CachedProperties = new List<string>();
    }
}