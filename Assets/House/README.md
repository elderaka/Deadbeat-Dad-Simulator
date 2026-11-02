# Rumah Kontrakan 3 Petak — Architectural Blockout v0.1

**Scale:** 1 Unity unit = 1 meter (Y-up); Bapak reference height **1.65 m**.

- Enclosed footprint **3.5 × 9.0 m**, three sequential 3-m-deep bays: front living room, middle bedroom, rear kitchen/service.
- Terrace in front: **3.5 × 1.2 m** (not part of enclosed footprint).
- Ceiling/walls: **2.8 m**; exterior walls 0.15 m, internal partitions 0.10 m.
- Rear-right bathroom: **x = 0.35..1.75, z = 7..9 m**; approximate graybox footprint.
- Z grows toward the **rear** of the house; entrance/front wall at **z = 0**. X=0 is facade center.

## Door layout — preserved from the approved denah

| Opening | Wall | X center | Width | Height |
|---|---|---:|---:|---:|
| Main entrance | Front, z=0 | 0.00 m | 0.90 m | 2.10 m |
| Living → Bedroom | Partition, z=3 | **+0.76 m (RIGHT)** | 0.85 m | 2.10 m |
| Bedroom → Kitchen | Partition, z=6 | **−0.055 m (NEAR CENTER)** | 0.85 m | 2.10 m |
| Kitchen → Bathroom | Bathroom front, z=7 | +1.05 m | 0.70 m | 2.10 m |

**Important:** The original denah did not draw a bathroom access door. The fourth opening is a necessary **documented assumption**. The other three doorway locations match the drawn denah; do not center both interior passages.

## Files

- `rumah-3-petak-v01.fbx` — FBX Binary 7.4, separate named architectural meshes.
- `layout.json` — machine-readable dimensions, openings, source-of-truth coordinates.
- `../../tools/generate_house.mjs` — dependency-free deterministic FBX generator (run with Node.js).

## Unity import notes

Copy/retain these under the Unity project's **Assets/House/** directory. Drag the FBX into a scene. Use import scale **1.0** and compare a 1.65 m character against a 2.1 m doorway; adjust importer unit conversion if Unity reports unexpected bounds.

The two `*_Roof_*_Removable` meshes are distinct and can be disabled for isometric or interior views. Rooms and doors are **openings and frames only**—there are no dynamic door leaves or colliders yet. Generate physics colliders / NavMesh in Unity; do not add a single mesh collider that blocks passageways.

**Validation caveat:** Geometry dimensions and format are programmatically constructed. Unity import has not been tested in the editor. This is a first-pass graybox (no furniture, windows, interactive doors, UV texturing or detailed roof structure).
