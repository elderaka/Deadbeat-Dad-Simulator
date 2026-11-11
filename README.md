# Deadbeat Dad Simulator (Bapak Simulator)

A comedic Indonesian household-and-leisure simulator about **earning the freedom to do absolutely nothing** while helping the family thrive.

> **North star:** The better Bapak becomes at managing life, the less he is required to do personally. Ibu and Anak have independent routines, interests and choices.

## Unity prototype

Repository root is a Unity project targeting **Unity 6000.6.8f1**, URP and the Input System (see `ProjectSettings/ProjectVersion.txt`). Build Settings currently begin with `Assets/Scenes/prototype-title.unity`, then `Assets/Scenes/kampung-foothill-blockout.unity`.

The bare Kampung prototype contains first-person movement, school and factory escorts, vehicle/passenger transport, authored ojol routes, physical pond fishing and fish sales, phone/Yarn dialogue, pinjol and fictional judol interactions, and end-of-day checkpoint saving. Many balances, visual assets, touch/device controls and consequence links are provisional. For detailed work and prior scripted Play Mode checks see [Neighborhood and Phone Prototype](docs/roadmap/neighborhood-phone.md).

The older `Assets/Scenes/kontrakan-blockout.unity` remains a limited blockout fixture. Editor commands under `Bapak Simulator` may regenerate scenes; save manual edits before invoking destructive setup commands.

## Docs and milestones

Start with the [Design Vault](docs/index.md). The [roadmap](docs/roadmap/index.md) records the bare slice as implemented; **recurring household economics and meaningful delayed consequences are the next milestone**, not yet a completed feature. The [Connected Progression GDD](docs/gdd/16-connected-progression.md) is a proposal to prototype and test, not a finalized skill tree.

Only Bapak is directly controlled. The optional fictional judol/pinjol systems use game money and depict financial risk; they are not intended as real-money gambling or purchasable wagering products.
