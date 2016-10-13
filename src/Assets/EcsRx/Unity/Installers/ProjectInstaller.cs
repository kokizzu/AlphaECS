using UnityEngine;
using Zenject;
using EcsRx;
using System.Linq;
using System.Collections.Generic;
using EcsRx.Unity;
using UnityEngine.SceneManagement;
using UniRx;

public class ProjectInstaller : MonoInstaller
{
	public const string KernelScene = "Kernel";
	List<GameObject> KernelObjects = new List<GameObject> ();

	public override void InstallBindings()
    {
		var resources = Resources.LoadAll ("Kernel");
		foreach(var resource in resources)
		{
			var go = (GameObject)Instantiate (resource);
			DontDestroyOnLoad (go);
			KernelObjects.Add (go);
			var systems = go.GetComponentsInChildren<SystemBehaviour> ();
			foreach (var system in systems)
			{
				Container.Bind(system.GetType()).FromInstance (system).AsSingle ();
			}
		}

		foreach(var go in KernelObjects)
		{
			Container.InjectGameObject (go);
		}

//		SceneManager.LoadSceneAsync (KernelScene, LoadSceneMode.Additive).AsObservable ().Last ().Subscribe (_ =>
//		{
//			var kernelScene = SceneManager.GetSceneByName (KernelScene);
//			SceneManager.MoveGameObjectToScene (ProjectContext.Instance.gameObject, kernelScene);
//
//			foreach(var go in KernelObjects)
//			{
//				SceneManager.MoveGameObjectToScene (go, kernelScene);
//			}
//		}).AddTo(this);
    }
}