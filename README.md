# View Configurations

A Unity framework for cleanly separating view-related and business-related data using ScriptableObject-based configurations.

## Features

- Decouples view and business configurations while enabling clean mapping  
- Supports both direct and indirect view config assignment  
- Automatic UI-based registries with target inference  
- Inspector tooling with live preview and validation  
- Unified access to view configs via a runtime service  

## Installing

Use Unity Package Manager (UPM) with the following Git URL:

```
1. https://github.com/CliffCalist/editor-flex-list.git
2. https://github.com/CliffCalist/view-configurations.git
```

## Usage

There are two main ways to associate view configurations with business data: **Direct** and **Indirect**.

---

### Direct View Configs

This approach directly links a ViewConfig to a specific business config via a serialized reference.  
It's convenient when your business config is a `ScriptableObject` asset.

#### Example: Business Config
```csharp
public class CharacterConfig : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private int _maxHealth;

    public string Id => _id;
    public int MaxHealth => _maxHealth;
}
```

#### Example: View Config
```csharp
public class CharacterViewConfig : DirectViewConfig<CharacterConfig>
{
    [SerializeField] private Sprite _portrait;

    public Sprite Portrait => _portrait;
}
```

#### Example: Registry
```csharp
[CreateAssetMenu(menuName = "Registry/Character View Configs")]
public class CharacterViewConfigRegistry : DirectViewConfigRegistry<CharacterConfig, CharacterViewConfig>
{ }
```

---

### Indirect View Configs

This pattern is more flexible. Instead of directly storing the target, the config holds a reference to a provider  
(registry, context, etc.) and resolves the target dynamically (commonly by ID or key). Useful when business config is not a separate SO asset.

#### Example: Business Config
```csharp
public class CharacterModel
{
    public string Id;
    public int MaxHealth;
}
```

#### Example: Business Config Registry
```csharp
[CreateAssetMenu(menuName = "Registry/Character Models")]
public class CharacterModelRegistry : ScriptableObject
{
    [SerializeField] private List<CharacterModel> _models;

    public CharacterModel GetById(string id)
    {
        return _models.FirstOrDefault(x => x.Id == id);
    }
}
```

#### Example: View Config
```csharp
public class CharacterViewConfig : IndirectViewConfig<CharacterModel, CharacterModelRegistry>
{
    [SerializeField] private string _characterId;

    protected override CharacterModel GetTarget(CharacterModelRegistry provider)
    {
        return provider.GetById(_characterId);
    }
}
```

#### Example: Registry
```csharp
[CreateAssetMenu(menuName = "Registry/Character View Configs")]
public class CharacterViewConfigRegistry : IndirectViewConfigRegistry<CharacterModel, CharacterModelRegistry, CharacterViewConfig>
{ }
```

---

### View Config Service

The runtime service `ViewConfigService` allows querying for ViewConfigs using the associated business config.

```csharp
var viewConfig = ViewConfigService.GetConfigFor<CharacterViewConfig>(characterConfig);
```

> Note: A single business config can be targeted by multiple view configs. However, for each view config type, only one instance should point to a specific business config. The service does not guarantee which instance you’ll receive if multiple configs of the same type target the same business object. This allows you to define multiple view representations for a single piece of business data, while maintaining unique type-based resolution.

- Internally, the service uses all registered view config registries to find a config whose `Target` matches the business config.
- This enables automatic mapping between logic and presentation layers.

Since every view config is designed to represent a specific business config, you can access that config at runtime via the `.Target` property:

```csharp
var character = characterViewConfig.Target;
```

This allows game logic or UI components to retrieve the underlying business data tied to a view config, without needing to maintain a separate reference.

#### Registering View Config Registries
Before querying, registries must be registered to the service:

```csharp
ViewConfigService.AddRegistry(someRegistry);
ViewConfigService.RemoveRegistry(someRegistry);
ViewConfigService.ClearRegistries();
```

### Display Names in Registries

Every `ViewConfig` exposes a virtual `AssociatedName` property that defines how it's displayed in registry inspectors. This improves usability when working with large lists.

- If the associated business config (`Target`) is null, the fallback display name is `"Target is null"`.
- If the target is a `UnityEngine.Object`, the inspector displays `target.name`.
- If the target is not a Unity object, the default display becomes `"Element {index}"`.

To customize the display name, override the `AssociatedName` property in your `ViewConfig`:
```csharp
public override string AssociatedName => _characterId; // or any string logic
```
> Returning `null` will fallback to Unity-style indexed display names.

## Roadmap

- [x] Direct and indirect config mapping  
- [x] Runtime service-based resolution  
- [x] Registry system  
- [x] UI Toolkit custom inspectors  
- [ ] Inspector search and filtering  
- [ ] Asset validation utilities 