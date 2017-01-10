# Tdmts.WiFiPositioning


This project is designed as a Proof Of Concept Universal App to get the approximate distance from a WiFi access point.

Currently it:
-  Lists the Access Points in the area together with their Received Signal Strength Indicator (RSSI)
-  Estimates the distance based on this formula: d = 10 ^ ((TxPower - RSSI) / (10 * n))

Where:
-  TxPower is the RSSI measured at 1m from a known AP.
-  n is the propagation constant or path-loss exponent. For example: 2.7 to 4.3 (Free space has n =2 for reference).
-  RSSI is the measured RSSI
-  d is the distance in meter

Ideally it should measure your location in a room with X, Y and Z coördinates with an accuracy rate of less than 2 meter.
