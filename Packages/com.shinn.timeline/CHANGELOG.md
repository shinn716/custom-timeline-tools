# Changelog

All notable changes to this package are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [2.0.0] - 2026-07-16

### Added
- **Custom-UnityEvent track**: UnityEvent-style clip — drag any Object
  (GameObject, Light, custom MonoBehaviour, ...) via ExposedReference and
  invoke one of its public methods at clip start.
  - Supported parameters: void, int, float, string, bool, Vector2/3/4,
    Color, Quaternion (edited as Euler angles), any enum (EnumPopup), and
    UnityEngine.Object references (project assets only).
  - Property setters (e.g. `Light.set_intensity`) are listed in the method
    dropdown. GameObject targets list methods of all components.
  - Events fire in Play mode only; edit-mode scrubbing never invokes methods.
- "Custom UnityEvent" sample: demo scene + timeline (log a message, dim a
  light, toggle a cube).
- Clip display names on the Timeline (method call / text content).
- Package dependencies, keywords and sample descriptions in package.json.

### Changed
- Restructured as a standard embedded UPM package at
  `Packages/com.shinn.timeline` (install path changed from
  `Assets/Packages/CustomTimelineTools`).
- Namespace fixed: `Shinn.Timelinie` → `Shinn.Timeline`.
- Custom editors rewritten with SerializedProperty flow (Undo/Redo support,
  empty-state help boxes); method selection is now serialized and survives
  domain reloads.
- Distinct track colors per track type.
- Field renames with FormerlySerializedAs: `txtSzie` → `fontSize`,
  `useClipDuring` → `useClipDuration`.

### Fixed
- AVPro sample: video now replays when the timeline loops (the start flag is
  reset when the clip ends).
- AVPro sample: pausing the timeline mid-clip now pauses the video and
  resuming continues it (previously the video kept playing).
- AVPro sample: the video reliably stops when the clip ends — clip-end
  detection moved to a dedicated track mixer (`CustomAvproMediaMixer`) that
  polls input weights every frame, because the clip's `OnBehaviourPause`
  never fires when the director holds at the timeline end; the director-pause
  case still pauses/resumes the video, and `OnPlayableDestroy` stops it when
  the director stops or the graph is rebuilt.
- AVPro sample: added a clip inspector with a MediaPlayer warning, a Browse
  button that converts picked files to the correct relative path, a bounded
  Playback Rate slider (0.25-4) and field tooltips; new optional **Preload**
  opens the media paused at graph start for instant playback.
- AVPro sample: migrated to the AVPro Video 2+ API —
  `MediaPlayer.FileLocation`/`OpenVideoFromFile` (1.x, no longer public)
  replaced with `MediaPathType`/`OpenMedia`; the serialized field is renamed
  `fileLocation` → `pathType` with FormerlySerializedAs (enum values are
  unchanged, existing clips keep their setting).
- AVPro sample: track color red channel out of range (277 → 227).
- AVPro sample: NullReferenceException when the timeline ends — the track
  mixer instance has no MediaPlayer and hit an unchecked `Stop()` in
  `OnBehaviourPause`.

### Removed
- **Breaking**: Custom-Event track (`CustomEventTracker`/`CustomEventPlayable`)
  and Custom-MSG track (`CustomMsgTracker`/`CustomMsgPlayable`/
  `CustomMsgEventManager` + prefab) — replaced by Custom-UnityEvent.
  Existing timelines using these tracks will show missing scripts.
- Unused utilities: `CustomMsgUtils`, `ListToPopupAttribute`/`ListToPopupDrawer`.
- Old "Samples" sample (depended on the removed tracks).

## [1.2.0]

- Custom-Text (Text & TextMeshPro), Custom-Event, Custom-MSG,
  Custom-AVProMediaPlayer tracks.
