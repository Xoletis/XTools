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
  - [`EnumDictionary<TEnum, TValue>`](#enumdictionarytenum-tvalue)
  - [ScriptableObject "Open" Button](#scriptableobject-open-button)
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

### ScriptableObject "Open" Button

Any serialized field referencing a `ScriptableObject` (or a subclass)
automatically gets an "Open" button next to it in the Inspector:

```csharp
using UnityEngine;

public class Example : MonoBehaviour
{
    public MyScriptableObject config;
}
```

Clicking "Open" selects and pings the referenced asset, so its own
Inspector shows up immediately — no need to go hunt for it in the Project
window. The button is disabled when the field is empty.

This applies automatically to every `ScriptableObject` field across your
project (no attribute required). If one of your `ScriptableObject`
subtypes already has its own `[CustomPropertyDrawer]`, that more specific
drawer still takes priority, so nothing breaks.

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
