# AlphaECS

[![Gitter](https://badges.gitter.im/AlphaECS/Lobby.svg)](https://gitter.im/AlphaECS/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=body_badge)

AlphaECS is yet another Entity Component System framework for Unity that uses [UniRx](https://github.com/neuecc/UniRx) for fully reactive systems and includes support for dependency injection (we use [Zenject](https://github.com/modesttree/Zenject)). It's a fork of [EcsRx](https://github.com/grofit/ecsrx) and heavily inspired by [uFrame](https://github.com/uFrame/uFrame.github.io).

- <a href="#introduction">Introduction</a>
- <a href="#a_better_way_to_code">A Better Way to Code</a>
- <a href="#alphaecs_overview">AlphaECS Overview</a>
- <a href="#quick_start">Quick Start</a>
- <a href="#example_project">Example Project</a>
- <a href="#dependencies">Dependencies</a>
- <a href="#final_thoughts">Final Thoughts</a>


## <a id="introduction"></a>Introduction
What follows is my own personal take on ECS based design. I try to keep it light and to the point, but I highly recommend taking a look around Google for more thorough explanations, as designing your code this way can take some getting used to.

Developing with Unity often centers around MonoBehaviours, which are a very special type of class that you can attach to your GameObjects. Let's imagine we want to create a player. We'll re-use some code from the [Unity Survival Shooter](https://unity3d.com/learn/tutorials/projects/survival-shooter-tutorial) tutorial:

```
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
}
```

A simple enough way to get started. Next, we'll add some logic for setting up our current health when the player is created and also for taking damage:

```
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;

    void Awake ()
    {
        // Set the initial health of the player.
        currentHealth = startingHealth;
    }

    public void TakeDamage (int amount)
    {
        // Reduce the current health by the damage amount.
        currentHealth -= amount;
    }    
}
```

So far, so good. We've set up simple data for the player (startingHealth and currentHealth), and some methods to transform that data (setting it up in Awake() and removing it with TakeDamage()).

How about adding some special effects when our player gets hit, like playing a sound and decreasing a health bar? With Unity, this is very easy to do:

```
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public AudioClip deathClip;

    void Awake ()
    {
        // Set the initial health of the player.
        currentHealth = startingHealth;
        playerAudio = GetComponent <AudioSource> ();
    }

    public void TakeDamage (int amount)
    {
        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;

        // Play the hurt sound effect.
        playerAudio.Play ();
    }    
}
```

It's so easy to add effects like these, how about a cool red flash that is triggered when the player gets hit, then slowly fades out? We'll add some references to the flash effect image and color, a boolean to keep track of when the player gets damaged, and a timer to fade out the effect:

```
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;

    public Image damageImage;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    bool damaged;

    void Awake ()
    {
        // Setting up the references.
        playerAudio = GetComponent <AudioSource> ();

        // Set the initial health of the player.
        currentHealth = startingHealth;
    }


    void Update ()
    {
        // If the player has just been damaged...
        if(damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;

        // Play the hurt sound effect.
        playerAudio.Play ();
    }     
}
```

Try not to get too hung up on the logic above. Just understand that our previously simple PlayerHealth class is now doing a few different things: managing health, playing sound fx, and triggering visual effects. Next, we'll add some extra effects for when the player dies, including playing a death animation and a death sound:

```
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    Animator anim;
    AudioSource playerAudio;
    bool isDead;
    bool damaged;


    void Awake ()
    {
        // Setting up the references.
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();

        // Set the initial health of the player.
        currentHealth = startingHealth;
    }


    void Update ()
    {
        // If the player has just been damaged...
        if(damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;

        // Play the hurt sound effect.
        playerAudio.Play ();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if(currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death ();
        }
    }


    void Death ()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;

        // Turn off any remaining shooting effects.
        playerShooting.DisableEffects ();

        // Tell the animator that the player is dead.
        anim.SetTrigger ("Die");

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        playerAudio.clip = deathClip;
        playerAudio.Play ();
    }       
}
```

So far, this isn't *too* bad. We're at about 100 lines of code for our PlayerHealth class, and it's mostly doing things related to player health.

We'll take a break from our PlayerHealth and spend some time getting our player moving and shooting by creating a PlayerMovement class and a PlayerShooting class and slowly adding bits of logic similar to our PlayerHealth class. I won't go into the details of these classes, just trust we've now got 3 classes for dealing with the majority of our player logic. They're all nicely encapsulated from each other and contain their own data and methods for transforming that data.

But there's one little issue. When our player dies they can still shoot and move. Our neatly separate classes need to be tied together somehow. We need to disable moving and shooting when the player's health is less than or equal to 0. Where should this logic go? Should the movement and shooting logic be checking if the player is alive? Or should this health class be in charge of disabling movement and shooting? In the tutorial, PlayerHealth ended up being responsible:

```
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);


    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;
    bool isDead;
    bool damaged;


    void Awake ()
    {
        // Setting up the references.
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> ();
        playerShooting = GetComponentInChildren <PlayerShooting> ();

        // Set the initial health of the player.
        currentHealth = startingHealth;
    }


    void Update ()
    {
        // If the player has just been damaged...
        if(damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;

        // Play the hurt sound effect.
        playerAudio.Play ();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if(currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death ();
        }
    }


    void Death ()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;

        // Turn off any remaining shooting effects.
        playerShooting.DisableEffects ();

        // Tell the animator that the player is dead.
        anim.SetTrigger ("Die");

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        playerAudio.clip = deathClip;
        playerAudio.Play ();

        // Turn off the movement and shooting scripts.
        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }       
}
```

We added some references to PlayerMovement and PlayerShooting and when the Death() method gets called, we disable those scripts. However, the fact that we were even asking the question of "where should this logic go?" is telling: as your project grows larger, you'll be asking yourself this question more frequently, and the flow of logic in your game will become increasingly more complex.

We put this troubling thought to the back of our mind for the time being. Now that we've got a nice working player, we should probably give him some things to kill. And also create some things that can kill him, depleting his health and triggering all those cool effects we just spent so much time coding. We need *enemies*. Here's a simplified version of the EnemyAttack class:

```
using UnityEngine;
using System.Collections;


public class EnemyAttack : MonoBehaviour
{
    public int attackDamage = 10;

    GameObject player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;

    void Awake ()
    {
        // Setting up the references.
        player = GameObject.FindGameObjectWithTag ("Player");
        playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent<EnemyHealth>();
    }


    void OnTriggerEnter (Collider other)
    {
        // If the entering collider is the player...
        if(other.gameObject == player)
        {
          Attack();
        }
    }

    void Attack ()
    {
        // If the player has health to lose...
        if(playerHealth.currentHealth > 0)
        {
            // ... damage the player.
            playerHealth.TakeDamage (attackDamage);
        }
    }
}
```

This is manageable. The PlayerHealth class and EnemyAttack class are both fairly small. But there are already some issues with this code that will make things difficult should you try to scale this style of programming to a larger game.

In the Awake() method of EnemyAttack we're calling FindGameObjectWithTag("Player"). This will work just fine but will cause you major problems if you later decide to rename the "Player" tag to something else. A much better (but still ill-advised) way to approach this would be to search for a type instead of a string:

```
void Awake ()
{
    // Setting up the references.
    playerHealth = (PlayerHealth)FindObjectOfType (typeof(PlayerHealth));
    player = playerHealth.gameObject;
}
```

If you later decide to rename `PlayerHealth` to `Player`, the compiler can now at least help your sort things.

However, we're still left with at least one major issue here: dependencies. Remember our PlayerHealth, PlayerMovement, and PlayerShooting classes, where we tried encapsulating things away nicely and neatly into little methods and variables to modify, but ended up having to tie them together? We've done the same thing here with our `EnemyAttack` class.

For a game that won't grow any larger than the scope of the tutorial, this will work. But start adding any kind of complexity to your project and you'll quickly find yourself drowning in this stuff. You'll be checking boolean values trying to track what's alive and what's dead or injured or healing. And calling different methods and passing along different sets of data depending on what "state" you've found. And because you encapsulated everything so nicely you'll have halfway hidden all your dependencies and will be up at 5:00 am tracking down bugs through an endless beautiful chain of method calls.

So you go to Google and then things will get worse because everyone will have a different opinion or framework or style for dealing with this mess. MVVM! MVC! Procedural-functional ABCDEFG! I know because I've been there.

## <a id="a_better_way_to_code"></a>A Better Way to Code
In the Unity Shooter tutorial, we have a PlayerHealth class and an EnemyHealth class. They both do a lot of similar things. This is great, because when we find similar bits of code in our programs, that's usually a sign that we can simplify things.

The response of many a programmer in this situation, especially coming from an object-oriented background, might then be to create a base class where you keep all your shared logic, then branch out into PlayerHealth and EnemyHealth sub-classes and override the parts that are different. This feels good at first, but will lead to almost as many headaches later on.

Instead, we can stop thinking in terms of nice neat objects and inheritance chains, and start thinking in terms of breaking our game down into little pieces of data. Our new Health class can just be:

```
public class Health
{
	public int StartingHealth;
	public int CurrentHealth;
  public bool IsDead;
}
```

That's it. No logic, no methods, no inheritance, nothing. Instead, think about COMPOSING our player out of these core pieces of data. What other pieces of data might our player have? In addition to health, we want our player to be able to shoot:

```
public class Shooter
{
  public int Damage;
  public float ShotsPerSecond;
  public float Range;
  public bool IsShooting;
}
```

We also want our player to be able to move around, so we need some input data:

```
public class Input
{
  public float HorizontalInput; // left and right arrow keys
  public float VerticalInput; // up and down arrow keys
}
```

In the Unity tutorial we had 3 fairly simple classes for PlayerHealth, PlayerMovement, and PlayerShooting classes that contained all the data and methods required for that functionality. Here we've separated our data into Health, Shooter, and Input data containers that don't do anything, and we'll need to create at least 3 additional classes that will contain our logic. This might seem like we're making things overly complicated at first. We're going from 3 classes to 6 or more. Often when I examples like this I roll my eyes. The programmer giving the example is trying to demonstrate how much tidier and more readable your classes become by separating things out. But most of the time the logic of the game isn't actually made any simpler - they've just hidden the complexity behind layers of encapsulation and neatly named methods. But just stay with me a bit. Here's our new HealthSystem, which only deals with the core health data, no special effects or other stuff:


```
public class HealthSystem
{
    private Health[] HealthComponents;

    void Start ()
    {
        // Setting up the references.
        HealthComponents = FindObjectsOfType(typeof(HealthComponent)) as HealthComponent[];

        // Set the initial health of each health component.
        foreach(HealthComponent healthComponent in HealthComponents)
        {
          healthComponent.CurrentHealth = healthComponent.StartingHealth;
        }
    }

    public void TakeDamage (HealthComponent healthComponent, int amount)
    {
        // Reduce the current health by the damage amount.
        healthComponent.CurrentHealth -= amount;

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if(healthComponent.CurrentHealth <= 0 && !healthComponent.IsDead)
        {
            healthComponent.IsDead = true;
        }
        else if(healthComponent.CurrentHealth > 0 && healthComponent.IsDead)
        {
          healthComponent.IsDead = false;
        }
    }   
}
```

Again, the reasons for doing this are not immediately clear, but will pay off hugely in the long run.

One such reason is that your enemies and your players now share a whole bunch of logic. Anything in your game that you want to have health you can add a `Health` component to and it will "just work". **I can not overstate the value of this**. It's a magical feeling to be able to start adding and removing functionality like this in realtime just by adding components to your game objects.

- Want to add health to an NPC and make them killable? Add a `Health` component.
- How about creating an enemy that can shoot? Add a `Shooting` component.
- How about something that can shoot AND has a melee attack? Create a small class that contains the type of data you'd want to have for a melee attack (Damage, AttacksPerSecond, and Range, for example), then create a system for managing that logic. Add both your `Shooting` and `Melee` components to your game object.
- How about removing all shooting from your game? Disable the ShootingSystem.

Your game basically becomes a big, flat database that you filter at a higher level to handle your movement logic, create special effects, spawn enemies, etc. When you separate your data and then read, transform, and react to that data from separate systems, it becomes almost trivial to build and test new functionality, remove functionality entirely, and just have fun making your game. Someone once described working this way as "feeling like you're physically wiring things up".

## <a id="alphaecs_overview"></a>AlphaECS Overview
**Entities**

A container for a list of components. In AlphaECS an entity is NOT a GameObject, but is instead a simple class. Each entity has a unique ID and is created via a pool. They can be created via one of two different ways:
  - Code - using the PoolManager, get a pool and use it to create an entity:
  ```
  var entity = PoolManager.GetPool().CreateEntity ();
  ```
  - Scene - add an EntityBehaviour component, which is a special MonoBehaviour included in the Unity portion of the framework, to your GameObject. You can give it a named Pool or let the framework use the default pool.

**Components**

Small containers for data. You add components to your entities to "compose" different types of objects in your game. For example, instead of creating a typical Player class with hundreds of lines of code, you define the types of data your player might have in components, then add those components to an entity. Think of it like implicitly rather than explicitly defining your objects. In most other ECS frameworks a component either a plain old C# object (ala Entitas) or a MonoBheaviour that you attach to a GameObject for easy setup of your entities in your Unity scene (ala uFrame ECS). Both approaches have their advantages and disadvantages. In AlphaECS a component is an object and thus can be either a POCO or a MonoBehaviour:
- POCO - define your class, then add it to an entity:

```
public class Shooter
{
  public int Damage;
  public float ShotsPerSecond;
  public float Range;
  public BoolReactiveProperty IsShooting;
}

public class ShootingSystem
{
  void Start()
  {
    var entity = PoolManager.GetPool().CreateEntity ();
    entity.AddComponet<Shooter>();
  }
}
```
- MonoBehaviour - define your MonoBehaviour class and add an EntityBehaviour and your MonoBheaviour to your GameObject:

**Systems** and **Groups**

Systems are where you define your logic, and groups are how you define things like `Player` and `Enemy` in ECS. Note that we're no longer using things like FindObjectsOfType, as we want our systems to be able to react to new entities being created, not just grabbing lists of things when the game starts. This is how we actually define our group of entities with Health in our HealthSystem in AlphaECS:


```
public class HealthSystem : SystemBehaviour
{
    public override void Setup ()
    {
        // define your group
        // in this example, any entity that has a `Health` component will be added to the group
        var HealthComponents = GroupFactory.Create(typeof(Health));

        // this confusing looking bit of code is just watching for when entities get added to the group
        // ObserveAdd = watch for additions
        // Subscribe = do your logic when the entity gets added
        HealthComponents.Entities.ObserveAdd ().Select(x => x.Value).StartWith(group.Entities).Subscribe (entity =>
        {
          var healthComponent = entity.GetComponent<HealthComponent>()
          healthComponent.CurrentHealth = healthComponent.StartingHealth;
        }).AddTo(this.Disposer);
    }
}
```

For a deeper dive, check out the quick start guide, example project, or give a holler in the gitter channel!

## <a id="quick_start"></a>Quick Start

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


## <a id="example_project"></a>Example Project
- [Survival Shooter](https://github.com/tbriley/AlphaECS.SurvivalShooter)


## <a id="dependencies"></a>Dependencies
- [UniRx](https://github.com/neuecc/UniRx) (Required)
- [Editor Extensions](https://github.com/tbriley/EditorExtensions) (Required)
- [Zenject](https://github.com/svermeulen/Zenject) (optional)

 The **Core** framework only depends upon UniRx. The **Unity** helper classes and MonoBehaviours that bootstrap your scenes use Zenject, but feel free to create your own unity bridge to consume the core framework if you do not want the dependency.


## <a id="final_thoughts"></a>Final Thoughts

This was not designed with performance in mind. However, it should be performant enough for most scenarios, and given its reactive nature and decoupled design you can easily replace implementations at will. Lots of people love performance metrics, but I have none and have put performance secondary to functionality.
