# Changelog

Toutes les modifications notables de ce package sont documentées ici.
Le format suit [Keep a Changelog](https://keepachangelog.com/) et le
versionnage suit [SemVer](https://semver.org/).

## [0.2.1] - 2026-07-28

### Fixed
- Erreur de compilation `CS0104` (référence ambiguë à `PackageInfo` entre
  `UnityEditor.PackageManager.PackageInfo` et `UnityEditor.PackageInfo`)
  dans `XToolsUpdateWindow`.

## [0.2.0] - 2026-07-28

### Added
- Menu `Tools > XTools > Update`: fenêtre d'éditeur affichant la version
  installée, vérifiant la dernière version disponible sur le dépôt Git
  (via les tags GitHub), permettant d'installer la mise à jour et
  affichant le changelog du package.

## [0.1.0] - 2026-07-28

### Added
- Structure initiale du package (`package.json`, assembly `Editor` dédiée).
