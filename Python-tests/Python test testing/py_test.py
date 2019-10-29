# -*- coding: utf-8 -*-
"""
Created on Sat Oct 26 11:44:21 2019

@author: McSmoker
"""

import requests
from cerberus import Validator


##schema's 

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



