# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.2.3

### Fixed
- Dropping Weapons now does nothing if the "caster" is a pet or polymorphed.
- Only droppable Items/Weapons should now be dropped with the Drop Weapons action.

## 1.2.2

### Fixed
- Fixed a minor problem in the flanking code caused by me being a big dumb dummy.
- Fixed a not-so-minor problem which caused all sorts of nastiness when a maneuver replaced an AoO.

## 1.2.1

### Fixed
- Combat-Maneuver-as-attack-toggles are now a bit smarter and won't use "redundant" Combat Maneuvers (e.g. tripping a target that is already (going) prone).
- Tandem Trip no longer has any prerequisites

## 1.2.0

### Added
- New Feat: [QuickDraw](https://www.d20pfsrd.com/feats/combat-feats/quick-draw-combat/)
- New Ability: Drop Weapons: free action which throws the currently equipped weapon set to the ground
- Added grouping ability for Standard Action Combat Maneuvers
- Trip, Disarm and Sunder Armor can now be used in place of a melee attack (including attacks of opportunity)
- Added toggleable abilities for each of the Combat Maneuvers that can be used in place of a melee attack.

## 1.1.2

### Added
- New toggle option in the mod menu to toggle the use of the soft cover mechanic.

### Changed
- New method for calculating Soft Cover. Should yield more intuitive results, and be more accurate when dealing with units of different sizes.

### Fixed
- A potential AoO triggered by a combat maneuver now first checks if the target of the maneuver is actually engaged to the initiator.
- ContextActioBreakFree (used for break free from spells like being Entangled or Webbed) no longer gives the spellcaster an AoO.
- Prone units are now actually ignored for soft cover.

## [1.1.1]

### Fixed
- Loading a save that did not have the mod enabled previously should now correctly add all the combat maneuver actions.

## [1.1.0]

### Added
- New Feat: [Low Profile](https://www.d20pfsrd.com/feats/combat-feats/low-profile-combat/)
- New Feat: [Phalanx Formation](https://www.d20pfsrd.com/feats/combat-feats/phalanx-formation-combat/)
- Added homepage to Info.json
### Fixed
- Updated Improved Precise Shot description to include cover.
- Dead and prone units should no longer provide cover.

## [1.0.2]

### Fixed
- Certain free combat maneuvers (Aspect of the Wolf, Piledriver) still provoked attacks of opportunity.
- New text (currently Soft Cover and the Improved Combat Maneuver feat names) will be added to the localization pack even if the locale is not English.