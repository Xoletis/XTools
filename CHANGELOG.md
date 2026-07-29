# Changelog

Toutes les modifications notables de ce package sont documentées ici.
Le format suit [Keep a Changelog](https://keepachangelog.com/) et le
versionnage suit [SemVer](https://semver.org/).

## [0.4.1]

### Fixed
- Ajout des fichiers `.meta` manquants pour `ConditionalFieldAttribute.cs`
  et `ConditionalFieldDrawer.cs`, qui faisaient ignorer ces scripts par
  Unity ("has no meta file... will be ignored").

## [0.4.0]

### Added
- `[ConditionalField]` : affiche/masque un champ de l'Inspector selon la
  valeur d'un autre champ du même objet (bool, enum, int, float, string ou
  référence d'objet), avec mise à jour automatique quand la variable liée
  change.

## [0.3.0]

### Added
- `EnumDictionary<TEnum, TValue>` : structure sérialisable associant une
  valeur à chaque membre d'une enum, éditable dans l'Inspector comme un
  dictionnaire (clé = valeur de l'enum).
- Bouton (icône œil) à côté de tout champ référençant un `UnityEngine.Object`
  (asset, GameObject, Component, ScriptableObject...) dans l'Inspector,
  permettant de le sélectionner directement.
- Bouton "◀ Back" dans le header de l'Inspector, avec historique de
  sélection, permettant de revenir à l'objet précédemment inspecté.
