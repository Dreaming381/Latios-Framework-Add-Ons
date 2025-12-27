# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic
Versioning](http://semver.org/spec/v2.0.0.html).

## [0.1.5] – 2025-11-22

### Changed

-   Changed the in-chunk buffer capacities of `MecanimClipEvent` and
    `MecanimStateTransitionEvent` to 0

## [0.1.4] – 2025-9-13

This release only contains internal compatibility changes

## [0.1.3] – 2025-7-6

### Changed

-   Blend tree blending and transition crossfades now use frequency blending
    rather than duration blending

### Fixed

-   Fixed time scale values being inverted in blend tree clips

## [0.1.2] – 2025-5-17

### Fixed

-   Fixed baking of name hashes for states and parameters to use the fixed
    string hashers and `UnityEngine.Animator` hashers

## [0.1.1] – 2025-5-3

### Added

-   Added `StartInertialBlend()` to `MecanimAspect`
-   Added `manualInertialBlendDurationSeconds`, `performingManualInertialBlend`,
    and `StartInertialBlend()` to `MecanimController`

### Improved

-   Improved inertial blend smoothing of root motion when the timestep varies

## [0.1.0] - 2025-4-5

### This is the first release of *Mecanim V2*.

### 
