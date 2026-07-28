# XTools

Package Unity (UPM) contenant des extensions et outils personnalisés,
pensé pour être réutilisé facilement d'un projet à l'autre.

## Fonctionnalités actuelles

### `[ReadOnly]`

Attribut à appliquer sur un champ sérialisé pour l'afficher en lecture seule
dans l'Inspector, sans le rendre modifiable :

```csharp
using UnityEngine;
using Xoletis.EditorTools;

public class Example : MonoBehaviour
{
    [ReadOnly]
    public int computedValue;
}
```

## Structure

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

- `Runtime/` contient le code utilisable dans un build (ex. l'attribut
  `ReadOnlyAttribute`), compilé dans l'assembly `Xoletis.EditorTools.Runtime`.
- `Editor/` contient le code qui ne s'exécute qu'en mode éditeur (ex. les
  `PropertyDrawer`), compilé dans l'assembly `Xoletis.EditorTools.Editor`
  (qui référence l'assembly `Runtime`).

## Installation dans Unity depuis Git

Dans Unity, ouvrez `Window > Package Manager`, cliquez sur le `+` en haut à
gauche, puis `Install package from git URL...` et collez :

```
https://github.com/Xoletis/XTools.git
```

Vous pouvez aussi ajouter la dépendance directement dans le fichier
`Packages/manifest.json` de votre projet :

```json
{
  "dependencies": {
    "com.xoletis.xtools": "https://github.com/Xoletis/XTools.git"
  }
}
```

### Épingler une version précise

Pour figer une version (recommandé une fois le package stable), ajoutez un
tag Git (ex. `#0.1.0`) à la fin de l'URL :

```
https://github.com/Xoletis/XTools.git#0.1.0
```

### Mettre à jour

Unity ne re-vérifie pas automatiquement les mises à jour d'un package Git.
Pour récupérer la dernière version, utilisez le bouton "Update" du Package
Manager, ou supprimez puis réinstallez la dépendance.

## Ajouter un outil

Ajoutez vos scripts d'éditeur dans `Editor/` (ex. `[MenuItem]`,
`CustomEditor`, `PropertyDrawer`) et le code réutilisable en build dans
`Runtime/`. Unity les compile automatiquement dans les assemblies
correspondantes.

## Versionnage

Incrémentez `version` dans `package.json` (SemVer) et documentez les
changements dans `CHANGELOG.md` à chaque évolution notable.
