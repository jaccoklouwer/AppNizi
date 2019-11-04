# -*- coding: utf-8 -*-
"""
Created on Sat Oct 26 11:44:21 2019

@author: McSmoker
"""

import requests
import json
import cerberus
from cerberus import Validator

##urls
urlLocal = "http://localhost:7071/api/v1"

food="/food"
foodPartial="/food/partial"
foodFavorites="/food/favorite"

patient ="/patient"
patients ="/patients"
patientMe ="/patients/me"

doctor="/doctor"

consumption="/consumption"
consumptions ="/consumptions"

waterconsumption="/waterconsumption"
waterconsumptiondaily="/waterconsumption/daily"
waterconsumptionperiod="/waterconsumption/period"

dietarymanagement="/dietaryManagement"

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
  'FoodId':{'type':'number'},
  'Name':{'type':'string'},
  'KCal':{'type':'number'},
  'Protein':{'type':'number'},
  'Fiber':{'type':'number'},
  'Calcium':{'type':'number'},
  'Sodium':{'type':'number'},
  'PortionSize':{'type':'number'},
  'WeightUnit':{'type':'string'},
  'Picture':{'type':'string'},
}

consumptionschema = {
    'PatientId': {'type':'number'}, 
    'ConsumptionId': {'type':'number'}, 
    'FoodName': {'type':'string'}, 
    'KCal': {'type':'number'}, 
    'Protein': {'type':'number'}, 
    'Fiber': {'type':'number'}, 
    'Calium': {'type':'number'}, 
    'Sodium': {'type':'number'}, 
    'Amount': {'type':'number'}, 
    'Weight': {'type':'dict',
               'schema':{
                    'Id': {'type':'number'}, 
                    'Unit': {'type':'string'}, 
                    'Short': {'type':'string'}
                        }},
    'Date': {'type':'string'},
    'Valid': {'type':'boolean'}
}
consumptiondateschema ={
    'Consumptions': {'type':'list',
                     'schema':{
                             'PatientId': {'type':'number'}, 
                             'ConsumptionId': {'type':'number'}, 
                             'FoodName': {'type':'string'}, 
                             'KCal': {'type':'number'}, 
                             'Protein': {'type':'number'}, 
                             'Fiber': {'type':'number'}, 
                             'Calium': {'type':'number'}, 
                             'Sodium': {'type':'number'}, 
                             'Amount': {'type':'number'}, 
                             'Weight': {'type':'dict',
                                        'schema':{
                                        'Id': {'type':'number'}, 
                                        'Unit': {'type':'string'}, 
                                        'Short': {'type':'string'}
                        }},
    'Date': {'type':'string'},
    'Valid': {'type':'boolean'}
                              }}, 
    'KCalTotal': {'type':'number'}, 
    'ProteinTotal': {'type':'number'}, 
    'FiberTotal': {'type':'number'}, 
    'CaliumTotal': {'type':'number'}, 
    'SodiumTotal': {'type':'number'}    
    }
waterconsumptionschema ={
        'weightUnit': {'type':'dict',
                       'schema':{
                               'id': {'type':'number'}, 
                               'unit': {'type':'string'}, 
                               'short': {'type':'string'}}},
        'error': {'type':'boolean'} , 
        'id': {'type':'number'} , 
        'amount': {'type':'number'} , 
        'date': {'type':'string'} , 
        'patientId': {'type':'number'} 
        }

header = {"Authorization": "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik5ERkdPRFUxTnpJNFJEZ3lNakkxUmtFMU5EZ3dRMEUxTkVJM05UTTBSRGRFUTBFNE5FWkdNZyJ9.eyJpc3MiOiJodHRwczovL2FwcG5pemkuZXUuYXV0aDAuY29tLyIsInN1YiI6ImRWWXRtU3c1bTgxOW1YMm5TMnJhTVp3bzVsWGN3RGc2QGNsaWVudHMiLCJhdWQiOiJhcHBuaXppLm5sL2FwaSIsImlhdCI6MTU3Mjg2Nzg0MiwiZXhwIjoxNTcyOTU0MjQyLCJhenAiOiJkVll0bVN3NW04MTltWDJuUzJyYU1ad281bFhjd0RnNiIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.UBh4Sruso9KcD4sFU80VnRMfUKXlb3uXyucTfxEXZTfTvYGSx1cseqvbp2-kksASXYXiNCbv34iOTLWeHYddCemZT5w6BdgMducBt4Wf_BP_krbU0FiGxb71L88BLxVUU8QGtyBCfqyTBfgmjYDUNPDmJKWRB2CblGyLH481HkAHzymNkfw9xBHJTAvMLnNGo_msvWC_O0akJgAr3N9eTjGxlV1CNHtHzPEYOJasHe1WzTpfFEgEezSMo7eSkPIaLzP35HVpcBijesc5CR6pwo3mhElMN2lfIsduUQtS0PdCLbRlcv2f3m1xFqL3dI-qIlFvjxP0jxqfFeBbs3xyBA.eyJpc3MiOiJodHRwczovL2FwcG5pemkuZXUuYXV0aDAuY29tLyIsInN1YiI6ImRWWXRtU3c1bTgxOW1YMm5TMnJhTVp3bzVsWGN3RGc2QGNsaWVudHMiLCJhdWQiOiJhcHBuaXppLm5sL2FwaSIsImlhdCI6MTU3Mjc3OTM4OSwiZXhwIjoxNTcyODY1Nzg5LCJhenAiOiJkVll0bVN3NW04MTltWDJuUzJyYU1ad281bFhjd0RnNiIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.s2k8FrRkaTuoVx_uFBOhKTAS6avfBZJ1GSY8gfFG-FTD0krv_mzlnKhjNsrFUtPKeM9XTSYq1uvZYmFRmQvU_xEffji_Is_fnn0sjmGCgOG8WRntaF4zeipNn9Q276UCSgxnsTss5dO6ihAiXNZeDAWm6j7y5MHvoDzwwAnp_nxKrhQEwIar37faTRQduVoWlzXmudOJ2qv3j259nUqRNkB1MzK1XcPzt7k6V85ZOqrsUUg-oU2eSdlaMcDfyEo3w_rey8mcDFo4YmjhSSGSlp4TNHayNBCyeehzQWDAdIrq3Z7275PtDoIYTJDqrLYJcH5wyFUmezZ8GlpQMGmDRA"}

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
consumptionitem ={
  "FoodName": "teveel stront",
  "KCal": 0,
  "Protein": 0,
  "Fiber": 0,
  "Calium": 0,
  "Sodium": 0,
  "Amount": 20,
  "WeightUnitId": 0,
  "Date": "2019-11-03T14:16:29.305Z",
  "PatientId": 11,
  "Id": 0
  }
waterconsumptionitem={
  "Id": 2,
  "amount": 100,
  "date": "2019-11-03T14:44:52.978Z",
  "PatientId": 11   
  }
patientitem={
  "firstName": "anus",
  "lastName": "piraate",
  "dateOfBirth": "11-11-2011",
  "weight": 0,
  "doctorId": 0
}
doctoritem={
  "firstName": "ho",
  "lastName": "mo",
  "location": "aarslaan"
}
dietarymanagementitem ={
  "Id": 2,
  "Description": "streng dieet",
  "Amount": 50,
  "IsActive": True,
  "PatientId": 11
}

#problematisch
def getwaterconsumptionbydate():
    r= requests.get(urlLocal+waterconsumptiondaily+"/11?date=20-10-1993" ,headers=header)
    print(r)
    j= r.json()
    return j
def getwaterconsumptionbydates():
    r= requests.get(urlLocal+waterconsumptionperiod+"/11&startDate=11-02-2019&endDate=11-04-2019" ,headers=header)
    print(r)
    j= r.json()
    return j

    
#consumption
def getconsumptionbyid():
    r= requests.get(urlLocal+consumption+"/9",headers = header)
    j= r.json()
    return j
def deleteconsumption(consumptionId):
    r= requests.delete(urlLocal+consumption+"/"+str(consumptionId),headers = header)
    return r.status_code
def postconsumption():
    r= requests.post(urlLocal+consumptions,data = json.dumps(consumptionitem) ,headers=header)
    return r.status_code
def putconsumption():
    r= requests.put(urlLocal+consumption+"/1",data= json.dumps(consumptionitem),headers = header)
    return r.status_code
def getconsumptionbydate():
    r= requests.get(urlLocal+consumptions+"?patientId=11&startDate=11-02-2019&endDate=11-04-2019",headers = header)
    j= r.json()
    return j

#wwater 
def postwaterconsumption():
    r= requests.post(urlLocal+waterconsumption,data = json.dumps(waterconsumptionitem) ,headers=header)
    return r.status_code
def getwaterconsumption():
    r= requests.get(urlLocal+waterconsumption+"/2" ,headers=header)
    j= r.json()
    return j

def putwaterconsumption():
    r= requests.put(urlLocal+waterconsumption+"/2",data= json.dumps(waterconsumptionitem),headers = header)
    return r.status_code
def deletewaterconsumption():
    r= requests.delete(urlLocal+waterconsumption+"/2",headers = header)
    return r.status_code

#patient
def getpatients():
    r= requests.get(urlLocal+patients ,headers=header)
    j= r.json()
    return j
def getpatientbyid():
    r= requests.get(urlLocal+patient+"/11" ,headers=header)
    j= r.json()
    return j
def deletepatient():
    r= requests.delete(urlLocal+patient+"/11",headers = header)
    return r.status_code
def registerpatient():
    r= requests.post(urlLocal+patient,data = json.dumps(patientitem) ,headers=header)
    return r.status_code
def getpatientme():
    r= requests.get(urlLocal+patient+"/me" ,headers=header)
    j= r.json()
    return j

#doctor
def getdoctors():
    r= requests.get(urlLocal+doctor ,headers=header)
    j= r.json()
    return j
def postdoctor():
    r= requests.post(urlLocal+doctor,data = json.dumps(doctoritem) ,headers=header)
    return r.status_code
def getdoctorbyid():
    r= requests.get(urlLocal+doctor+"/1" ,headers=header)
    j= r.json()
    return j
def deletedoctor():
    r= requests.delete(urlLocal+doctor+"/2",headers = header)
    return r.status_code
def getdoctorpatients():
    r= requests.get(urlLocal+doctor+"/2/patients" ,headers=header)
    j= r.json()
    return j
def getdoctorme():
    r= requests.get(urlLocal+doctor+"/me" ,headers=header)
    j= r.json()
    return j
#dietarymanagement
def test_putdietarymanagement():
    r= requests.put(urlLocal+dietarymanagement+"/8",data= json.dumps(dietarymanagementitem),headers = header)
    assert r.status_code == 200
def test_deletedietarymanagement():
    r= requests.delete(urlLocal+dietarymanagement+"/8",headers = header)
    assert r.status_code == 200
def test_postdietarymanagement():
    r= requests.post(urlLocal+dietarymanagement,data = json.dumps(dietarymanagementitem) ,headers=header)
    assert r.status_code == 200
def test_getdietarymanagement():
    r= requests.get(urlLocal+dietarymanagement+"/11" ,headers=header)
    j= r.json()
    assert r.status_code == 200
    assert len(j['Restrictions']) == 8



def test_getconsumptionbyid():
    v = Validator(consumptionschema)
    j = getconsumptionbyid()
    assert v.validate(j) == True
    assert j['ConsumptionId'] ==9
def test_getwaterconsumption():
    v = Validator(waterconsumptionschema)
    j = getwaterconsumption()
    assert v.validate(j) == True
    assert j['id'] ==2  
def test_getconsumptionbydate():
    v = Validator(consumptiondateschema)
    j = getconsumptionbydate()
    assert v.validate(j) == True
    ##asser waarde nog te doen maar ik weet niet hoe ik iets terugkrijg hier wat niet leeg is








#methods to test
def getfoodbysearch():
    r= requests.get(urlLocal+foodPartial+"/ban/20",headers = header)
    j= r.json()
    return j
def getfoodbyid():
    r= requests.get(urlLocal+food+"/1",headers = header)
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
    j= r.json()
    return j
def deletemeal(mealId):
    r= requests.delete(urlLocal+meal+"?patientId=11&mealId="+str(mealId),headers=header)
    return r.status_code
#tests
def test_foodsearch():
    v = Validator(foodschema)
    j = getfoodbysearch()
    assert v.validate(j[0]) == True
    assert "ban" in j[0]['Name']
def test_getfoodbyid():
    v = Validator(foodschema)
    j = getfoodbysearch()
    assert v.validate(j[0]) == True
    assert j[0]['FoodId'] == 1
def test_postfoodfavorite():
    r = postfoodfavorite()
    assert r == 200
def test_getfoodfavorites():
    v = Validator(foodschema)
    j = getfoodfavorites()
    assert v.validate(j[0]) == True
def test_deletefoodfavorite():
    r= deletefoodfavorite()
    assert r == 200
def test_getmeal():
    v = Validator(mealschema)
    j = getmeal()
    assert v.validate(j[0]) == True
    assert j[0]['PatientId'] == 11
def test_postanddeletemeal():
    v = Validator(mealschema)
    j = postmeal()
    assert v.validate(j) == True
    assert j['PatientId'] == 11
    mealId = j['MealId']
    r = deletemeal(mealId)
    assert r == 200


