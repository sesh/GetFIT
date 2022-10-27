from thttp import request


GETFIT_URL = "https://getfitfile.azurewebsites.net/api/getfitfromjson"
# GETFIT_URL = "http://localhost:7071/api/getfitfromjson"


def get_fit():
    response = request(GETFIT_URL, json={
        'name': 'Test Workout',
        'steps': [{
            "intensity": "interval",
            "duration": 60000,
            "targetSpeedLow": None,
            "targetSpeedHigh": None
        }, {
            "intensity": "active",
            "distance": 1000,
            "targetSpeenLow": 4.0,
            "targetSpeedHigh": 4.33
        }, {
            "intensity": "active",
            "duration": 1000,
            "targetSpeenLow": 3.3,
            "targetSpeedHigh": 4.8
        }]
    }, method='post')

    assert response.status == 200
    print(response.headers)

    with open("fartlek.fit", "wb") as f:
        f.write(response.content)

if __name__ == "__main__":
    get_fit()
