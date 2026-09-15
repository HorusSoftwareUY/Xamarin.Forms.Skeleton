# Agent skills

This folder is the **source** of the skills this library ships. Agents do not look here — they look
inside *your* project, each in its own directory. Copy the skill there:

| Agent | Put it in |
| --- | --- |
| Claude Code | `.claude/skills/skeleton/SKILL.md` |
| Codex | `.agents/skills/skeleton/SKILL.md` |
| GitHub Copilot | either of the above, or `.github/skills/skeleton/SKILL.md` |
| Cursor | either of the above, or `.cursor/skills/skeleton/SKILL.md` |

One command, run from the root of your app:

```bash
mkdir -p .claude/skills/skeleton && curl -fsSL https://skills.horus.com.uy/skeleton -o .claude/skills/skeleton/SKILL.md
```

Swap `.claude` for `.agents` for Codex. Or simply tell your agent:

> Read https://skills.horus.com.uy/skeleton and add skeleton loading to MainPage.xaml

The skill is a single Markdown file with no scripts — open the link and read it before you install it.
