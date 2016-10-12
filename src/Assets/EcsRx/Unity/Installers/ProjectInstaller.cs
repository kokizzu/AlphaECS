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

    public override void InstallBindings()
    {
		SceneManager.LoadScene (KernelScene, LoadSceneMode.Additive);
		var kernelScene = SceneManager.GetSceneByName (KernelScene);
		SceneManager.MoveGameObjectToScene (ProjectContext.Instance.gameObject, kernelScene);

		var systemTypes = GetComponentsInChildren<SystemBehaviour> ();
		foreach (var system in systemTypes)
		{
			Container.Bind(system.GetType()).FromInstance (system).AsSingle ();
		}
    }
}