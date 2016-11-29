# AlphaECS

[![Join the chat at https://gitter.im/grofit/ecsrx](https://badges.gitter.im/grofit/ecsrx.svg)](https://gitter.im/grofit/ecsrx?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

AlphaECS is yet another Entity Component System framework written for Unity (but theoretically could be used elsewhere) that utilizes [UniRx](https://github.com/neuecc/UniRx) for fully reactive systems and includes support for dependency injection (we use [Zenject](https://github.com/modesttree/Zenject)). It's a fork of [EcsRx](https://github.com/grofit/ecsrx) and heavily inspired by [uFrame](https://github.com/uFrame/uFrame.github.io).

## Dependencies

- UniRx (required)
- Zenject (optional)

The **Core** framework only depends upon UniRx. The **Unity** helper classes and MonoBehaviours that bootstrap your scenes use Zenject, but feel free to create your own unity bridge to consume the core framework if you do not want the dependency.

## Quick Start

To feel comfortable with AlphaECS you'll want to be comfortable with a few different ideas:
- Entity Component System (ECS) patterns. If you're unfamiliar with these I suggest taking a quick look [here](http://www.gamedev.net/page/resources/_/technical/game-programming/understanding-component-entity-systems-r3013).
- Reactive programming. Here's a great article to get started: [The introduction to Reactive Programming you've been missing](https://gist.github.com/staltz/868e7e9bc2a7b8c1f754)
- Dependency injection. Again, we use [Zenject](https://github.com/modesttree/Zenject) and think their intro guide is pretty great.

In your Unity project:
- Install AlphaECS, UniRx, and Zenject
- Create a ProjectContext prefab and put it in a Resources folder. Add `ProjectContext`, `AlphaECSInstaller`, and `ProjectInstaller` components to the prefab and then add setup the installer references:

![image](https://cloud.githubusercontent.com/assets/6376639/20701079/fb9243da-b64b-11e6-99ab-0c9b869305a8.png)

 - Add a `SceneContext` and `SceneInstaller` to the root of your scene and setup the installer references:

 ![image](https://cloud.githubusercontent.com/assets/6376639/20701169/773484e4-b64c-11e6-9e36-fc218bc45cc0.png)


This setup accomplishes a few things. First, when you hit play, AlphaECSInstaller will setup the core systems of the framework for you automatically. Then, project installer will setup the **game specific** systems you've added as prefabs to the Resources/Kernel folder (think InputSystem, SaveSystem, MultiplayerSystem, SceneTransitionSystem, etc) as single instances and marks them as DontDestroyOnLoad. More on how to set these up in the paragraph below. Finally, the scene installer will look for any **scene specific** systems that exist in the scene  (think EnemySystem, PowerUpSystem, ShootingSystem, CameraSystem, etc) and bind them as single instances. Of course, if you're comfortable with code you can skip all of this and implement your own bootstrapping method.

There is one optional system included with the framework that allows you to take full advantage of the Unity Editor to compose your entities. It is the EntityBehaviourSystem. To add this to your project, create a new folder under `Resources` called `Kernel`, then create a new prefab there and attach the `EntityBehaviourSystem` component included as one of the **Unity** helper classes.

 ## Example Project
 - [Survival Shooter](https://github.com/tbriley/AlphaECS.SurvivalShooter)


## HEADS UP

This was not designed with performance in mind. However, it should be performant enough for most scenarios, and given its reactive nature and decoupled design you can easily replace implementations at will. Lots of people love performance metrics, but I have none and have put performance secondary to functionality.
