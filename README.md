# ViewConfigurations

A Unity framework for cleanly separating view-related and business-related data using ScriptableObject-based configurations.

## Features

- Clear separation of business and view configurations using dedicated ScriptableObject types  
- Generic, type-safe mapping between configuration types  
- Support for multiple view configs per business config  
- Custom inspector for editing both configs from a single entry point  

## Installing

Use Unity Package Manager (UPM) with the following Git URL:

```
https://github.com/CliffCalist/view-configurations.git
```

## Usage

### Define Business Config

```csharp
[CreateAssetMenu(menuName = "Config/My Game Config")]
public class MyGameConfig : ScriptableObject
{
    [SerializeField] private int baseScore;
    public int BaseScore => baseScore;
}
```

### Define View Config

```csharp
[CreateAssetMenu(menuName = "Config/View/My Game Config View")]
public class MyGameViewConfig : ViewConfig<MyGameConfig>
{
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;
}
```

### Access View Config from Registry

```csharp
IViewConfigRegistry registry = ...; // resolve via DI or static provider
var viewConfig = registry.GetViewFor<MyGameViewConfig>(myGameConfig);
```

### Custom Inspector Support

- When selecting a `ViewConfig`, the linked `CoreConfig` is automatically shown as a foldout.  
- If no target is assigned, an error box and auto-creation button will be displayed.  

## Roadmap

- [x] Base generic architecture: `ScriptableObject` / `ViewConfig<T>`  
- [x] Runtime registry for config mapping  
- [x] Editor tooling  
  - [x] Custom inspector for ViewConfig  
  - [ ] Validation tools  
  - [ ] Auto-mapping and view discovery utilities  