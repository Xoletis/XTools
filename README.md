# XTools

Custom Unity Editor extensions and tooling, packaged as a UPM package for
easy reuse across projects.

## Table of Contents

- [Installation](#installation)
  - [Via Package Manager (Git URL)](#via-package-manager-git-url)
  - [Via manifest.json](#via-manifestjson)
  - [Pinning a version](#pinning-a-version)
  - [Updating](#updating)
- [Features](#features)
  - [`[ReadOnly]`](#readonly)
  - [`[ConditionalField]`](#conditionalfield)
  - [`EnumDictionary<TEnum, TValue>`](#enumdictionarytenum-tvalue)
  - [Open-in-Inspector Button](#open-in-inspector-button)
  - [Inspector History](#inspector-history)
  - [Update Panel](#update-panel)
- [Contributing](#contributing)
- [Versioning](#versioning)

## Installation

### Via Package Manager (Git URL)

In Unity, open `Window > Package Manager`, click the `+` button in the
top-left corner, select `Install package from git URL...`, and paste:

```
https://github.com/Xoletis/XTools.git
```

### Via manifest.json

Alternatively, add the dependency directly to your project's
`Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.xoletis.xtools": "https://github.com/Xoletis/XTools.git"
  }
}
```

### Pinning a version

To lock onto a specific release (recommended once the package is stable),
append a Git tag to the URL:

```
https://github.com/Xoletis/XTools.git#0.1.0
```

### Updating

Unity does not automatically check for updates on Git packages. To pull the
latest version, use the "Update" button in the Package Manager, or remove
and reinstall the dependency.

## Features

### `[ReadOnly]`

Apply this attribute to a serialized field to display it as read-only in
the Inspector, without making it editable:

```csharp
using UnityEngine;
using Xoletis.EditorTools;

public class Example : MonoBehaviour
{
    [ReadOnly]
    public int computedValue;
}
```

### `[ConditionalField]`

Shows or hides a serialized field in the Inspector based on the value of
another field, updating live as that field changes:

```csharp
using UnityEngine;
using Xoletis.EditorTools;

public class Example : MonoBehaviour
{
    public bool useCustomSpeed;

    [ConditionalField(nameof(useCustomSpeed))]
    public float customSpeed;

    // Hide instead of show when the condition is true:
    [ConditionalField(nameof(useCustomSpeed), inverse: true)]
    public float defaultSpeed;

    public enum Mode { Simple, Advanced }
    public Mode mode;

    // Show only when the referenced field equals a specific value
    // (works with enums, ints, floats, strings and bools).
    [ConditionalField(nameof(mode), Mode.Advanced)]
    public float advancedSetting;
}
```

The condition field must be a sibling of the annotated field (same object,
or same element when both are inside a list/array). Bools, enums, ints,
floats, strings and object references are all supported as conditions —
for any type without an explicit compare value, "truthy" means non-zero /
non-empty / non-null.

### `EnumDictionary<TEnum, TValue>`

A serializable, dictionary-like structure that lets you assign a value to
each member of an enum, editable directly in the Inspector:

```csharp
using UnityEngine;
using Xoletis.EditorTools;

public enum Materiaux
{
    Fer,
    Bois,
    Pierre
}

public class Example : MonoBehaviour
{
    public EnumDictionary<Materiaux, float> weights;
}
```

In the Inspector, `weights` appears as a foldout with one field per enum
member (`Fer`, `Bois`, `Pierre`), each holding its own `float`. At runtime,
read/write values with the enum as key:

```csharp
float fer = weights[Materiaux.Fer];
weights[Materiaux.Bois] = 2.5f;

foreach (var pair in weights)
{
    Debug.Log($"{pair.Key} -> {pair.Value}");
}
```

> **Note:** values are matched to enum members by declaration order. Adding
> or removing members at the end is safe; reordering or inserting a member
> in the middle will shift the existing values, so double-check them in
> the Inspector afterwards.

### Open-in-Inspector Button

Any serialized field referencing a `UnityEngine.Object` — a
`ScriptableObject`, a `GameObject`, a `Component`, a `Material`, a
`Texture`, or any other asset type — automatically gets a small eye-icon
button next to it in the Inspector:

```csharp
using UnityEngine;

public class Example : MonoBehaviour
{
    public MyScriptableObject config;
    public Material material;
    public Transform target;
}
```

Clicking the eye selects and pings the referenced object, so its own
Inspector shows up immediately — no need to go hunt for it in the Project
window or Hierarchy. The button is disabled when the field is empty.

This applies automatically to every object reference field across your
project (no attribute required). If one of your types already has its own
more specific `[CustomPropertyDrawer]`, that drawer still takes priority,
so nothing breaks.

### Inspector History

Every Inspector now shows a small "◀ Back" button right below the object's
header, letting you jump back to the previously inspected object/asset —
similar to a browser's back button. It's disabled when there's nothing to
go back to.

> **Note:** Unity does not expose a public API to inject a control
> directly into the Inspector window's own icon row (the lock/context-menu
> icons). The button is added via the public `Editor.finishedDefaultHeaderGUI`
> event instead, right under the header — this keeps the feature stable
> across Unity versions instead of relying on internal/reflection hacks.

### Update Panel

Open `Tools > XTools > Update` to manage the package from within Unity:

- Shows the currently installed version.
- Checks the GitHub repository's tags for the latest available version.
- Lets you install the update in one click (via the Package Manager).
- Displays the package's changelog.

## Contributing

Add editor-only scripts (`[MenuItem]`, `CustomEditor`, `PropertyDrawer`,
etc.) under `Editor/`, and reusable runtime code under `Runtime/`. Unity
automatically compiles them into their respective assemblies.

## Versioning

Bump `version` in `package.json` (SemVer) and document notable changes in
`CHANGELOG.md`.
