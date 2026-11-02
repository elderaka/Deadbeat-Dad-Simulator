# Agent Operating Guide — Deadbeat Dad Simulator

This repository is design-first. **Start here every session**:
1. Read [docs/index.md](docs/index.md) and [docs/gdd/index.md](docs/gdd/index.md).
2. Read [docs/decisions/index.md](docs/decisions/index.md), especially accepted ADRs.
3. Read [docs/gdd/12-open-questions.md](docs/gdd/12-open-questions.md) and the latest [journal handoff](docs/journal/index.md).
4. Read only relevant GDD sections for your task; avoid inventing consensus from a chat summary.

**Authority order:** explicit current user direction > accepted ADRs > canonical GDD > proposed GDD > journal speculation. When sources conflict, record it as an open question and ask for direction rather than silently choosing.

**Do not:**
- Upgrade a proposal into canon without a user decision and an ADR.
- Treat illustrative numbers, screenshots, example character names, or example events as final production requirements.
- Implement multiplayer in MVP; it is a future roadmap idea.
- Add gambling mechanics or disparage ethnic groups. The satire is about fictional Bapak behavior and domestic logistics.

**Every multi-agent task must conclude with:**
- Files/sections changed; decisions made (links to ADR); outstanding questions; tests/evidence; next steps; any blocked assumptions.
- Append a dated journal/handoff entry (never rewrite old sessions to conceal changes). Use [context protocol](docs/agents/context-protocol.md).
- Keep changes scoped. Agent journals are scratchpads, not design authority.

**Repository conventions:** use lowercase kebab-case filenames, descriptive titles, YAML frontmatter and relative Markdown links (GitHub-safe). Optional Obsidian wikilinks may be added, but must not be the sole navigation path. Dates use YYYY-MM-DD (Asia/Jakarta project time).
