# Changelog

Toutes les modifications notables de ce package sont documentées ici.
Le format suit [Keep a Changelog](https://keepachangelog.com/) et le
versionnage suit [SemVer](https://semver.org/).

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
