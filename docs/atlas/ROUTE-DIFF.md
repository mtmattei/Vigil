# Route diff: built app vs SPEC.md route tree (2026-10-07)

Source: `atlas extract Vigil/App.xaml.cs --app Vigil --source Vigil --out docs/atlas/vigil.appmodel.json`
(atlas parses `RegisterRoutes` and XAML `Navigation.Request` triggers).

## Nodes

| Spec route | Built | Notes |
|---|---|---|
| Board (default) | board | `IsDefault: true` |
| NewCase `!` | newCase | `ViewMap<NewCasePage, NewCaseModel>(ResultData: typeof(CaseRef))` |
| Record (`CaseRef`) | record | `DataViewMap<RecordPage, RecordModel, CaseRef>` |
| Dose `!` (`CaseRef`) | dose | `DataViewMap<DosePage, DoseModel, CaseRef>` |
| SignOff `!` (`CaseRef`) | signOff | `DataViewMap<SignOffPage, SignOffModel, CaseRef>` |
| Settings | settings | `ViewMap<SettingsPage, SettingsModel>` |

6 of 6 spec routes registered; no extra routes.

## Edges

| Spec edge | Atlas | Actual trigger | Status |
|---|---|---|---|
| Board → Settings | declared (Board_2) | `uen:Navigation.Request="Settings"` | match |
| Board list → Record (CaseRef) | declared (Board_13) | `ItemsRepeater uen:Navigation.Request="Record"` | match |
| Board live band → Record | not found | `Controls/LiveBandView.xaml` `Navigation.Request="Record"` + `Navigation.Data` | present; atlas does not attribute a UserControl's trigger to its host page |
| Board → NewCase `!` | not found | `BoardModel.NewCase` → `NavigateRouteForResultAsync<CaseRef>("!NewCase")` | present; code-driven by design (D2), atlas scans XAML triggers only |
| NewCase → Record | not found | `BoardModel.NewCase` navigates to `Record` with the returned `CaseRef` | present (desktop trace: `Some(CaseRef)`, success=True); see F-024 for WASM |
| Record → Dose `!` | declared (Record_29) | Drugs tab "Give a dose" | match (also Pre-op "Give premedication", same route) |
| Record → SignOff `!` | declared (Record_77) | Recovery tab "Sign off" | match |
| Record / Settings → Board (back) | n/a | `utu:NavigationBar` MainCommand | present; back is a stack pop, not a route edge |

Built route tree = spec route tree. The three edges atlas cannot see are explained above (UserControl trigger,
code-driven result navigation, D2).
