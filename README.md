# Xoletis Editor Tools

Package Unity (UPM) contenant des extensions et outils d'éditeur personnalisés,
pensé pour être réutilisé facilement d'un projet à l'autre.

## Structure

```
com.xoletis.editor-tools/
  package.json
  README.md
  CHANGELOG.md
  Editor/
    Xoletis.EditorTools.Editor.asmdef   <- assembly Editor-only
    ...vos scripts d'éditeur ici...
```

Tous les scripts placés dans `Editor/` sont automatiquement compilés dans une
assembly dédiée qui ne s'exécute qu'en mode éditeur (jamais dans un build).

## Ajouter un outil

Ajoutez simplement vos fichiers `.cs` dans `Editor/` (sous-dossiers autorisés).
Exemple : une fenêtre d'éditeur, un `[MenuItem]`, un `CustomEditor`,
un `PropertyDrawer`, etc.

## Exporter vers un autre projet

Ce package est "embedded" : il vit directement dans le dossier `Packages/`
du projet. Pour le réutiliser ailleurs, deux options :

1. **Copier-coller** : copiez le dossier `Packages/com.xoletis.editor-tools`
   tel quel dans le dossier `Packages/` de l'autre projet. Unity le détecte
   automatiquement au prochain focus de l'éditeur.
2. **Via un dépôt Git dédié** (recommandé si le package évolue souvent) :
   - Initialisez ce dossier comme dépôt Git indépendant (ou déplacez-le dans
     un repo séparé).
   - Dans l'autre projet, ouvrez `Window > Package Manager > + > Install
     package from git URL...` et collez l'URL du repo.
   - Ou ajoutez une ligne dans `Packages/manifest.json` :
     `"com.xoletis.editor-tools": "https://github.com/<user>/<repo>.git"`

## Versionnage

Incrémentez `version` dans `package.json` (SemVer) et documentez les
changements dans `CHANGELOG.md` à chaque évolution notable.
