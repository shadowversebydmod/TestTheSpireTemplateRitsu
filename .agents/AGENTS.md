# AGENTS.md

## Bundled Skills

This template consumes project skills from the `bundled_skills` submodule. Before using AI-assisted development in a fresh checkout, run:

```bash
git submodule update --init --recursive
```

The active skill paths are symlinks:

- `.agents/skills/card-task-orchestrator` -> `../../bundled_skills/skills/card-task-orchestrator`
- `.agents/skills/sts2-card-creator` -> `../../bundled_skills/skills/sts2-card-creator`

Do not edit the symlink targets from inside this template repository. Update the TestTheSpireSkills repository, commit there, then update the `bundled_skills` submodule pointer here.
