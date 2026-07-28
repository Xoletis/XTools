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
- [Package Structure](#package-structure)
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

## Package Structure

```
com.xoletis.xtools/
  package.json
  README.md
  CHANGELOG.md
  Runtime/
    Xoletis.EditorTools.Runtime.asmdef
    ReadOnlyAttribute.cs
  Editor/
    Xoletis.EditorTools.Editor.asmdef
    ReadOnlyDrawer.cs
```

- `Runtime/` contains build-safe code (e.g. `ReadOnlyAttribute`), compiled
  into the `Xoletis.EditorTools.Runtime` assembly.
- `Editor/` contains editor-only code (e.g. `PropertyDrawer` classes),
  compiled into the `Xoletis.EditorTools.Editor` assembly, which references
  `Runtime`.

## Contributing

Add editor-only scripts (`[MenuItem]`, `CustomEditor`, `PropertyDrawer`,
etc.) under `Editor/`, and reusable runtime code under `Runtime/`. Unity
automatically compiles them into their respective assemblies.

## Versioning

Bump `version` in `package.json` (SemVer) and document notable changes in
`CHANGELOG.md`.
