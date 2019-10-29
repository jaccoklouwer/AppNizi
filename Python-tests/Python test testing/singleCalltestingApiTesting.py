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

##Gets
#meal
v = Validator(mealschema)
#try:
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
