using UnityEngine;
using AlphaECS.Unity;
using Zenject;

public class SceneInstaller : MonoInstaller
{
	public SystemBehaviour[] Systems;

	public EntityBehaviour[] Entities;

	[ContextMenu("Setup Systems")]
	private void SetupSystems()
	{
		if (Systems == null || Systems.Length <= 0)
		{ Systems = GetComponentsInChildren<SystemBehaviour>(true); }
	}
	
	[ContextMenu("Setup Entities")]
	private void SetupEntities()
	{
		if (Entities == null || Entities.Length <= 0)
		{ Entities = GetComponentsInChildren<EntityBehaviour> (true); }
	}

    public override void InstallBindings()
    {
	    SetupSystems();

        foreach (var system in Systems)
        {
            Container.Bind(system.GetType()).FromInstance(system).AsSingle();
        }
    }

    public override void Start ()
	{
		base.Start ();

		SetupEntities();

//		var gameObjects = entityBehaviours.Select (eb => eb.gameObject).ToArray();
//		gameObjects.ForceEnable ();
////		gameObjects.ForceEnable (); //HACK for some reason we have to call this twice - not sure if it's a framework bug or a project specific edge case...

		foreach (var eb in Entities)
		{
			if (eb == null)
			{
#if UNITY_EDITOR
				Debug.LogWarning($"{this.gameObject.scene.name} IS MISSING AN ENTITY ON SETUP!", this.gameObject);
#endif
				continue;
			}
			if (!eb.gameObject.activeInHierarchy)
			{
				//HACK for some reason we have to call this twice - not sure if it's a framework bug or a project specific edge case...
				//i recall seeing similar things in other frameworks in the past, i guess it has something to do with the scene not technically being fully bootstrapped...
				//...or some case where a child object may not be considered fully enabled until AFTER parent has been enabled (or vice versa)...
				//...for example, enabling parent -> child in the same step, or child -> parent
				eb.gameObject.ForceEnable ();
				eb.gameObject.ForceEnable ();
			}
		}
	}

	private void Reset()
	{
		#if UNITY_EDITOR
		Systems = null;
		Systems = GetComponentsInChildren<SystemBehaviour> (true);
		Entities = null;
		Entities = GetComponentsInChildren<EntityBehaviour> (true);

		UnityEditor.EditorUtility.SetDirty(this);
		#endif
	}
}