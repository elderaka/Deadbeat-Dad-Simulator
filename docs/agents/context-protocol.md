---
title: "Context Survival and Compaction Protocol"
type: process
status: living
updated: 2026-11-02
---

# Context Survival and Compaction Protocol

## On entering a fresh session (or after compaction)
1. Read [docs/index.md](../index.md) and [GDD index](../gdd/index.md).
2. Read [Accepted ADRs](../decisions/index.md), then [Open Questions](../gdd/12-open-questions.md).
3. Read the latest dated [journal entry](../journal/index.md) and most recent handoff.
4. Inspect repository files relevant to the assigned task before editing or trusting memory.
5. Summarize known facts, assumptions and unknowns; ask for creator input only for blocking decisions.

## Before a delegated agent begins
Provide: goal, exact target files, source-of-truth links, must-not-change constraints, test expectation and precise expected output. Assign non-overlapping ownership for parallel writing.

## During work
Write experiments and speculative ideas under journal first. For product changes:
- If a creator approves a choice, create/modify ADR with status **accepted** and update GDD.
- If not approved, mark **proposed** and keep a question in the decision queue.
- If technical reality conflicts with the design, log a discrepancy rather than silently redefining the game.

## On ending work or before context compaction
Append a dated journal file (or dated section if same session) containing:
- Goal and summary, files/commits changed
- Decisions made, rejected alternatives and evidence
- Tests executed and results; known failures
- Unresolved questions and explicit blockers
- Exact next action with owners
- Links to modified canonical pages

Make a short, link-heavy handoff entry readable in <2 minutes. **Do not claim a test or code change was performed without evidence.**

## Naming conventions
- GDD files: numbered lower-case kebab-case, one domain each
- Decisions: ADR-000N-short-name.md, never reuse an ID
- Journal: YYYY-MM-DD-topic.md; add -02 suffix for multiple same-topic sessions
- Journal entries are historical and should generally be append-only. Corrections should be clearly attributed and dated.

## Important nuance
This protocol guards *project decisions* from agent compaction, not private internal reasoning. Prefer actionable facts over long chat transcripts.
