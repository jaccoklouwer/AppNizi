import http.client

conn = http.client.HTTPSConnection("appnizi.eu.auth0.com")

def get_auth_token():
    payload = '{"client_id":"BEWe620aN1psUCDMdtyymzpKvM7gmKJE","client_secret":"GWbroX4DQJEWBr0rZAKFOtj_qBJaFWCLG6M6arLkG4D7rEdN_He21gTYIC2TRbxR","audience":"appnizi.nl/api","grant_type":"client_credentials"}'
    headers = { 'content-type': "application/json" }
    conn.request("POST", "/oauth/token", payload, headers)

    res = conn.getresponse()
    data = res.read()

    print(data.decode("utf-8"))
    return data.decode("utf-8")
