# -*- coding: utf-8 -*-
"""
Created on Sat Oct 26 11:44:21 2019

@author: McSmoker
"""

import requests
from cerberus import Validator
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
  'WeightUnitId':{'type':'number'}
}
foodschema = {
  'FoodId':{'type':'number'},
  'Name':{'type':'string'},
  'KCal':{'type':'number'},
  'Protein':{'type':'number'},
  'Fiber':{'type':'number'},
  'Calcium':{'type':'number'},
  'Sodium':{'type':'number'},
  'PortionSize':{'type':'number'},
  'WeightUnitId':{'type':'number'}
}


#methods to test
def foodsearch():
    r= requests.get(urlLocal+foodPartial+"/11/ban")
    j= r.json()
    return j
def test_foodsearch():
    v = Validator(foodschema)
    j = foodsearch()
    assert v.validate(j[0]) == "true"
    assert j[0]['Name'] == "banaan"


##ff checken of er uberhaupt iets is

#???
def inc(x):
  return x + 2
def test_inc():
  assert inc(3) == 5
##schema's 






