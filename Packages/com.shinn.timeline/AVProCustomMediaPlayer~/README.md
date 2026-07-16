# AVPro Custom-MediaPlayer Sample

Requires the [AVPro Video](https://renderheads.com/products/avpro-video/)
plugin (2.x or newer) — this sample does not compile without it.

## Setup

The sample scene cannot pre-reference AVPro components (the plugin's scripts
live outside this package), so finish the wiring after import:

1. Open `CustomAvproSample.unity`.
2. Select `MediaPlayer (add AVPro MediaPlayer here)` and add AVPro's
   **MediaPlayer** component.
3. Select `Video Screen (add AVPro ApplyToMesh here)` and add AVPro's
   **ApplyToMesh** component, then set its Media property to the MediaPlayer.
4. Select the `Timeline` object, open the Timeline window, click the
   `MyVideo.mp4` clip and drag the MediaPlayer into the clip's
   **Media Player** field.
5. The clip defaults to `Relative To Streaming Assets Folder` with path
   `MyVideo.mp4` — put a video at `Assets/StreamingAssets/MyVideo.mp4`
   or change the clip's `Path Type` / `Path` (the Browse button converts
   picked files to the right relative path).
6. Enter Play mode; the clip opens and plays the video at 1s and stops the
   player when the clip ends.

## Behaviour notes

- Opening media takes a moment; enable **Preload** on the clip to open the
  media (paused) when the timeline starts so playback begins instantly.
  Only use it when the MediaPlayer is used by a single clip.
- Pausing the timeline mid-clip pauses the video; resuming continues it.
- When the timeline loops, the clip seeks back to 0 and replays.
- **Loop** loops the video while the clip is active; playback still stops
  when the clip ends.
