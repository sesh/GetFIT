Generate Garmin-compatible FIT Workout files from a simple JSON workout structure.

Example JSON for the `/api/getfitfromjson` endpoint:

```json
{
  "name": "GetFIT Test",
  "steps": [
    {
      "intensity": "warmup",
      "duration": 480,
      "targetSpeedLow": null,
      "targetSpeedHigh": null
    },
    {
      "intensity": "interval",
      "duration": 120,
      "targetSpeedLow": 4.0,
      "targetSpeedHigh": 4.67
    },
    {
      "intensity": "recovery",
      "duration": 180,
      "targetSpeedLow": null,
      "targetSpeedHigh": null
    },
    {
      "intensity": "interval",
      "duration": 120,
      "targetSpeedLow": 4.0,
      "targetSpeedHigh": 4.67
    },
    {
      "intensity": "recovery",
      "duration": 150,
      "targetSpeedLow": null,
      "targetSpeedHigh": null
    },
    {
      "intensity": "interval",
      "duration": 75,
      "targetSpeedLow": 4.0,
      "targetSpeedHigh": 4.67
    },
    {
      "intensity": "recovery",
      "duration": 135,
      "targetSpeedLow": null,
      "targetSpeedHigh": null
    },
    {
      "intensity": "interval",
      "duration": 90,
      "targetSpeedLow": 4.0,
      "targetSpeedHigh": 4.67
    },
    {
      "intensity": "recovery",
      "duration": 210,
      "targetSpeedLow": null,
      "targetSpeedHigh": null
    },
    {
      "intensity": "cooldown",
      "duration": 240,
      "targetSpeedLow": null,
      "targetSpeedHigh": null
    }
  ]
}
```
## Run Locally

```
func start
```

## Publish

```
func azure functionapp publish GetFitFile
```