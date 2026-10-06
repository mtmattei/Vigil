---
name: Vigil
description: Anesthesia record for surgical vet techs, read at arm's length beside the patient.
colors:
  primary: "#7A5A00"
  on-primary: "#FFFFFF"
  secondary: "#3E5F73"
  tertiary: "#00707F"
  background: "#F3F5F2"
  surface: "#FFFFFF"
  surface-variant: "#E6EBE6"
  text: "#121A17"
  text-muted: "#4E5B55"
  border: "#C4CEC8"
  error: "#B3261E"
typography:
  display:
    fontFamily: Archivo Narrow
    fontSize: 64px
    fontWeight: 600
    lineHeight: 68px
  h1:
    fontFamily: Archivo Narrow
    fontSize: 40px
    fontWeight: 600
    lineHeight: 44px
  h2:
    fontFamily: Archivo Narrow
    fontSize: 28px
    fontWeight: 600
    lineHeight: 32px
  title:
    fontFamily: Public Sans
    fontSize: 20px
    fontWeight: 600
    lineHeight: 26px
  body:
    fontFamily: Public Sans
    fontSize: 16px
    fontWeight: 400
    lineHeight: 24px
  body-sm:
    fontFamily: Public Sans
    fontSize: 14px
    fontWeight: 400
    lineHeight: 20px
  label:
    fontFamily: Public Sans
    fontSize: 13px
    fontWeight: 600
    lineHeight: 16px
  code:
    fontFamily: IBM Plex Mono
    fontSize: 15px
    fontWeight: 500
    lineHeight: 20px
rounded:
  sm: 6px
  md: 12px
  lg: 24px
spacing:
  xs: 4px
  sm: 8px
  md: 12px
  lg: 16px
  xl: 24px
  2xl: 32px
  3xl: 48px
---

# Vigil

Vigil replaces the paper anesthesia sheet clipped to the anesthesia machine. A vet tech glances at it
from a metre away, with gloves on, while watching a patient. Every value must read at arm's length,
and every tap target is at least 56 px.

## Colors

The palette comes from the anesthesia machine, not from software. Primary is the sevoflurane vaporizer
yellow (the agent's colour code): it marks the one thing due now, the next reading. Everything else is
graphite and paper.

- **Primary** `#7A5A00` light / `#F2C230` dark: the reading due now, the Record button, the current column.
- **Secondary** `#3E5F73`: ballpoint blue-slate for secondary actions and the drug row.
- **Tertiary** `#00707F`: oxygenation (SpO2), following the monitor convention.
- **Error** `#B3261E`: out-of-range values and alarms only. Never decoration.

Vital-sign series colours follow the patient-monitor convention and have no Material role. They are
app tokens (heart rate green, SpO2 cyan, blood pressure red, EtCO2 white-grey, temperature orange),
and each also carries a glyph: dot for heart rate, the paper record's chevrons for blood pressure,
ring for SpO2, cross for EtCO2, square for temperature. Colour is never the only cue.

### Dark

The theatre is dim and the monitor glows, so dark mode ("Theatre") is hand-tuned and is the default
during a procedure.

- Primary: #F2C230
- On primary: #1E1600
- Secondary: #9DB8C9
- Tertiary: #5CC8D7
- Error: #FF6B5E
- Background: #0F1312
- Surface: #161B1A
- Surface variant: #1E2523
- Text: #E4EAE6
- Text muted: #94A39C
- Border: #2C3532

## Typography

Archivo Narrow for numbers read from across the table (vitals, weight, elapsed time): condensed, so
four digits fit a tablet column at 64 px. Public Sans for interface text: it was designed for
government forms, and this is a medical form. IBM Plex Mono only for clock times and dose volumes,
where tabular figures stop columns from jittering as values change.

## Layout

A 4 px base grid. The record is laid out like the paper sheet: a patient band across the top (name,
species, weight, ASA status, like a wristband), the five-minute strip below, and the entry pad beside
it on a tablet or under it on a phone.

## Components

Buttons are 56 px tall (gloved taps). Steppers for vitals are 64 px square. Cards are outlined with the
border colour; shadows are reserved for the floating Record button.
