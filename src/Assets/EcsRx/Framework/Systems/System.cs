using UnityEngine;
using UniRx;
using Zenject;
using System.Collections;
using System;

namespace EcsRx
{
	public abstract class System : ISystem, IDisposableContainer, IDisposable
	{		
		[Inject] public IEventSystem EventSystem { get; set; }

		protected CompositeDisposable _disposer = new CompositeDisposable();
		public CompositeDisposable Disposer
		{
			get { return _disposer; }
			set { _disposer = value; }
		}
						
		public virtual void Setup ()
		{
			
		}

		public virtual IEnumerator SetupAsync ()
		{
			yield break;
		}

		public virtual void Dispose ()
		{
			Disposer.Dispose ();
		}
	}
}
