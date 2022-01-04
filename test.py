from thttp import request


def get_fit():
    response = request("http://localhost:7071/api/getfitfromjson", json={
        'name': 'Test Workout',
        'steps': [{
            "intensity": "interval",
            "duration": 60000,
            "targetSpeedLow": None,
            "targetSpeedHigh": None
        }],
    }, method='post')

    print(response.status)
    print(response.headers)

    with open("fartlek.fit", "wb") as f:
        f.write(response.content)


if __name__ == "__main__":
    get_fit()