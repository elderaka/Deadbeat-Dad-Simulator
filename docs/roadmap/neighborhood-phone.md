---
title: "Neighborhood and Phone Prototype"
type: experiment
status: living
updated: 2026-11-10
---

# Neighborhood and Phone Prototype

The creator accepted the two-day prototype on 2026-11-10 and found the clock too fast. This page records behavior in the local Kampung prototype. Clock speed, activity costs, and fares remain provisional.

## First playable scenario

1. Bapak can make the school pickup himself or use the phone to call Ibu. She checks her own shift and can decline with a visible reason.
2. Bapak can call Pak Sopir and confirm a paid pickup. Confirmation reserves the fee. Completion resolves the obligation and charges the wallet once. An agreed recurring arrangement can cover later eligible days without a repeat call.
3. The phone shows who accepted, when pickup is due, and whether it happened. Use the same rules for phone and in-person requests. A confirmed plan still has to be carried out.
4. Use the freed time for fishing or a ride. For the first phone-based ngojol offer, use one authored resident with a valid pickup and destination. Future dispatch can vary trips among known residents according to their schedules.

## Starter roster

| Resident | Prototype role |
|---|---|
| Ibu | Existing family contact whose factory shift can conflict with pickup |
| Anak | Existing student with school and home as routine destinations |
| Pak Sopir | Existing school transport provider and first recurring paid arrangement |
| Bu Warung | Existing fish buyer and a resident anchored at the warung |
| Pak Yanto | Authored Ojol passenger, warung regular, and potential buyer of the household TV |

The five-door courtyard is the social anchor. Units 2–5 and their occupants remain to be assigned. The reference boards offer different names and roles, so the door labels, character names, dimensions, map scale, and storyboard scenes are provisional. The map gives us useful anchors for home, school, factory, warung, pond, and routes between them.

## Framework boundary and acceptance

Author resident identity, contacts, routine, and eligible ride locations as content with stable IDs. World interactions and the phone resolve the same resident and agreement. Availability, requested, confirmed, completed, and failed states are tracked explicitly. Save standing agreements through the existing checkpoint. Repeat the phone pickup and ride path with a second authored resident to verify that the shared code needs no change.

A good test day has time to complete a chore and choose one optional activity. Once an arrangement is established, the player spends attention on exceptions and trade-offs. The phone and routine foundations come before broad dialogue, contextual events, and the weekly report.

## NPC foundation checkpoint

Kampung now has a scene-authored roster and four resident profiles. Ibu's factory shift drives the existing pickup availability check. Anak's Ngobrol interaction demonstrates ordered dialogue conditions, a persistent flag, and an affinity threshold. The current routine blocks answer availability queries. The escort and passenger systems still own movement. Checkpoint v3 includes resident-scoped dialogue flags and migrates v1/v2 saves.

Anak and Ibu can also initiate a pickup request by calling Bapak. Model an incoming call as a presentation of a pending request from a known resident. The request should carry its trip and time window. Accepting, declining, and completing the pickup remain separate outcomes. Use the same request from the phone and face-to-face interaction so replying through one channel resolves the other.

## Yarn Spinner pilot

The Kampung scene uses the pinned Yarn Spinner Unity package for Anak's pickup dialogue. Edit the conversation in `Assets/Dialogue/anak-pickup.yarn`. The `Neighborhood Dialogue` scene object owns the Yarn runner and presenters. `Household and activities > School Pickup Dialogue` contains the authored 11:30 call time, prompt, P binding, and resident references.

The incoming-call button and Anak's Ngobrol interaction at school start the same Yarn node. Yarn receives the current channel, affinity, and promise state. The confirmation command asks PickupDay to record a promise. The escort arrival still completes the obligation, and the recap is persisted at the end-of-day checkpoint. The existing ambient conversation remains available before Anak reaches school.

A temporary Play Mode slot verified declining the phone call, accepting at school, the affinity variant, completing and reloading the pickup recap, then accepting a later call and seeing the promise acknowledged at the gate.

During a Yarn conversation, Bapak's movement, camera, jump, and world interaction remain locked until the runner finishes. In Kampung, Escape opens the pause overlay and freezes the simulation. Lanjutkan restores the current line and leaves world controls locked until the conversation finishes. A Play Mode probe exercised the Escape binding, clock freeze, menu focus, Continue button, and restoration of player control.

## Shared parking markers

Kampung has visible bays for Rumah, Sekolah, Pabrik, Ojol pickup, and Bensin under the Parking markers scene object. A ParkingMarker uses its trigger footprint and an authored speed threshold. Its Occupied Changed event emits true when the first vehicle parks and false when the last clears the bay. Wire scene reactions to this event in the Inspector.

Passenger drop-off is matched against the bay's authored destination references. A stopped vehicle unloads matching passengers, and each walks to its existing commute or fare destination on the NavMesh. Completion and payout happen on arrival. The current Ojol passenger first boards at the Ojol bay after accepting the job. The fuel pump serves a stopped, unmounted vehicle in its assigned bay. A scripted village Play Mode probe verified the occupancy transitions, pump gate, distinct school and factory drop-offs, Ojol boarding and the single fare payout.

## Kampung dialogue migration

All current Kampung NPC interactions use the shared Neighborhood Dialogue runner. Anak's phone, home and school choices live in `Assets/Dialogue/anak-pickup.yarn`. Ibu, Pak Sopir, the Ojol passenger, and Bu Warung live in `Assets/Dialogue/kampung-residents.yarn`. Schedules, affinity and flags remain in the resident and progression systems. The runner passes current gameplay values to Yarn. Yarn choices submit actions to the existing escort, transport, ride, and sale rules. The old two-button menu was retired.

Play Mode exercised all five conversations, including Ibu's shift refusal, both escorts, Anak's phone-to-gate promise, accepting an Ojol ride, individual and bulk sales, the empty bag response, and scheduling paid transport.

## Neighborhood map blockout

Kampung now has a second route from the Kontrakan approach to the factory road. The east loop passes a toko kelontong, warnet, musholla, bengkel, lapangan, and three provisional neighbor houses. The existing school, pond, warung, factory, and service bays remain the active gameplay anchors. The new places are exterior blocks. Their entrances are grouped under Neighborhood blockout > Destination entrances for the next scheduling pass.

The existing Village Navigation surface was rebaked into the same navigation asset. Validation found complete NavMesh paths from the gate to all eight new entrances, the existing family and Ojol destinations, and all five parking bays. Ground probes checked the new loop and its junctions with the existing roads.

Next, author recurring routine entries with destination references, then give an idle NPC ownership of their next scheduled move. Prove the handoff with Ibu and Anak. While escorted or in a vehicle, their existing travel state owns movement. On drop-off, finish the walk to the scheduled destination before resuming the routine.

## Autonomous family routines

In Kampung, Anak leaves for school at 08:20 and Ibu leaves for the textile factory at 08:25 if Bapak has not taken over their trips. Ibu walks home at 14:00. Anak waits for pickup after school. When a paid pickup or missed-pickup outcome resolves the obligation, he returns home and the commute tracker records his arrival. An autonomous return does not award manual pickup credit.

Edit the recurring windows on the Anak and Ibu profiles in Assets/NPC. Routine Location assets identify destinations. Each NPC's Routine Walker binds those assets to scene transforms and exposes an arrival event for the commute tracker. The walker yields to face-to-face conversation, escorts, vehicle rides and parking drop-off walks. Schedules resolve again on the next in-game day.

A scripted Play Mode run used accelerated NavMesh agents and clock advances to verify both outbound routes, both returns, the pickup hold, commute tracking, a repeat day, and player escort takeover.

## Phone prototype

P opens the Kampung phone on foot. Kontak has scene-authored calls for Anak, Ibu, and Pak Sopir. Catatan reads today's pickup objective, the four commute legs, and Ibu's current request outcome. Calls use the same Yarn runner and household actions as in-person conversations. The old standalone school answer prompt is now a shared incoming-call banner.

Ibu's Factory Pickup Request reads shift end from the routine profile. Her prototype wait is 30 in-game minutes, with a shorter incoming banner. She waits for a response or a promised pickup, then resumes her autonomous route if Bapak declines or the wait expires. If he has already started escorting her, that trip retains movement ownership. Arrival through the escort completes the request.

Play Mode verified P, Anak's call through Kontak, the checklist layout, scheduling Pak Sopir, Ibu's incoming call and waiting, a promise displayed in Catatan, and Ibu walking home after timeout.

## Phone Ojol routes

The Ojol tab opens the current Yarn offer. Pak Yanto is an authored resident who rotates through five configured prototype routes. Routes and fares are on his Ngojol Mechanic component. Each route has a pickup bay, a waiting point, a drop-off bay, and a walking destination. The ten parking triggers and their indicator children are grouped under Ojol stops.

Only the current pickup indicator appears after accepting. Boarding hides it and shows the destination indicator. The fare pays once the rider reaches the walking destination, then the indicator hides. Idle indicators are inactive while the triggers stay active. The next offer reuses the same flow with the next authored route.

A Play Mode probe completed all five routes through the phone and verified exact fares, a wrong-bay stop, indicator transitions, and live idle triggers.

## Finance and full-height phone

In Kampung, right-click while the phone is open to switch between the corner layout and a centered, nearly full-height layout. The current app stays open. Keuangan shows the prototype electricity bill, a due day, and an explicit Pinjol quote before confirmation. The household component authors Rp18,000 due on day 2, with a provisional Rp12,000 advance and Rp16,000 repayment due the following day. Catatan reflects pending amounts and overdue status. The loan is limited to one outstanding balance and bills and repayments respect cash reserved for school transport.

Checkpoint v4 records bill payment, outstanding debt, and its due day. Existing checkpoints start with no recorded payment or debt. The Play Mode probe verified both phone sizes, quote cancellation, borrowing, bill payment, save and reload, reserved funds, and repayment.

Pinjol offers now live in Assets/Economy/Loans and are selected from Household Finances > Loan Offers. Keuangan cycles the list with Ganti. Each offer has a stable ID, lender name, principal, repayment and term. Checkpoint v5 keeps the lender ID and the accepted repayment and due day separately, so editing the offer does not rewrite an existing debt. Version-four debts retain their original balance and due date.

Two scripted day-one to day-two checks passed. The no-loan path earned an Ojol fare, paid for fuel, reloaded and paid the electricity bill. The borrowing path compared the two authored offers, cancelled a quote, borrowed, saved and reloaded the exact loan, then repaid it while respecting the school transport reserve.

## Prototype title and starting circumstances

The build opens Assets/Scenes/prototype-title.unity. Its discrete difficulty slider reads editable assets from Assets/Settings/Difficulty. The current prototype presets set starting cash, inherited debt, and the due day. The Bapak belum pernah judol toggle removes the inherited history flag and the inherited loan. Income, NPC behavior, and vehicle condition use their existing gameplay rules across the presets.

Mulai baru opens Kampung in a fresh checkpoint slot. Lanjutkan opens the most recent title-run checkpoint, with the authored Kampung slot as a fallback for earlier playtests. Starting a run leaves previous checkpoint files in place. The Continue pointer changes when the new run first saves at day end. Checkpoint v6 carries the preset ID and inherited-history flag with the existing cash and debt fields.

Play Mode verified the build entry, all slider previews, new runs with and without inherited history, the first end-of-day save, and Continue loading the correct run.

## Overdue event and Pak Yanto

The Finance Overdue Trigger can watch the bill, the loan, or both. It fires an Inspector event once per overdue in-game day. Catatan displays its reminder while an obligation remains overdue. The first instance lives on Household and activities.

An in-person dialogue choice lets Bapak disclose the overdue finances to Pak Yanto. Once he knows, he can offer Rp5,500 for the household TV, whose provisional reference value is Rp14,000. Declining records the response and keeps the TV. Selling credits the wallet once and removes the Unit 1 TV visual, including after reload. Configure the item, values, and memory flags on Pak Yanto > Neighbor Buyout. Pak Yanto returns to his authored warung hangout after completing an Ojol ride.

Play Mode verified the overdue gate and Catatan reminder, disclosure, saved refusal, alternative sale, and the absence of the TV after reloading a sold state.

## Warung domino prototype

Interact with Meja domino - Warung during a running day. The table uses Pak Yanto, Bu Warung, and Pak Sopir as named seats in the panel. Each seat gets five tiles. Match either end or pass when blocked. The round ends on an empty hand or when every seat passes. The prototype has a casual option and a scene-authored Rp1,000 stake, uses only the household game wallet, and advances the day by five minutes per player turn. Leaving an active round forfeits any stake. Play Mode verified interaction, legal moves, time costs, cash settlement, and closing the table.

## Judol and Warnet

The phone **Judol** tab and **Terminal Warnet** share the arcade and ledger. **Slot** animates three reels to reveal the weighted result. **Crash** shows the live multiplier and lets the player cash out before it falls. **Bola** and **Togel** display the current pick and settle tickets on a later configured day. **Saham** displays the current quote for a fictional stock.

The stock reprices once per in-game morning. Its next price is the previous price plus a fraction of the gap back to its anchor, plus a random percentage move, with an occasional downward news shock. Buy pays the current quote and sell receives the quote less the spread. The risk ledger measures cash in and out. Open shares retain a separate quantity and current sell quote.

Tune the five profiles in Assets/Economy/Risk and register them on Household and activities > Risk Arcade. Checkpoint v7 saves pending tickets, stock quotes and holdings, and the per-game cash ledger. Play Mode verified both entry points, a slot result, crash cashout and forfeit, ticket settlement after reload, and buying and selling stock.

The **Bapak Pengangguran Simulator** title screen opens on a plain New Game, Continue, and Quit menu over a slow camera sweep of Kampung. The backdrop uses static village meshes. New Game opens the compact difficulty setup, and Back returns to the main menu. Play Mode verified the sweep, both menus, difficulty launch, and Continue.

## Bedroom start and village sky

Kampung starts Bapak on the bed in Unit 1's second room. A new game starts the clock at 06:00 after the prologue. Continue loads the last checkpoint and starts the next morning. The old Rumah interaction was replaced by the bed, which ends the day and exposes the next-morning action. Tap or click during a Yarn line to finish revealing it, then tap or click again to advance. Space and Enter use the same behavior. Jeda opens the pause menu, and dialogue choices have their own buttons. School and work schedules keep their existing times.

Kampung has its own day schedule asset and runs until 22:00. The sky rig reads the household clock and drives the sun, moon, sky, ambient light, and haze. Daylight warms near dusk, and cool moonlight takes over after sunset. Play Mode verified the bed spawn, automatic startup, three dialogue inputs, dusk and moonrise, the bed checkpoint, and day-two Continue.

An incoming family call now slides the phone open on foot, or overlays the current app if it is already open. Angkat hands the conversation to Yarn. Tolak dismisses the ring while leaving the pickup decision and callback available. Calls show a banner while another activity owns the controls. The Yarn canvas only raycasts during conversation, so its hidden backgrounds no longer intercept the phone. Play Mode used pointer input on the compact and full phone and verified both family callers.

## Driving and refueling

The prototype has a Motor Supra and a car. Family and Ojol passengers can board vehicles with available seats. Stop in the matching bay to let passengers out for the final walk. To refuel, park in the Bensin bay, dismount, and use the pump. While driving, the dashboard shows speed, RPM, fuel, passengers, and the camera binding when chase view is available. Handling and fuel settings live in the vehicle assets and scene components.

## Pond fishing and selling

Equip the rod at the pond, aim at the water, and cast. The bobber flies under gravity, floats after landing, and moves back through the water as the line shortens. Reel on a bite, watch the tension, and ease off when the fish runs. The fish and bobber move in the scene and respond to the pond water. Land a fish to add it to the bag, then sell one or all to Bu Warung through dialogue. Fishing tuning lives in Assets/Fishing. Persistent pond populations and an ecosystem remain future work.

## Dialogue and HUD

Six Yarn nodes cover Anak, Ibu, Pak Sopir, the Ojol passenger, Pak Yanto's buyout, and Bu Warung. The dialogue uses everyday Indonesian and neighborhood sitcom beats. The existing conditions and gameplay actions still govern choices. All six nodes imported in Unity, and Ibu delivered her revised line in Play Mode. A branch-by-branch tone and timing pass remains open.

The bottom-left card shows the clock, day, wallet, bag, and one objective or brief activity update. The end-of-day recap uses that same space. Driving adds a readout above the clock. The top-right panel appears only while fishing and displays line tension during the fight. Family travel details are in Catatan. The world interaction prompt stays near the crosshair. The placeholder panels use plain backgrounds.

## Save and resume

Use the bed to end the day and write a checkpoint. Continue loads the last checkpoint and starts the next morning. Checkpoint v7 records day recaps, cash, relationships, inventory, resident flags, completed trips, recurring transport, bills, loans, difficulty, inherited history, and Judol state. Per-vehicle entries record unlock state, tank contents, odometer, fuel used, fuel purchased, and spending on fuel. Mid-day progress returns to the last saved day boundary.

The Play Mode checks on this page were run against the local Unity prototype. Android device testing, longer-run balance, and the weekly scenario remain separate milestones.


## Catatan responsibility board

Catatan shows school pickup and the electricity bill using the same row format. Each row shows its deadline or window, who is responsible, the cost, and the current state. Pickup reads the household agreement and outcome, including manual promises and paid coverage. The bill reads its amount, due day, and payment state from Household Finances. Confirm pickup through the existing conversations and pay the bill in Keuangan. The family travel checklist and reminders remain below the two rows.

Unity Play Mode checks exercised the unpaid and paid bill, an overdue bill, a manual pickup promise, and paid pickup completing at the deadline. A staged missed pickup verified its display. Scheduled bill payment, groceries, and the weekly recap are future wireframe steps.

## New Game prologue

New Game opens on the bed, cuts to Ibu and Anak in the courtyard, and returns to Bapak as Day 1 begins at 06:00. Ibu has a placeholder gesture and both conversations use the shared Yarn runner. Continue resumes from the checkpoint as before.

Edit the ordered steps on **New game prologue** in the Kampung scene. Each step can select a camera pose, cut or ease to it, play an Animation clip, hold for a duration, and wait for a Yarn node to finish. The sequence restores the gameplay camera, HUD, and input on completion. Another scene event can call `SceneSequence.Play` from a UnityEvent. The startup caller controls when the day starts.

Play Mode verified the New Game entry, camera and gesture playback, pause, both Yarn conversations, and the handoff to the bed spawn with gameplay restored.


## Windows touch controls

In Kampung, open Jeda and enable Kontrol sentuh. Drag from anywhere on the left half to move and drag on the right half to look. Lompat, Interaksi, Ponsel, and Jeda are buttons. The walking controls yield to dialogue and the phone, and Tutup ponsel returns to the world. Toggle the option in Jeda to return to desktop controls.

Unity Play Mode checked independent simulated pointers, action buttons, Ibu dialogue, and the Catatan round trip. Simultaneous movement and look through a phone remote client depend on the pointer input Windows receives. Driving and fishing use their existing controls.
