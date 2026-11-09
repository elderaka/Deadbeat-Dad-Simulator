# Kontrakan Deret 5 Pintu — Graybox v0.1

A **proposed five-unit** compound for Bapak Simulator. The previous single-unit FBX at `Assets/House/` is preserved. No new site-size decision has been canonized.

## Physical scale
- 1 Unity unit = 1 meter, Y-up; Bapak height reference 1.65 m.
- Five houses, each **3.5 × 9 × 2.8 m**, at X centers **−7, −3.5, 0, +3.5, +7 m**, with Unit 1 as the player's home.
- Each unit preserves the *approved denah*: main entrance centered (X=0, Z=0), living→bedroom opening **right-offset +0.76 m** (Z=3), bedroom→kitchen opening **near center −0.055 m** (Z=6).
- Rear-right bathroom access at X=+1.05, Z=7 is still a **provisional** location (not drawn on original plan).
- Three 3m deep interior zones per unit; front living, middle bedroom, rear kitchen/service. Terrace 1.2m.

## Complex layout
- **19.5 × 17m proposed footprint** (X ±9.75, Z −6.5 to +10.5).
- Shared courtyard **5.3m depth** (Z −6.5 to −1.2) plus 1.2m terraces.
- Rear service strip **1.5m** (Z 9–10.5); each side has a **1m** access strip.
- Front gate opening **3.2m**, open in the FBX until a working gate prefab exists.
- Parking-bay markings, shared drain, benches/planter, utility/wash/laundry placeholders.
- Two independently removable continuous roof panels: `Roof_Front_Removable`, `Roof_Rear_Removable`.

## Important dimension correction
The generated concept drawing labels **18 × 14.5m**, but 9m rooms + 1.2m terrace + 4m separate court + 1.5m rear strip require **15.7m depth**, before circulation tuning. This first playable layout is enlarged to 19.5 × 17m for a usable 5.3m court. Do not treat this as a construction blueprint.

## Files and Unity import
- `kontrakan-row-5-v01.fbx` — binary FBX 7.4 architectural graybox with U01–U05 named meshes, shared grounds, roof.
- `layout.json` — all coordinates, rooms, door centers, shared areas.
- `../../tools/generate_kontrakan_complex.mjs` — Node 22+ reproducible geometry exporter.

In Unity, place under `Assets/Housing/`; import at scale 1, verify a 1.65m capsule and 2.1m door openings; remove roof panels for cutaway views. Create colliders/navmesh separately—do not apply one compound mesh collider blindly.

**Limitations:** no interactive door leaves, no finished textures/UVs, no modeled scooters, no animated NPCs, no tested Unity Editor import. This graybox includes *functional placeholder* props and does not yet add detailed facade windows. Separate party walls are modeled once rather than double-stacking walls.

**2026-11-05 update:** Fixed FBX checksum/coordinate serialization after actual Unity import failed. The regenerated compound imports in Unity 6000.4.4f1 at the source layout coordinates. The walkable scene passes all fifteen primary doorway checks, floor/wall collision checks and character scale checks. Manual walkthrough remains deferred; see the [initialization handoff](../../docs/journal/2026-11-05-unity-initialization.md).
