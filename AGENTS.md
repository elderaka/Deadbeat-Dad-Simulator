# Agent Operating Guide — Deadbeat Dad Simulator

## Scope and orientation

- Work in the checkout that contains `Assets/`, `Packages/`, `ProjectSettings/`, `docs/`, and `tools/`. Verify the absolute checkout path and the Unity CLI or MCP target before editing or running Editor commands.
- Start with the [design vault](docs/index.md), [GDD index](docs/gdd/index.md), [decisions](docs/decisions/index.md), [open questions](docs/gdd/12-open-questions.md), and the latest relevant [journal handoff](docs/journal/index.md). Use the [roadmap](docs/roadmap/index.md) for milestone scope and the [Unity project context](docs/agents/unity-project-context.md) for the implementation snapshot. Check current source, scene wiring, and Git status before relying on a snapshot.
- Authority runs from current explicit creator direction through accepted ADRs, canonical GDD, proposed GDD, and journal exploration. If implementation and design disagree, record the discrepancy and escalate the product choice when it blocks work.
- Keep design decisions, milestone progress, and validation evidence in their existing documents. Update the relevant design or context page when a contract, startup path, or system owner changes. Record creator-approved design changes in an ADR. Keep playtest timings and other tuning provisional until approved.

## Gameplay ownership

- Build the current vertical slice around the player earning discretionary time through reliable household arrangements. Bapak is the only directly controlled character. Family members act through their own schedules and agreements. A request, confirmed coverage, and completed pickup are distinct states.
- Give each state one owner. Player control handles movement, look, and interaction input. World interactions dispatch actions. The clock owns simulation time. Scenario and household systems decide eligibility, coverage, and effects. Presentation reads that state and sends player intent through the same gameplay entry points as world interactions.
- Keep deterministic rules in plain C# where practical. Use MonoBehaviours for Unity lifecycle, physics, input, and scene wiring. Extend the current clock, interaction, and authoring paths before introducing a competing system. Add abstractions when a real gameplay case needs them.
- Scope new assets, systems, and dependencies to the agreed milestone. Keep multiplayer outside the MVP. Keep the deferred risk-economy systems out of the initial slice and follow the existing restriction on adding gambling mechanics. The satire concerns fictional Bapak behavior and household logistics. Avoid disparaging ethnic groups.

## Reusable code and authored data

- Separate reusable mechanics from scenario composition. Name shared systems for what they do, such as `GameClock` or `WorldInteractable`. Keep day-specific rules in the scenario or its authored definition. Shared code must work across scenes without special branches for a day, landmark, character, or GameObject name.
- Author pickup windows, activity durations and costs, NPC availability, landmark placement, and other tuning outside the rules that consume them. Use serialized fields for instance settings and ScriptableObjects for definitions shared across scenes or variants. Use a validated data format when an existing authoring workflow needs it. Constants in code are for invariants, such as minutes per hour. A named constant for a playtest duration is still hardcoded tuning.
- Wire scene dependencies through serialized references and bind actions through the Input System. Use stable typed identifiers where definitions cross save or content boundaries. Display labels, scene paths, hierarchy names, and input keys must never serve as gameplay identifiers. Put player-facing copy in authored content and shared UI styles in Canvas/prefab assets.
- Keep authored definitions separate from mutable runtime state. Make the system that owns the state validate and apply changes through a small API. Consumers should depend on the capabilities they use. Prefer composition and explicit dependencies, with no scene-wide searches, singletons, or shared mutable assets as shortcuts.
- Validate required references, IDs, and values at the authoring or initialization boundary. Report the specific asset and field when configuration is invalid. Preserve expected gameplay failures, such as a missed pickup, as explicit outcomes. Use defaults only when they are intentional and documented.
- Before extending a shared system, inspect its callers and authoring path. For any change that claims reuse, show that a second configuration works through the same runtime path without editing the shared code. Extend the existing system first, and keep migrations scoped to the feature being changed. Preserve unrelated working-tree changes and use the narrowest relevant verifier.

## Unity and asset workflow

- Use the version declared in `ProjectSettings/ProjectVersion.txt` for this checkout. Preserve the existing URP and Input System setup. Treat an Editor or package migration as a separate change with its own evidence. Windows and Android are target platforms. Input code alone does not establish device support.
- Use the Unity Editor or its CLI/Pipeline for scene, prefab, and serialized-reference edits. Preserve scene links and asset GUIDs. Move an existing asset with its `.meta` file and let Unity generate metadata for new assets. Check for unsaved scenes or active Play Mode before disruptive operations. Restart or force-close only with authorization for this project and after checking the target and unsaved-work state.
- Static HUD structure, layout, sizing, and styling belong in Editor-authored Canvas objects or prefabs. Runtime code can bind state, animate UI, and instantiate authored templates for dynamic entries. Use uGUI for the current HUD and follow its existing scene wiring.
- Keep runtime code under `Assets/Scripts/` and Editor-only authoring tools under `Assets/Editor/`. Prefer the Unity and .NET features already available. If a shared diagnostic mode becomes necessary, use one state owner and one input binding. Check current bindings before reserving a key.
- Change the source generator for procedural building geometry, then regenerate the FBX. Keep approved dimensions and door openings. Run `node tools/check-fbx.mjs` and verify Unity import, collider wiring, and affected passages. Check suspected z-fighting from a moving camera when visual verification is available.

## Coordination and validation

- Assign one writer per shared file and one operator per Unity Editor when coordinating authorized work. Agree on scope, ownership, and the acceptance check before concurrent edits. Use the [context protocol](docs/agents/context-protocol.md) and [handoff template](docs/agents/task-template.md). A question or an idle session does not transfer ownership or grant permission.
- Implement a cohesive slice, then run a consolidated validation pass. Do safety checks before scene or asset operations. For gameplay, use the normal startup and interaction path in the active build configuration. A direct method call can check rules, while an in-scene run is needed to verify wiring and player-facing behavior.
- Report design approval, source implementation, scene wiring, compile/import checks, and runtime or device acceptance separately. Record what changed, evidence and known gaps, and the next check. Append a dated journal handoff for multi-agent work per the context protocol. Documentation-only work needs link and diff checks when relevant.
- Keep credentials and personal data out of source, authored assets, logs, and shared handoffs. Use synthetic fixtures where an example would otherwise expose private data.

Use lowercase kebab-case for documentation filenames, YAML frontmatter for vault pages, relative Markdown links, and YYYY-MM-DD dates in Asia/Jakarta project time.

Workflow reference: [AlterOS development guidelines](https://github.com/elderaka/alteros/blob/main/AGENTS.md).
