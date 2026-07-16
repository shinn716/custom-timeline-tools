# CustomTimelineTools

Timeline Playable, Support: Custom-Text / Custom-UnityEvent

Install via Package Manager (Add package from git URL):

```
https://github.com/shinn716/CustomTimelineTools.git?path=Packages/com.shinn.timeline
```
  
<img src="https://github.com/shinn716/CustomTimelineTools/blob/main/imgs/tutorial.gif" width="70%"/></a>

## Custom Text playable asset
* Step 1: Add text playable asset.  
<img src="https://github.com/shinn716/CustomTimelineTools/blob/main/imgs/Snipaste_2022-03-07_15-58-38.png" width="70%"/></a>
* Step 2: Set parameter in timeline.  
<img src="https://github.com/shinn716/CustomTimelineTools/blob/main/imgs/Snipaste_2022-03-07_16-00-20.png" width="70%"/></a>

## Custom UnityEvent playable asset

Replaces the old Custom-Event / Custom-MSG tracks (v2.0.0).
Drag any Object and invoke one of its public methods at clip start, UnityEvent-style.

* Step 1: Add a `Custom Unity Event Tracker` to the timeline and create a clip.
* Step 2: In the clip inspector, drag a target into `Target` — a GameObject
  (lists methods of the GameObject and all of its components), or a single
  component such as a `Light` or a custom `MonoBehaviour`.
* Step 3: Pick a method from the dropdown and set the argument.

Supported parameter types: none(void), `int`, `float`, `string`, `bool`,
`Vector2/3/4`, `Color`, `Quaternion` (edited as Euler angles), any `enum`,
and `UnityEngine.Object` references (project assets only — scene objects
cannot be serialized on a clip asset). Property setters (e.g.
`Light.set_intensity`) are listed as well.

Events fire in Play mode only; scrubbing the timeline in edit mode never
invokes methods. Import the **Custom UnityEvent** sample from Package Manager
for a demo scene and timeline.

## Samples

* **Custom UnityEvent** — demo scene + timeline (log a message, dim a light, toggle a cube).
* **TextMeshPro Custom-Text** — TMP variant of the text track.
* **AVPro Custom-MediaPlayer** — AVPro media player track.
