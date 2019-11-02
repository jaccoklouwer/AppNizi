# -*- coding: utf-8 -*-
"""
Created on Sat Oct 26 11:44:21 2019

@author: McSmoker
"""

import requests
import cerberus
import json
from cerberus import Validator

decimal_type = cerberus.TypeDefinition('decimal', (Decimal,), ())

Validator.types_mapping['decimal'] = decimal_type
##urls
urlLocal = "http://localhost:7071/api/v1"

food="/food"
foodPartial="/food/partial"
foodFavorites="/food/favorite"

patient ="/patients"
patientMe ="/patients/me"

doctor="/doctor/patients"

consumption="/consumption"

waterconsumption="/waterconsumption"
waterconsumptionperiod="/waterconsumption/period"

dietarymanagement="/dietarymanagement"

meal="/meal"
#schema's
mealschema = {
  'MealId':{'type':'number'},
  'Name':{'type':'string'},
  'PatientId':{'type':'number'},
  'KCal':{'type':'number'},
  'Protein':{'type':'number'},
  'Fiber':{'type':'number'},
  'Calcium':{'type':'number'},
  'Sodium':{'type':'number'},
  'PortionSize':{'type':'number'},
  'Picture':{'type':'string'},
  'WeightUnit':{'type':'string'},
}
foodschema = {
  'foodId':{'type':'number'},
  'name':{'type':'string'},
  'kCal':{'type':'number'},
  'protein':{'type':'number'},
  'fiber':{'type':'number'},
  'calcium':{'type':'number'},
  'sodium':{'type':'number'},
  'portionSize':{'type':'number'},
  'weightUnit':{'type':'string'},
  'picture':{'type':'string'},
}

header = {"Authorization": "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik5ERkdPRFUxTnpJNFJEZ3lNakkxUmtFMU5EZ3dRMEUxTkVJM05UTTBSRGRFUTBFNE5FWkdNZyJ9.eyJpc3MiOiJodHRwczovL2FwcG5pemkuZXUuYXV0aDAuY29tLyIsInN1YiI6ImRWWXRtU3c1bTgxOW1YMm5TMnJhTVp3bzVsWGN3RGc2QGNsaWVudHMiLCJhdWQiOiJhcHBuaXppLm5sL2FwaSIsImlhdCI6MTU3MjY5MTQzNCwiZXhwIjoxNTcyNzc3ODM0LCJhenAiOiJkVll0bVN3NW04MTltWDJuUzJyYU1ad281bFhjd0RnNiIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.T5FFc5zqsxa4i7pRyPhYd_DFnAEhAORJd-Gh4WVQ7bpNQPVxcmWJt8hckz3MTSeg7Fp7dJQmhVQ7fegQsXQ-90uO38gNRNSyW0XLkiNUVdjcyAz4NJGcqBkMlHPQy7zdB3f_w8ONOwFvW8iiMcN7kiAKTk5x97jjTO1jBI0-XJOTTUZemlexVP4GjD_iS8zEwi3Y0DWlujkrgDELlMlWMkLURUlTkg2Vfc7tM-ayrA2LqRZrDlVToWB4dPXgFFviR-W_iZImAMzF2mMbApt9VVBDGVcYzymqWpRJ2jXjjtmnIRfj11mpqLJm47WawdHgbRmjIRSqMXjdCv_gzxskUg"}

mealitem = {
  "mealId": 0,
  "name": "poeptaart",
  "patientId": 3,
  "kCal": 1.0,
  "protein": 2.0,
  "fiber": 3.0,
  "calcium": 5.0,
  "sodium": 7.0,
  "portionSize": 100.0,
  "WeightUnit": "Gram",
  "picture":"www.poep.nl"
}

#methods to test
def getfoodbysearch():
    r= requests.get(urlLocal+foodPartial+"/11/ban/20",headers = header)
    j= r.json()
    print(j)
    return j
def getfoodbyid():
    r= requests.get(urlLocal+food+"/11/1",headers = header)
    j= r.json()
    return j
def getfoodfavorites():
    r= requests.get(urlLocal+foodFavorites+"/11",headers = header)
    j= r.json()
    return j
def postfoodfavorite():
    r= requests.post(urlLocal+foodFavorites+"?patientId=11&foodId=3",headers = header)
    return r.status_code
def deletefoodfavorite():
    r= requests.delete(urlLocal+foodFavorites+"?patientId=11&foodId=3",headers = header)
    return r.status_code
def getmeal():
    r= requests.get(urlLocal+meal+"/11",headers=header)
    j= r.json()
    return j
def postmeal():
    r= requests.post(urlLocal+meal+"/11",data = json.dumps(mealitem) ,headers=header)
    return r.status_code
def deletemeal():
    r= requests.delete(urlLocal+meal+"/11/3",headers=header)
    j= r.json()
    return j
def test_foodsearch():
    v = Validator(foodschema)
    j = getfoodbysearch()
    assert v.validate(j[0]) == True
    assert "ban" in j[0]['name']
def test_getfoodbyid():
    v = Validator(foodschema)
    j = getfoodbysearch()
    assert v.validate(j[0]) == True
    assert j[0]['foodId'] == 1
#deze function is inderdaad niet werkend goed werk python
#def test_getfoodfavorites():
#    v = Validator(foodschema)
#    j = getfoodfavorites()
#    assert v.validate(j[0]) == True
def test_postfoodfavorite():
    r = postfoodfavorite()
    assert r == 200
def test_deletefoodfavorite():
    r= deletefoodfavorite()
    assert r == 200
def test_getmeal():
    v = Validator(mealschema)
    j = getmeal()
    assert v.validate(j[0]) == True
    assert j[0]['PatientId'] == 11
def test_postmeal():
    r = postfoodfavorite()
    assert r == 200
##moet ff aangepast zodat er gedelete wordt wat net gepost is
#def test_deletemeal():
#    v = Validator(foodschema)
#    r = postfoodfavorite()
    #doe hier een check of de code 200 is



