---
title: "Unity Project Context"
type: context
status: living
updated: 2026-11-10
---

# Unity Project Context

## Bobber gravity and bank lift — 2026-11-10

- The float is a gravity-enabled Rigidbody. A cast gives it launch velocity, and the water volume applies buoyancy and drag after splashdown. The cast waits for actual water contact.
- A hooked fish is reeled toward the water-facing edge of the bank at water depth. Once the fish is close and has eased off its run, the line lifts it above the bank lip, then draws it toward the rod. Lift speed and reel reduction are authored in Pond Fishing.
- The scripted Play Mode check passed physical flight, flotation, underwater retrieve, bank lift, landing, and gravity after the catch.

## Physical pond checkpoint — 2026-11-10

- Kolam now has a recessed 1.19 m water volume, a basin collider, a shallow exit shelf, and an animated translucent URP water material. The pond carves the old flat navigation surface so walking NPCs route around it.
- Three scene-authored fish patrol separately. `PondWater` discovers an available fish within the bait radius. `FishingFish` applies buoyancy, drag, and swimming forces to its Rigidbody. The line applies a damped spring force to the hooked body. A landed fish falls under gravity and stays out of the catch pool for the session.
- Unity compiled without errors. A scripted Play Mode probe verified submerged movement, gravity in air, settling back into water, physical bait approach, line payout, lifting a fish clear of the surface, and falling after landing. A Unity camera preview verified the rippled surface and visible fish. Population replenishment and player swimming are later work.

## Fish fight and HUD checkpoint — 2026-11-09

- The first hooked fish alternates between runs and recovery while it has stamina. Its world position moves inside the pond during runs. Releasing reel input lets the drag pay out line. Reeling during recovery shortens the line. Holding reel against a running fish builds strain and can snap the line.
- The fish references `Assets/Fishing/pond-fight.asset` for run speed, run and recovery duration, stamina, drag payout, tension response, and landing distance. The fishing HUD shows bite cues, reel guidance, tension, and remaining line length. The rod dips as tension rises.
- The Play Mode probe verified the world fish run and line payout, bite and danger cues, HUD text fit, tension recovery on release, a successful landing, and a snapped line with no catch credit. The probe used scripted reel input. Hands-on feel, reel audio, and touch remain untested.

## Family commute directions — 2026-11-09

- The village now authors four separate trips in the travel panel: Antar Anak ke sekolah, Jemput Anak dari sekolah, Antar Ibu ke Pabrik Tekstil, and Jemput Ibu dari Pabrik Tekstil. The two NPCs use the same escort implementation with different direction labels and arrival references.
- Each leg shows pending, underway, or completed for the current day. The child arriving home can complete the school pickup obligation. School drop-off and Ibu's commute only complete their travel legs. Ibu's option to discuss Anak's pickup remains separate.
- A Play Mode probe verified all four NavMesh arrivals, the separate school pickup credit, panel fit, and next-day reset. The probe used accelerated NPC movement and staged Bapak at the stops.

## Manual versus paid pickup recap — 2026-11-09

- In the village, Anak's school interaction starts the return escort. The pickup is complete when Anak arrives home during the pickup window. The route consumes the ordinary clock time. The fixed-duration pickup shortcut remains in the original Kontrakan fixture.
- The end-of-day HUD reports pickup outcome and provider, expenses, fishing minutes, catches, and first-cast time. It shows the previous day's provider, expenses, fishing time, and catches for comparison. `PickupDay` owns the summary and counts active fishing as the shared clock advances.
- A scripted two-day Play Mode probe exercised both escort legs, successful manual completion without the extra duration, the paid pickup charge, first cast, catches, and the final fishing minute before automatic day end. The probe confirmed that the recap fit its text container. The test used accelerated escort motion and scripted clock advancement.

## In-world fishing checkpoint — 2026-11-09

- In the village, start the day at Rumah and interact at Kolam to equip the rod. Aim at the pond, press the cast action, then hold it to reel. The float dips during the hook window. Early reeling retrieves an empty line. A successful hook attaches the visible line to the fish entity as it is reeled toward the rod.
- `FishingRod` owns the cast, float, and line states. `FishingFish` owns the fish transform. `Assets/Fishing/pond-fishing.asset` owns range, arc, bite timing, hook window, depth, and reel speed. The village wires one authored fish, a trigger defining the water surface, and the existing Player input actions. The original Kontrakan blockout keeps its old test interaction.
- The household clock keeps running and the active line contributes elapsed minutes to the fishing tally. The catch count is play-session feedback. The fish returns for the next cast. Touch controls, independent fish populations, tension, gear, and catch economy are later work.
- Play Mode probes passed cast rejection outside water, arc and float, early empty retrieve, missed hook, fish movement under the line, one catch, elapsed time accounting, and cleanup. Physical input was not part of the probe.
- References for readable water and line feedback: [Fishing Planet float guide](https://wiki.fishingplanet.com/Float_fishing/en) and [Sea of Thieves fishing guide](https://www.seaofthieves.com/news/fishing-in-sea-of-thieves).

## Fuel, passenger seats, and Ngojol checkpoint — 2026-11-09

- The village fuel pump by the warung discovers the nearest parked vehicle in range by collider and component. The scene configures Rp10,000/L and a 1 L purchase cap. The purchase is limited by tank space and available household cash, including the reservation for any confirmed school transport.
- `VehiclePassenger` owns seat binding, collider suspension, and NavMesh restoration. `EscortFollower` and `NgojolMechanic` supply their own ride intent. The Supra has one passenger seat and the car has two. Both discover nearby passengers with active ride intent through their authored boarding range and layer mask.
- The warung passenger offers one Rp12,000 ride to the signed Pabrik Tekstil drop-off. Accept the fare on foot, board with a nearby stopped vehicle, and disembark at the destination. An early stop keeps the fare open. A completed fare pays once.
- The rider's placement, drop-off reference, destination label, arrival radius, and fare are scene data. The existing warung-to-factory ride is a playable example. A second probe spawned a new vehicle and a new rider with a school destination and a different fare. Both worked without registering either object in the scene. Random offer and route generation can be added when the pickup and drop-off locations are authored.
- The Play Mode interaction probe staged vehicles at the stops and verified a full and partial fuel purchase, protected transport funds, both family members using separate car seats, rider boarding, unpaid early exit, and one-time factory payout. Hands-on driving and boarding animations are outside this check.

## School-run vehicles checkpoint — 2026-11-09

- The village has `motor-supra.prefab` and `village-car.prefab` parked outside the gate. Both use `VehicleDrive` with a separate `VehicleHandling` asset, authored seat/exit/camera anchors, and a replaceable visual child. Scene overrides wire Bapak and Anak. The source is a local arcade controller using Rigidbody contacts and the existing Player input map.
- Interact to mount, steer and accelerate with WASD, and interact again when stopped to dismount. S brakes on the bike. On the car it brakes, then reverses. An actively following Anak boards when close to a stopped vehicle and resumes the escort after dismounting onto the NavMesh. School pickup continues to use Anak's NPC menu and the household rules.
- The scene-authored dashboard shows speed, estimated RPM, and fuel. Fuel is tracked per vehicle for the play session. It decreases with powered distance and cuts engine power at empty. Tuning is provisional and the prototype has no refueling or touch driving controls.
- The bike visual is a local graybox. The [Honda Supra X 125 by boiledfish.vvti](https://sketchfab.com/3d-models/honda-supra-x-125-6d61abb3aa3a4161bda31ef783111015) is a CC Attribution candidate whose download requires a Sketchfab login. The [MIT Arcade Car Physics project](https://github.com/Saarg/Arcade_Car_Physics) was reviewed as a controller reference.
- Play Mode probes used the actual interaction ray for mounting, scripted steering and segment headings for the bike route, and physics steps for the car. They verified boarding and dismounting with Anak at school, steering, speed/RPM/fuel telemetry, rejection of a moving exit, and car reverse. Hands-on handling and Android driving remain open.

## NPC interaction checkpoint — 2026-11-09

- In the village, talk to Ibu for the factory escort or pickup request. Talk to Anak at school for manual pickup or the return escort. Pak Sopir at the warung handles the recurring transport offer. Rumah and Kolam remain location interactions.
- One scene-authored uGUI menu presents NPC choices. `EscortFollower` supplies labels and callbacks through serialized references, and `PickupDay` remains the owner of household outcomes. The original Kontrakan scene still serves as the isolated blockout.
- A Play Mode probe verified the player interaction rays, menu buttons, Ibu's refusal, Anak's pickup and return choice, and Pak Sopir's confirmation. The navigation data was refreshed after removing the action markers.

## Jump and escort checkpoint — 2026-11-09

- The foothill blockout now has an eastward lane to Pabrik Tekstil and a baked `NavMeshSurface`. The bake is stored in `Assets/Housing/Foothill/village-navigation.asset`. Re-bake the Village Navigation surface after changing routes or collider geometry.
- `PlayerController` uses the existing Player/Jump input action in both blockouts. Jump height is an Inspector setting. Space is the desktop binding. A touch jump control is still pending.
- `EscortFollower` is one reusable root component built on `WorldInteractable` and `NavMeshAgent`. It uses authored references for its leader, home, and destination. Interact to recruit Ibu or Anak. Arrival at the destination ends following and makes the return trip available. The village assigns the factory to Ibu and the school to Anak.
- A Play Mode probe verified the jump arc and landing, then ran both NPCs along their baked routes in both directions using accelerated agents. The escort is physical travel in this scene. Pickup coverage and Ibu's working hours still use the existing household rules.

## Foothill map and recurring pickup checkpoint — 2026-11-09

- Open `Assets/Scenes/kampung-foothill-blockout.unity` in Unity to play the map. The original Kontrakan scene remains the existing build entry. The village scene is a 90 × 76 m graybox with the existing compound, one narrow lane, school forecourt, warung, field strips, pond, and factory turnoff. Cibuntu in Pasawahan, Kuningan was selected by a draw from Cibuluh and Cibuntu. The compressed fictional layout takes its cues from the documented Ciremai foothills and narrow approach. [Cibuntu map pin](https://www.google.com/maps/search/?api=1&query=-6.835101%2C108.441344) · [Village listing](https://jadesta.kemenpar.go.id/desa/17899) · [Approach road reference](https://ppid.purbalinggakab.go.id/pegiat-pokdarwis-ikuti-peningkatan-kapasitas-ke-desa-wisata-cibuntu-kuningan/).
- `DaySchedule` now describes the timings repeated by each new day. `PickupDay` tracks the day number, household cash, pickup occurrence, and recurring transport coverage. The cash, fee, and Ibu shift are Inspector values on the village scenario. Home advances to the next morning after the current day ends. Cash and the transport arrangement persist within the play session. Each pickup is confirmed before it is fulfilled and charged.
- The live Play Mode probe walked Bapak's controller through the gate to the school and then to the pond, used the actual landmark interaction rays, and verified Ibu's shift refusal and transport charging across two consecutive pickups. Driving, a fishing minigame, saved progress between sessions, and hands-on input are separate acceptance work.

## Local continuation checkpoint — 2026-11-09

- The current local checkout is `Documents/Deadbeat-Dad-Simulator` on `main`. Its working tree declares Unity `6000.6.8f1`. The original approved `6000.4.4f1` baseline and historical evidence remain below.
- `Assets/Scenes/kontrakan-blockout.unity` contains Bapak's `PlayerController`, three `WorldInteractable` landmarks, the `PickupDay` scenario, and a scene-authored uGUI HUD. `GameClock` owns time and `PickupDay` owns pickup and fishing rules. Edit `Assets/Settings/first-day-schedule.asset` to tune the clock rate, day bounds, pickup window, and activity durations. The scene selects the schedule through a serialized reference. `FirstDaySetup` creates the landmark and HUD wiring from the Editor.
- The last Play Mode probe exercised the three scene interactions, successful and missed pickup, fishing time, and replay. The Kontrakan FBX check reported zero same-facing overlaps. The blockout check passed all 15 doorways with 117 assigned mesh colliders. Physical input, a moving-camera walkthrough, and Android device acceptance were not part of those checks.
- These features and the Editor version reflect the local working tree at this checkpoint. Check Git status and the current Unity target before assuming they have been published.

## Original foundation snapshot — 2026-11-05

- Unity project root and Git repository: `C:\Deadbeat-Dad-Simulator`.
- Baseline inspected: `b3f17ca`; working branch `codex/unity-init` (2026-11-05 verification).
- Foundation: Unity 6000.4.4f1, URP, Input System. See [ADR-0006](../decisions/ADR-0006-unity-prototype-foundation.md).
- Assets: existing single-house and five-unit compound FBXs, source layouts, Node generators under `tools/`. Preserve the [approved house scale](../decisions/ADR-0005-starter-house-layout.md).
- Runtime: `Assets/Scripts/FirstPersonWalker.cs`, CharacterController-based first-person walking, mouse and touch input. No gameplay framework, networking, transport, fishing, or save system.
- Authoring/checks: `Assets/Editor/BlockoutSetup.cs`. Menu `Bapak Simulator` creates the blockout and validates fifteen doorways, floor/wall collision and character scale. `Create` replaces the current scene; save work before invoking it.
- Intended startup scene: `Assets/Scenes/kontrakan-blockout.unity`.
- Docs remain in `docs/`, outside `Assets/` but inside the project. Follow root `AGENTS.md` and existing ADR authority.
- Windows/Android are targets; verify actual build and device status in the [initialization handoff](../journal/2026-11-05-unity-initialization.md).

## Verified continuation checkpoint — 2026-11-05

- Confirmed URP 17.4.0 and Input System 1.19.0 in `Packages/manifest.json`; graphics settings reference a render-pipeline asset and `activeInputHandler: 1` selects Input System. Unity Test Framework 1.6.0 is installed; no first-party test assemblies or `.asmdef`/`.asmref` files were found under `Assets/`. Runtime and editor scripts use Unity's default assemblies.
- `EditorBuildSettings.asset` enables only `Assets/Scenes/kontrakan-blockout.unity`; SampleScene remains template content. No custom scene-loading system was found.
- Code is a small MonoBehaviour controller plus a static editor authoring/validation utility; namespace `BapakSimulator`, private serialized fields, no gameplay service framework or networking implementation. Multiplayer Center is an editor package, not evidence of multiplayer gameplay.
- Fresh `node tools/check-fbx.mjs` passes both FBXs. Fresh Unity batch execution of `BlockoutSetup.Validate` reports fifteen doorway checks, five walls/floors, 1.65m character, movement normalization and camera pitch checks passing, with process return code 0. **Caveat:** manual `SendMessage("Awake")` on the edit-mode movement probe also logs `ShouldRunBehaviour()` assertion at `BlockoutSetup.cs:112`. Do not describe this as an error-free validation run.
- Prior Windows build success is supported by the initialization log and existing executable; no rebuild was run during this continuation. The existing build was launched and its rendered entrance view inspected. Hands-on traversal and Android/device validation remain unverified.
- Official Pipeline 0.10.0-exp.1 is installed; batch log shows it stopping on exit. No live CLI connection was established; `unity` is not on this session's PATH. Package documentation read: `Documentation~/connectivity.md`. No Unity MCP tools are exposed in this chat.
- Unity generated untracked `Assets/Resources.meta` during validation; it was retained, not staged. No intentional scene, model, runtime, package, or project-settings edits were made.
- See [continuation handoff](../journal/2026-11-05-project-verification.md) for PR #2 integration conflicts and the next documentation task.

## Catch-to-cash checkpoint — 2026-11-10

- The village scene (`Assets/Scenes/kampung-foothill-blockout.unity`) wires three `Physical Pond` fish to `Assets/Economy/pond-fish.asset`. `Kolam` grants the landed fish item to the `ItemInventory` on `Household and activities`. `Day HUD` shows the total.
- `Bu Warung` uses `Assets/Economy/warung-fish-offer.asset` for the eligible item and unit price (prototype Rp8,000). Sales remove the quoted quantity from the bag and credit the same `PickupDay` cash balance used for transport, fuel and ngojol. Both balance and bag are session state.
- Unity Play Mode verified physical landing into the bag, the buyer quote, cancellation, single sale, empty-bag handling and bulk sale with a second item and price. The second offer was a temporary runtime fixture.

## Player wallet ownership — 2026-11-10

- `PlayerProgression` owns the live wallet. Set Starting Cash on `Household and activities` in Kampung or `First Day` in Kontrakan. Each scene kept its authored Rp20,000 start.
- `PickupDay` applies the paid-transport reserve and tracks expenses for the recap. Fish and ngojol income credit the progression wallet. The wallet carries across in-game days and resets on a new play session.
- Village Play Mode verified starting cash, spend rejection above the reserve, expense debit, paid pickup settlement, day-two carryover, and catch-to-cash sale paths.

## Local checkpoint saves — 2026-11-10

- `CheckpointSave` loads a versioned JSON file at startup and autosaves when `PickupDay` finishes a day, whether at home or by reaching the end of the clock. After reload, Rumah starts the next morning. Kampung and Kontrakan each have an Inspector-authored slot GUID.
- Checkpoints include the wallet, catch bag, recurring transport, latest and previous recaps, completed commute legs, and relationship values. Relationship content can use `RelationshipDefinition` assets with unique stable IDs and the progression get/set methods. Register any new persistent inventory items on the scene checkpoint component and give each a stable ID.
- Files live in `Application.persistentDataPath/Saves`. The previous checkpoint is kept as a backup. A corrupt primary can fall back to the backup. Unsupported versions and unrecoverable files disable saving for that session and are preserved.
- A temporary Play Mode slot verified first and second day checkpoints, reload, relationship values, inventory, commute status, recap comparison, backup recovery, and preservation of an unsupported version. Midday progress, active activities and transforms are outside the checkpoint boundary. Android filesystem behavior remains unverified.

## Vehicle checkpoint records — 2026-11-10

- Checkpoint v2 adds a record per registered vehicle with its access flag, odometer in meters, fuel in the tank, lifetime liters consumed and purchased, and lifetime rupiah spent on fuel. The end-of-day save boundary applies to these records.
- In Kampung, `Household and activities > Checkpoint Save > Vehicles` references `Motor Supra` and `Mobil`. Each `VehicleDrive` has its own stable ID and Inspector-authored starting access. Both vehicles are initially accessible in this prototype. Future progression actions can call `VehicleDrive.Unlock()`; locked vehicles cannot be driven or refueled.
- Version-one checkpoints keep their existing progression and initialize vehicle records from the scene. A temporary Play Mode slot verified driving, the fuel station and wallet charge, independent vehicle records, locked access after reload, v1 migration and backup recovery.
