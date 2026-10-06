# Vigil: DESIGN.md → Uno Material

Source: `DESIGN.md`. Files: `ColorPaletteOverride.xaml`, `Typography.xaml`, `Tokens.xaml`.

> Anesthesia record for surgical vet techs, read at arm's length beside the patient.

Status legend: **mapped** = taken from DESIGN.md, **inferred** = computed from other tokens, **defaulted** = Material default kept, **custom** = emitted under a non-Material key, **dropped** = read but not emitted.

## Summary

```
               mapped  inferred defaulted    custom   dropped
  color            22        34         0         0         0
  type              7         0         8         1         0
  font              0         0         0         0         0
  spacing           7         0         0         0         0
  radius            3         0         0         0         0
  total            39        34         8         1         0

  contrast  24/24 text/background pairs pass 4.5:1
```

## Colors

| Token | Value | Uno resource | Status | Line | Note |
| --- | --- | --- | --- | ---: | --- |
| primary | `#7A5A00` | PrimaryColor (light) | mapped | 5 |  |
| on-primary | `#FFFFFF` | OnPrimaryColor (light) | mapped | 6 |  |
| secondary | `#3E5F73` | SecondaryColor (light) | mapped | 7 |  |
| tertiary | `#00707F` | TertiaryColor (light) | mapped | 8 |  |
| background | `#F3F5F2` | BackgroundColor (light) | mapped | 9 |  |
| surface | `#FFFFFF` | SurfaceColor (light) | mapped | 10 |  |
| surface-variant | `#E6EBE6` | SurfaceVariantColor (light) | mapped | 11 |  |
| text | `#121A17` | OnBackgroundColor (light) | mapped | 12 |  |
| text-muted | `#4E5B55` | OnSurfaceVariantColor (light) | mapped | 13 |  |
| border | `#C4CEC8` | OutlineColor (light) | mapped | 14 |  |
| error | `#B3261E` | ErrorColor (light) | mapped | 15 |  |
| Primary (dark) | `#F2C230` | PrimaryColor (dark) | mapped | 98 |  |
| On primary (dark) | `#1E1600` | OnPrimaryColor (dark) | mapped | 99 |  |
| Secondary (dark) | `#9DB8C9` | SecondaryColor (dark) | mapped | 100 |  |
| Tertiary (dark) | `#5CC8D7` | TertiaryColor (dark) | mapped | 101 |  |
| Error (dark) | `#FF6B5E` | ErrorColor (dark) | mapped | 102 |  |
| Background (dark) | `#0F1312` | BackgroundColor (dark) | mapped | 103 |  |
| Surface (dark) | `#161B1A` | SurfaceColor (dark) | mapped | 104 |  |
| Surface variant (dark) | `#1E2523` | SurfaceVariantColor (dark) | mapped | 105 |  |
| Text (dark) | `#E4EAE6` | OnBackgroundColor (dark) | mapped | 106 |  |
| Text muted (dark) | `#94A39C` | OnSurfaceVariantColor (dark) | mapped | 107 |  |
| Border (dark) | `#2C3532` | OutlineColor (dark) | mapped | 108 |  |
| PrimaryContainer (light) | `#F0E3CB` | PrimaryContainerColor | inferred |  | Primary at OKLCH L 0.92 |
| OnPrimaryContainer (light) | `#362600` | OnPrimaryContainerColor | inferred |  | Primary at OKLCH L 0.28, ≥4.5:1 |
| OnSecondary (light) | `#FFFFFF` | OnSecondaryColor | inferred |  | white, best contrast (6.8:1) |
| SecondaryContainer (light) | `#DAE7EF` | SecondaryContainerColor | inferred |  | Secondary at OKLCH L 0.92 |
| OnSecondaryContainer (light) | `#132C3A` | OnSecondaryContainerColor | inferred |  | Secondary at OKLCH L 0.28, ≥4.5:1 |
| OnTertiary (light) | `#FFFFFF` | OnTertiaryColor | inferred |  | white, best contrast (5.8:1) |
| TertiaryContainer (light) | `#CFEBF0` | TertiaryContainerColor | inferred |  | Tertiary at OKLCH L 0.92 |
| OnTertiaryContainer (light) | `#002F36` | OnTertiaryContainerColor | inferred |  | Tertiary at OKLCH L 0.28, ≥4.5:1 |
| OnError (light) | `#FFFFFF` | OnErrorColor | inferred |  | white, best contrast (6.5:1) |
| ErrorContainer (light) | `#FFDBD5` | ErrorContainerColor | inferred |  | Error at OKLCH L 0.92 |
| OnErrorContainer (light) | `#540001` | OnErrorContainerColor | inferred |  | Error at OKLCH L 0.28, ≥4.5:1 |
| OnSurface (light) | `#121A17` | OnSurfaceColor | inferred |  | OnBackground, checked against Surface |
| OutlineVariant (light) | `#E1E6E3` | OutlineVariantColor | inferred |  | halfway between Outline and Surface |
| SurfaceTint (light) | `#7A5A00` | SurfaceTintColor | inferred |  | Primary |
| SurfaceInverse (light) | `#121A17` | SurfaceInverseColor | inferred |  | OnSurface, L ≤ 0.3 |
| OnSurfaceInverse (light) | `#FFFFFF` | OnSurfaceInverseColor | inferred |  | Surface on SurfaceInverse |
| PrimaryInverse (light) | `#F2C230` | PrimaryInverseColor | inferred |  | dark Primary |
| PrimaryContainer (dark) | `#44320A` | PrimaryContainerColor | inferred |  | light Primary at L 0.33 |
| OnPrimaryContainer (dark) | `#EADCC1` | OnPrimaryContainerColor | inferred |  | light Primary at L 0.90, ≥4.5:1 |
| OnSecondary (dark) | `#071822` | OnSecondaryColor | inferred |  | ink, best contrast (8.7:1) |
| SecondaryContainer (dark) | `#263843` | SecondaryContainerColor | inferred |  | light Secondary at L 0.33 |
| OnSecondaryContainer (dark) | `#D2E0EA` | OnSecondaryContainerColor | inferred |  | light Secondary at L 0.90, ≥4.5:1 |
| OnTertiary (dark) | `#001A1E` | OnTertiaryColor | inferred |  | ink, best contrast (9.2:1) |
| TertiaryContainer (dark) | `#0A3C44` | TertiaryContainerColor | inferred |  | light Tertiary at L 0.33 |
| OnTertiaryContainer (dark) | `#C5E5EB` | OnTertiaryContainerColor | inferred |  | light Tertiary at L 0.90, ≥4.5:1 |
| OnError (dark) | `#320001` | OnErrorColor | inferred |  | ink, best contrast (6.6:1) |
| ErrorContainer (dark) | `#611711` | ErrorContainerColor | inferred |  | light Error at L 0.33 |
| OnErrorContainer (dark) | `#FFD2CB` | OnErrorContainerColor | inferred |  | light Error at L 0.90, ≥4.5:1 |
| OnSurface (dark) | `#E4EAE6` | OnSurfaceColor | inferred |  | dark OnBackground |
| OutlineVariant (dark) | `#212825` | OutlineVariantColor | inferred |  | halfway between Outline and Surface |
| SurfaceTint (dark) | `#F2C230` | SurfaceTintColor | inferred |  | dark Primary |
| SurfaceInverse (dark) | `#FFFFFF` | SurfaceInverseColor | inferred |  | light Surface |
| OnSurfaceInverse (dark) | `#121A17` | OnSurfaceInverseColor | inferred |  | light OnSurface |
| PrimaryInverse (dark) | `#7A5A00` | PrimaryInverseColor | inferred |  | light Primary |

## Contrast (WCAG 2.x)

| Theme | Text | Background | Ratio | Result |
| --- | --- | --- | ---: | --- |
| light | OnPrimary | Primary | 6.38:1 | pass |
| light | OnPrimaryContainer | PrimaryContainer | 11.56:1 | pass |
| light | OnSecondary | Secondary | 6.80:1 | pass |
| light | OnSecondaryContainer | SecondaryContainer | 11.48:1 | pass |
| light | OnTertiary | Tertiary | 5.79:1 | pass |
| light | OnTertiaryContainer | TertiaryContainer | 11.45:1 | pass |
| light | OnError | Error | 6.54:1 | pass |
| light | OnErrorContainer | ErrorContainer | 11.87:1 | pass |
| light | OnBackground | Background | 16.15:1 | pass |
| light | OnSurface | Surface | 17.71:1 | pass |
| light | OnSurfaceVariant | SurfaceVariant | 5.89:1 | pass |
| light | OnSurfaceInverse | SurfaceInverse | 17.71:1 | pass |
| dark | OnPrimary | Primary | 10.71:1 | pass |
| dark | OnPrimaryContainer | PrimaryContainer | 9.10:1 | pass |
| dark | OnSecondary | Secondary | 8.71:1 | pass |
| dark | OnSecondaryContainer | SecondaryContainer | 9.04:1 | pass |
| dark | OnTertiary | Tertiary | 9.15:1 | pass |
| dark | OnTertiaryContainer | TertiaryContainer | 8.99:1 | pass |
| dark | OnError | Error | 6.60:1 | pass |
| dark | OnErrorContainer | ErrorContainer | 9.34:1 | pass |
| dark | OnBackground | Background | 15.34:1 | pass |
| dark | OnSurface | Surface | 14.27:1 | pass |
| dark | OnSurfaceVariant | SurfaceVariant | 5.94:1 | pass |
| dark | OnSurfaceInverse | SurfaceInverse | 17.71:1 | pass |

## Typography

| Token | Value | Uno resource | Status | Line | Note |
| --- | --- | --- | --- | ---: | --- |
| display | `Archivo Narrow · 64/68 · 600` | DisplayLarge | mapped | 17 |  |
| DisplayMedium | `Archivo Narrow · 45/52 · 400` | DisplayMedium | defaulted |  | Material default size, heading family |
| DisplaySmall | `Archivo Narrow · 36/44 · 400` | DisplaySmall | defaulted |  | Material default size, heading family |
| h1 | `Archivo Narrow · 40/44 · 600` | HeadlineLarge | mapped | 22 |  |
| h2 | `Archivo Narrow · 28/32 · 600` | HeadlineMedium | mapped | 27 |  |
| HeadlineSmall | `Archivo Narrow · 24/32 · 400` | HeadlineSmall | defaulted |  | Material default size, heading family |
| title | `Public Sans · 20/26 · 600` | TitleLarge | mapped | 32 |  |
| TitleMedium | `Public Sans · 16/24 · 500` | TitleMedium | defaulted |  | Material default size, body family |
| TitleSmall | `Public Sans · 14/20 · 500` | TitleSmall | defaulted |  | Material default size, body family |
| BodyLarge | `Public Sans · 16/24 · 400` | BodyLarge | defaulted |  | Material default size, body family |
| body | `Public Sans · 16/24 · 400` | BodyMedium | mapped | 37 |  |
| body-sm | `Public Sans · 14/20 · 400` | BodySmall | mapped | 42 |  |
| label | `Public Sans · 13/16 · 600` | LabelLarge | mapped | 47 |  |
| LabelMedium | `Public Sans · 12/16 · 500` | LabelMedium | defaulted |  | Material default size, body family |
| LabelSmall | `Public Sans · 11/16 · 500` | LabelSmall | defaulted |  | Material default size, body family |
| code | `IBM Plex Mono · 15/20 · 500` | CodeTextStyle | custom | 52 | no Material type role; emitted as a custom TextBlock style |

## Spacing and radius

| Token | Value | Uno resource | Status | Line | Note |
| --- | --- | --- | --- | ---: | --- |
| xs | `4px` | SpacingXs (+Thickness) | mapped | 62 |  |
| sm | `8px` | SpacingSm (+Thickness) | mapped | 63 |  |
| md | `12px` | SpacingMd (+Thickness) | mapped | 64 |  |
| lg | `16px` | SpacingLg (+Thickness) | mapped | 65 |  |
| xl | `24px` | SpacingXl (+Thickness) | mapped | 66 |  |
| 2xl | `32px` | Spacing2xl (+Thickness) | mapped | 67 |  |
| 3xl | `48px` | Spacing3xl (+Thickness) | mapped | 68 |  |
| sm | `6px` | RadiusSm (+CornerRadius) | mapped | 58 |  |
| md | `12px` | RadiusMd (+CornerRadius) | mapped | 59 |  |
| lg | `24px` | RadiusLg (+CornerRadius) | mapped | 60 |  |

## Not converted

These sections are guidance for people and agents, not tokens. Keep them next to the generated XAML.

- line 117: Layout
- line 123: Components
