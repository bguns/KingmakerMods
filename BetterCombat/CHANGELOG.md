# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.1.2

### Changed
- New method for calculating Soft Cover. Slower, but still ok and much more easy to tweak.

### Fixed
- A potential AoO triggered by a combat maneuver now first checks if the target of the maneuver is actually engaged to the initiator.
- ContextActioBreakFree (used for break free from spells like being Entangled or Webbed) no longer gives the spellcaster an AoO.

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