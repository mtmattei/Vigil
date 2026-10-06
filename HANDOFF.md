# HANDOFF — Vigil Phase 1 → Phase 2 (rooted build)
Updated: 2026-10-06 16:20 (America/Montreal)

## Where we are
Phase 1 finished in an unrooted session (home folder, so no App MCP). Done: concept picked with
`/product-thinking` Quick mode (Vigil, veterinary anesthesia record, verdict Test first, vs event-rental
turnaround Reframe/Park; `docs/VALIDATION.md`, `docs/DECISIONS.md` D1); visual direction via
`xaml-art-direction` written as `docs/design/DESIGN.md`; theme generated with DesignMd2Uno into
`docs/design/generated/` (24/24 contrast pairs pass; not yet copied into the app); `SPEC.md` with the
four briefs, capability inventory, spec gate and Quality Gates; scaffold from stable Uno.Templates 6.7.30
(isolated hive `~/source/tools/uno-templates-6.7.30-hive`; the global templates are 7.0-dev). The app is
still the template MainPage. No UI work has been done.

The user asked for a **non-stop run until completion**: keep going through SPEC.md's Implementation
Plan and Phase 3 gates; stop only when `docs/COMPLETENESS.md` + `docs/DEPLOY.md` exist with evidence
and CI is green, or when blocked on a choice only the user can make.

## Last verified state
- Build: pass, `net10.0-desktop`, 0 warnings, 27 s (2026-10-06 16:13).
- Runtime: not verified (no App MCP in the scaffolding session).
- Git: `main`, last commit `4315068 docs: add Vigil spec, design direction and generated theme`; clean. No remote yet.
- Lint: all gating counts 0 (template only).

## Guidance applied
| Skill / rule / decision | Applied at (file:line) |
|---|---|
| Stable templates in isolated hive (gotcha: 7.0-dev templates) | `global.json:4` (Uno.Sdk 6.7.30) |
| `.mcp.json` seeded from template | `.mcp.json:1` |
| MVUX, Material, regions, localization (fr) | `Vigil/Vigil.csproj:23-34` |

Read, not applied yet: uno-navigation, mvux, xaml-design-polish, capability-coverage (all spec-only so far).

## Next actions (in order)
1. Rooted session: confirm `uno_health` lists the App MCP tools. If not, stop and say so.
2. Start `docs/fleet/BUILD-LOG.md` with `uno-build-log`. Back-fill Phase 1 milestones: session start
   ~15:30, VALIDATION 15:55, scaffold 16:13 (build 27 s), spec 16:18.
3. SPEC.md *Implementation Plan* step 1 (foundation + Board), then step 2 (**release path before screen 2**:
   private repo `mtmattei/Vigil`, CI from Patina's `ci.yml` adapted, iOS Xcode 26.3 + workload 10.0.300 pin).
4. Steps 3–9, then Phase 3 gates in SPEC order (break-it → gold-standard-pass → uno-audit → atlas extract →
   uno-completeness-audit → `docs/COMPLETENESS.md`, `docs/DEPLOY.md`).

## Open questions
- SPEC.md *Unresolved Questions* (chart substitution, no audio reminder, CSV instead of PMS, no sync,
  no auth, no monitor integration, no photo): decided by default for the sample; surface them at the end.
- Template still has `MainWindow.UseStudio()` ungated (App.xaml.cs); keep it for App MCP, gate only for headless runs.

## Relaunch
```
pwsh -File ~/.claude/scripts/Open-RootedSession.ps1 -Path C:\Users\Platform006\Vigil
cd C:\Users\Platform006\Vigil; dotnet build Vigil/Vigil.csproj -f net10.0-desktop
```
DesignMd2Uno (regenerate theme): `dotnet run --project ~/source/tools/Designmd2uno/DesignMd2Uno/DesignMd2Uno/src/DesignMd2Uno.Cli -- docs/design/DESIGN.md -o Vigil/Styles --font-root ms-appx:///Assets/Fonts/`
