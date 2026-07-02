---
name: card-task-orchestrator
description: Coordinate multi-card implementation campaigns for Slay the Spire 2 mods that use TestTheSpireTemplate. Use when Codex should clarify campaign scope, create or maintain impl_task_list.md, split a card spec list into per-card branches or worktrees, maintain a tracker, assign focused card implementation/review work, merge approved branches, and keep TestTheSpire validation honest. Pairs with sts2-card-creator for individual card implementation and review.
---

# Card Task Orchestrator

This skill coordinates a campaign. It does not author card effects directly. Delegate card implementation and read-only review to focused workers that use `sts2-card-creator`; keep tracker state, branch state, and validation results truthful.

Keep the loop outcome-first: clarify only the decisions that can change the campaign, write the tracker, assign the next narrow unit, validate, record what changed, and continue.

## Required Inputs

Identify these before assigning work:

- `repo_root`: STS2 mod repository, normally a TestTheSpireTemplate-derived repo.
- `design_source`: CSV, YAML, markdown table, issue body, or another card spec list.
- `integration_branch`: branch that receives reviewed work.
- `tracker_path`: campaign tracker, usually `impl_task_list.md` under `repo_root`.
- `worktree_root`: directory for per-card worktrees.
- `sts_runtime_path`: Slay the Spire 2 install used for TestTheSpire runs.
- `test_command_template`: exact focused TestTheSpire command shape for this repo.
- `out_of_scope`: source sections or rows to skip.
- `scope`: card IDs/rows to process, or `all non-blocked todo`.
- `batch_size`: maximum concurrent workers/reviews. Respect active tool limits.

Default validation command shape:

```bash
dotnet msbuild CharMod.Tests/CharMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=<sts_runtime_path> \
  -p:Sts2TestArgs=--sts2-test-filter=<filter>
```

If the project renamed `CharMod.Tests`, discover the actual test project with `rg --files -g '*.Tests.csproj'`.

## Standard Campaign Mode

Use this mode for large card batches:

1. Intake and clarify campaign-level inputs.
2. Parse `design_source`, inspect existing repo patterns, and identify shared mechanisms before assigning cards.
3. Create or refresh `impl_task_list.md`; make it the campaign memory.
4. Launch clarification/design agents for ambiguous rows and shared mechanisms; record their findings before implementation.
5. Reserve conflict-prone shared file entries before parallel card work.
6. Put shared mechanism design tasks before dependent cards. Design branches require read-only review before runtime implementation.
7. Assign implementation workers only after the relevant row or mechanism is specific enough to test.
8. Require each implementation worker to self-test focused behavior before reporting ready.
9. Launch a separate read-only review agent for each implementation or mechanism branch.
10. Merge one approved branch at a time.
11. Update `impl_task_list.md` after each assignment, blocker, review, merge, and integration validation result.

## Clarification Gate

Do local investigation before asking. Inspect the source file, current code, assets, localization, tests, and branch state, then ask only for decisions that would otherwise change implementation or branch layout.

Clarify these at campaign start:

- Source scope: which source sections, card rows, tokens, generated cards, relics, or non-card entries are included.
- Branching: integration branch, base commit, naming convention, and whether existing implementations should be verified or rewritten.
- Runtime: STS2 install path, decompiled/source path, RitsuLib package docs, MinionLib source availability, and test command.
- Tracker ownership: `impl_task_list.md` is edited only by the orchestrator.
- Validation policy: focused tests first; exact numeric assertions only when values are text contracts, thresholds, or regression guards.
- Review policy: every shared mechanism and card branch gets read-only review before merge.
- Concurrency: maximum active workers/reviews and where worktrees may be created.

Clarify these during triage:

- Missing IDs, missing assets, missing localization keys, or unclear class names.
- Effects that depend on shared mechanics, generated cards, token inheritance, persistent state, random choice, selection UI, pile mutation, or multiplayer-sensitive synchronization.
- Any design row where current repo patterns contradict the source text.

Ask in the user's language. Prefer a compact decision list with a proposed default when the local evidence supports one. Record each answer or accepted assumption in `impl_task_list.md` under `Clarifications and Assumptions`.

## Agent Role Contract

Each launched agent needs one role. Do not ask one agent to both implement and approve the same branch.

- Coordinator: owns `impl_task_list.md`, branch/worktree naming, assignments, merge order, integration validation, and final reporting. The coordinator may inspect any branch but does not do routine card implementation.
- Clarification/design agent: runs during intake or shared-mechanism planning. It reads `design_source`, current code, local sources, assets, localization, and tests; it reports ambiguities, proposed defaults, shared API shape, dependent rows, placeholder keys, and test/review criteria. It does not edit production code unless explicitly assigned a design-note branch.
- Placeholder agent: edits only conflict-prone structured files such as localization JSON/YAML, keyword sources, resource manifests, generated-card registries, or asset maps. It creates parseable placeholder entries, validates the file format, and reports covered keys.
- Development/test agent: owns one implementation branch or mechanism branch. It uses `sts2-card-creator`, writes code/localization/tests for the assigned unit, runs focused self-tests, commits the branch, and reports commit, test result, and blockers. It never edits the tracker.
- Review agent: reviews a branch after development/test. It compares against `integration_branch...HEAD`, stays read-only, may run verification commands, and reports `PASS` or `FAIL` with blocking findings first. It never fixes files or edits the tracker unless reassigned under a different role.
- Fix agent: handles a failed review with a narrow prompt containing the blocking findings. It self-tests and reports a new commit; review returns to a separate read-only agent.

## Preflight

Run these before creating worktrees:

1. Confirm `repo_root` is a git repo and has no merge/rebase/cherry-pick in progress.
2. Inspect `git status --short --branch`; record dirty/untracked files and avoid unrelated changes.
3. Confirm `integration_branch` exists locally or create it from the intended base.
4. Confirm `design_source` exists and can be parsed.
5. Confirm `.agents/skills/sts2-card-creator/SKILL.md` resolves; if this repo uses bundled skills, run `git submodule update --init --recursive`.
6. Confirm local reference sources exist or know how to generate them:
   - `local/sts2-decompiled/sts2.csproj`
   - RitsuLib package docs
   - `local/MinionLib/`
7. Confirm `sts_runtime_path` exists and contains an STS2 data directory.
8. Confirm the test mods directory is not polluted by another active TestTheSpire test mod. If it is, record the limitation or isolate before validation.
9. Inspect representative cards, components, localization JSON, and current tests so worker prompts name real local patterns.
10. Create or refresh `tracker_path` before launching workers.

Ask the user only when a missing input could send work to the wrong repo, branch, card set, or runtime.

## Tracker Contract

Only the orchestrator edits `tracker_path`. Workers and reviewers report back; they do not mutate tracker state.

Default tracker path is `impl_task_list.md`. If the file is missing, create it before assignment. If it already exists, preserve historical decisions and append new results instead of replacing useful context.

Use this template:

```markdown
# <Campaign Name> Card Implementation Task List

Source list: `<design_source>` (<format/encoding if known>).

Coordination metadata:
- `repo_root`: `<absolute path>`
- `integration_branch`: `<branch>`
- `tracker_path`: `<repo_root>/impl_task_list.md`
- `worktree_root`: `<absolute path>`
- `sts_source_path`: `<STS_SOURCE_PATH or local/sts2-decompiled>`
- `sts_runtime_path`: `<SlayTheSpire2 path>`
- `test_project`: `<*.Tests.csproj>`
- `test_command_template`: `<focused TestTheSpire command with <filter>>`
- `source_of_truth`: `<design_source> plus current repo patterns`
- `scope`: `<included rows/sections>`
- `out_of_scope`: `<skipped rows/sections>`

Baseline:
- Current branch/status: `<branch and dirty-state summary>`.
- Existing implementation: `<what already exists and still needs verification>`.
- Local source availability: `<sts2-decompiled/RitsuLib/MinionLib status>`.
- Validation baseline: `<last relevant command/result or not yet run>`.
- Test guidance: focused mechanism/state-transition tests first.
- Review guidance: read-only review before merge.
- Agent role policy: clarification/design agents first when needed; development/test and review use separate agents; no self-review.
- Tracker ownership: only orchestrator edits this file.

Clarifications and Assumptions:
- Confirmed: `<user-confirmed decisions>`.
- Assumed: `<local-evidence assumptions to revisit if contradicted>`.
- Blockers needing user decision: `<open campaign-level questions>`.

Current count: `todo` 0 / `in_progress` 0 / `ready` 0 / `done` 0 / `blocked` 0.

Public mechanism status:
- `<shared mechanism>`: `<status, branch, test/review summary, dependent rows>`.

Conflict-prone placeholder tasks:

| Task | Status | Branch | Worktree | Files | Test | Review | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `Localization/key placeholders` | `todo` |  |  | `<json/yaml/resource files>` | `<parse/build check>` | `pending` | `<cards/keys covered>` |

Shared mechanism tasks:

| Task | Status | Branch | Worktree | Agent | Test | Review | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `<mechanism>` | `todo` |  |  |  |  | `pending` | `<dependencies and design notes>` |

Card tasks:

| No | Source row | Card | Type | Rarity | Cost | Source ID | Entry ID | Status | Branch | Worktree | Agent | Test | Review | Blocked reason |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `001` | `<row>` | `<name>` | `<type>` | `<rarity>` | `<cost>` | `<id or blank>` | `<class/entry id>` | `todo` |  |  |  |  | `pending` |  |
```

Track at least:

- row/card id
- card name
- status: `todo`, `in_progress`, `ready`, `done`, or `blocked`
- branch and worktree
- assigned worker/reviewer
- focused test command and result
- review status: `pending`, `pass`, `fail`, or `n/a`
- blocked reason
- clarification or assumption behind any non-obvious decision

Status rules:

- `todo`: not accepted yet.
- `in_progress`: worker assigned.
- `ready`: worker committed and focused validation passed, pending review/merge.
- `blocked`: missing spec, ambiguous behavior, missing asset, missing shared mechanic, or validation environment unavailable.
- `done`: review passed, branch merged into `integration_branch`, integration validation passed, tracker updated.

## Triage

Group work before assigning:

- Already implemented: verify against source and tests before marking done.
- Missing ID/spec/assets: block with a concrete reason.
- Token/generated cards: treat as independent units when they have their own model or localization.
- Conflict-prone shared files: create and merge a placeholder task before parallel workers edit them.
- Shared mechanics: create a mechanism branch first; do not let a single-card worker add broad infrastructure.
- Shared mechanism design: write the intended API/behavior in a design note or tracker section, run read-only review, then implement the reviewed design.
- Low-risk first: damage, block, draw, discard, exhaust, simple powers, straightforward components.
- High-risk later: selection UI, random generation, pile mutation, cross-turn hooks, multiplayer-sensitive actions, and reusable components.

## Placeholder Reservation

Use a placeholder task when many card branches will touch the same structured files. Typical files are localization JSON/YAML, keyword or hover-tip sources, resource manifests, generated-card registries, and shared asset maps.

The placeholder branch should:

1. Add stable keys for all in-scope cards, tokens, components, powers, keywords, selection prompts, and resource entries that can be derived from `design_source`.
2. Use minimal placeholder values that parse and make missing work obvious, such as `TODO: <card name>` or an empty description following the repo's existing schema.
3. Preserve existing ordering and formatting conventions so later single-card diffs stay local.
4. Run the lightest reliable validation: JSON/YAML parse, build, or focused localization/resource test.
5. Get read-only review and merge before launching workers that fill those entries.
6. Record covered files and keys in `impl_task_list.md`.

After placeholders are merged, worker prompts should tell implementers to update only their assigned placeholder entries and to avoid broad reordering of shared JSON/YAML files.

## Development/Test Assignment

For each card branch:

1. Update tracker row to `in_progress`.
2. Create a branch/worktree from latest `integration_branch`, with a stable name such as `<integration>-card-<row>-<slug>`.
3. Give a bounded worker prompt:
   - use `sts2-card-creator`
   - act as the development/test agent for this branch
   - read the design row and nearest local samples
   - inspect `local/sts2-decompiled/`, RitsuLib package docs, and `local/MinionLib/` before coding
   - implement only the assigned card or mechanism
   - update only the assigned localization/resource placeholders under the mod payload
   - add focused TestTheSpire tests when behavior changes
   - prefer mechanism/state-transition assertions; use exact values only when they are the card text contract or regression guard
   - run the focused test or report why it cannot run
   - do not edit the tracker
4. Tell workers not to revert unrelated changes.
5. If a worker stalls, inspect read-only, preserve its files, then reassign a narrower task.

Do not assign a card that depends on an unimplemented shared mechanism unless the task is explicitly a design-only investigation or blocker confirmation.

Normal single-card write scope:

- card model under the project card folder
- card-specific component/power/helper only when directly required
- localization JSON for that card/component/keyword
- focused tests under the test project
- direct token dependency files when justified by generator behavior

Do not include generated build output, unrelated formatting, broad pool rewrites, unrelated balance changes, or shared mechanics in a single-card branch.

## Review Gate

Every card or shared mechanism branch gets read-only review before merge.

Reviewer prompt:

- use `sts2-card-creator`
- act as a read-only review agent for this branch
- compare branch diff against `integration_branch...HEAD`
- check STS2 API usage, target null handling, command determinism, synchronization-sensitive choices, upgrades, components, localization, and tests
- verify scope is limited to the assigned card/mechanism
- report `PASS` or `FAIL`, with blocking findings first
- do not edit files unless reassigned as a fixer

If review fails, send the branch back for a focused fix and rereview.

## Merge Gate

Merge one branch at a time only when:

- worker committed implementation or reported a blocker
- focused validation passed or the branch is explicitly blocked/metadata-only
- read-only review passed
- tracker was not edited by the worker
- no unrelated files will be staged

Merge steps:

1. Switch to `integration_branch` and confirm status.
2. Merge the branch.
3. Resolve only relevant conflicts.
4. Run the focused test on `integration_branch`.
5. Mark tracker `done/pass` only after integration validation passes; mark blocked items `blocked/n/a`.
6. Commit merge and tracker update.
7. After a batch, run the broadest practical TestTheSpire validation.

## Completion Criteria

The campaign is complete when:

- tracker has `todo 0`
- implementable cards are `done/pass`
- non-implementable cards are `blocked/n/a` with concrete reasons
- agreed final validation passed or the limitation is recorded
- no worker/review thread remains needed
- final answer lists integrated work, blocked items, validation summary, and unrelated dirty files
