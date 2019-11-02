# -*- coding: utf-8 -*-
"""
Created on Sun Oct 27 10:54:09 2019

@author: McSmoker
"""


import requests
import pytest
from cerberus import Validator

##schema's 
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
  'WeightUnitId':{'type':'number'}
}
schema = {'name': {'type': 'string'}, 'age': {'type': 'integer', 'min': 10}}
##urls
urlLocal = "http://localhost:7071/api/v1"

food="/food"
foodPartial="/food/partial"
foodFavorites="/food/favorite"
foodAddFavorite ="/test/postfavorite"

patient ="/patients"
patientMe ="/patients/me"

doctor="/doctor/patients"

consumption="/consumption"

waterconsumption="/waterconsumption"
waterconsumptionperiod="/waterconsumption/period"

dietarymanagement="/dietarymanagement"

mealget="/meal/get"
mealadd="/meal/add"
mealdelete="/meal/delete"
##:()
def inc(x):
  return x + 1
def test_inc():
  assert inc(3) == 5


##Gets
#meal
v = Validator(mealschema)
r= requests.get(urlLocal+mealget+"/3")
j= r.json()
##ff checken of er uberhaupt iets is
print(j[0])
##checkt of het object hetzelfde is als mealschema
print(v.validate(j[0]))
##checken of het de juist patientId is
assert j[0]['PatientId'] == 3
#   print("succes by getmeal")
#except:
#    print("fout bij getmeal"+r.reason)

##Gets
#meal
try:
    r= requests.get(urlLocal+mealget+"/3")
    j= r.json()
    print("succes by getmeal")
except:
    print("fout bij getmeal")


#food
try:
    r= requests.get(urlLocal+foodPartial+"/ban")
    j= r.json()
    print("succes by foodsearch")
except:
    print("fout bij foodsearch")
try:
    r= requests.get(urlLocal+food+"/banaan")
    j= r.json()
    print("succes by getfood")
except:
    print("fout by foodfullname")
try:
    r= requests.get(urlLocal+foodFavorites+"/3")
    j= r.json()
    print("succes by foodfavorites")
except:
    print("fout by foodfavorites "+r.reason)
#patient
try:
    r= requests.get(urlLocal+patient+"/3")
    j= r.json()
    print("succes by getpatient")
except:
    print("fout by getpatient "+r.reason)
try:
    r= requests.get(urlLocal+patientMe)
    j= r.json()
    print("succes by patientMe")
except:
    print("fout by patientMe")
try:
    r= requests.get(urlLocal+patient)
    j= r.json()
    print("succes by patientlist")
except:
    print("fout bij patientlist")
#consumption
try:
    r= requests.get(urlLocal+consumption+"/1")
    j= r.json()
    print("succes by consumptionget")
except:
    print("fout by consumptionget")
#waterconsumptionTODO DATE
try:
    r= requests.get(urlLocal+waterconsumption+"/3")
    j= r.json()
    print("succes by waterconsumptionget")
except:
    print("fout by wateconsumptionget")
try:
    r= requests.get(urlLocal+waterconsumptionperiod)
    j= r.json()
    print("succes by waterconsumptionperiodget")
except:
    print("fout by waterconsumptionperiodget")
#dietarymanagemt
try:
    r= requests.get(urlLocal+dietarymanagement+"/3")
    j= r.json()
    print("succes by dietarymanagementget")
except:
    print("fout by dietarymangement get")

#posts
#meals
mealparam = {
  "mealId": 1,
  "name": "pythonaddedcake",
  "patientId": 3,
  "kCal": 10.0,
  "protein": 6.0,
  "fiber": 7.0,
  "calcium": 9.0,
  "sodium": 10.0,
  "portionSize": 5.0,
  "weightUnitId": 1
  }
try:
    r= requests.post(urlLocal+mealadd+"/3",json=mealparam)
    j= r.json()
    print("succes by meal add")
except:
    print("fout by meal add " +r.reason)
#patient
try:
    r= requests.post(urlLocal+patient)
    j= r.json()
    print("succes by create patient")
except:
    print("fout by create patient")
#waterconsumption
try:
    r= requests.post(urlLocal+waterconsumption+"/3")
    j= r.json()
    print("succes by consumption adden")
except:
    print("fout bij waterconsumption adden")
try:
    r= requests.post(urlLocal+waterconsumptionperiod)
    j= r.json()
    print("succes by waterconsumtion period adden")
except:
    print("fout by waterconsumption period adden???wtf is dat uberhaupt")

#delete
#dietarymanagemnt
try:
    r= requests.delete(urlLocal+dietarymanagement+"/3")
    j= r.json()
    print("succes by delete dietarymanagement")
except:
    print("fout by delete dietarymanagement")
#meal
try:
    r= requests.delete(urlLocal+mealdelete+"/3"+"/1")
    j= r.json()
    print("succes by mealdelete")
except:
    print("fout by mealdelete")
#patient
try:
    r= requests.delete(urlLocal+patient)
    j= r.json()
    print("succes by patientdelete")
except:
    print("fout by deletepatient")

#put
try:
    r= requests.put(urlLocal+dietarymanagement)
    j= r.json()
    print("succes by update diet" )
except:
    print("fout by update diet")
