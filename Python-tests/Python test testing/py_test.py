# -*- coding: utf-8 -*-
"""
Created on Sat Oct 26 11:44:21 2019

@author: McSmoker
"""

import requests
import json
import cerberus
import http.client
import datetime

from cerberus import Validator

##urls
def defineurl():
    try:
        os.environ["AppURL"]
    except:
        return "https://appnizi-api.azurewebsites.net/api/v1"
    return os.environ["AppURL"]
urlLocal = defineurl()


food="/food"
foodPartial="/food/partial"
foodFavorites="/food/favorite"

patient ="/patient"
patients ="/patients"
patientMe ="/login/patient"

doctorMe ="/login/doctor"

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

patientconsumptionschema = {
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
waterconsumptionperiodschema = {
        'WeightUnit': {'type':'dict',
                       'schema':{
                               'Id': {'type':'number'}, 
                               'Unit': {'type':'string'}, 
                               'Short': {'type':'string'}}},
        'Error': {'type':'boolean'} , 
        'Id': {'type':'number'} , 
        'Amount': {'type':'number'} , 
        'Date': {'type':'string'} , 
        'PatientId': {'type':'number'} 
        }
waterconsumptiondailyschema={
        'total':{'type':'number'},
        'minimumRestriction':{'type':'number'},
        'waterConsumptions': {'type':'list',
                              'schema':{
                               'weightUnit': {'type':'dict',
                                              'schema':{
                                                      'id': {'type':'number'}, 
                                                      'unit': {'type':'string'}, 
                                                      'short': {'type':'string'}}},
                               'error': {'type':'boolean'} , 
                               'id': {'type':'number'} , 
                               'amount': {'type':'number'} , 
                               'date': {'type':'string'} , 
                               'patientId': {'type':'number'} }},
        }

patientschema={
        'Id':{'type':'number'},
        'HandlingDoctorId':{'type':'number'},
        'FirstName':{'type':'string'},
        'LastName':{'type':'string'},
        'DateOfBirth': {'type':'string'},
        'WeightInKilograms': {'type':'number'}
        }
patientmeschema={
        'patientId':{'type':'number'}, 
        'accountId': {'type':'number'}, 
        'doctorId': {'type':'number'}, 
        'firstName': {'type':'string'}, 
        'lastName': {'type':'string'}, 
        'dateOfBirth': {'type':'string'}, 
        'weightInKilograms': {'type':'number'}, 
        'guid': {'type':'string'}
        }
doctorschema={
        
        "doctorId": {'type':'number'},
        "firstName": {'type':'string'},
        "lastName": {'type':'string'},
        "location": {'type':'string'}
        }


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
  "FoodName": "Pittige pytest",
  "KCal": 3.0,
  "Protein": 4.0,
  "Fiber": 1.0,
  "Calium": 3.0,
  "Sodium": 2.0,
  "Amount": 100,
  "WeightUnitId": 1,
  "Date": "2019-11-03T14:16:29.305Z",
  "PatientId": 17
  }
waterconsumptionitem={
  "Id": 23,
  "amount": 100,
  "date": "2019-11-03T14:44:52.978Z",
  "PatientId": 11   
  }
patientregisteritem={
  "Account": {
    "AccountId": 0,
    "Role": "string"
  },
  "Patient": {
    "PatientId": 0,
    "AccountId": 0,
    "DoctorId": 0,
    "FirstName": "string",
    "LastName": "string",
    "DateOfBirth": "2019-11-04T13:37:41.486Z",
    "WeightInKilograms": 0,
    "Guid": "string"
  },
  "Doctor": {
    "DoctorId": 0,
    "FirstName": "string",
    "LastName": "string",
    "Location": "string"
  },
  "AuthLogin": {
    "Guid": "string",
    "Token": {
      "Scheme": "string",
      "AccesCode": "string"
    }
  }
}
doctoritem={
  "firstName": "ho",
  "lastName": "mo",
  "location": "aarslaan"
}
dietarymanagementitem ={
    "Id": 0,
    "Description": "Caloriebeperking",
    "amount" : 1492,
    "Patient": 17,
    "isActive" : 1
}

patientitembody ={
  "patientId": 17,
        }

#variablen
patientid = "/17"

def accestoken():
    conn = http.client.HTTPSConnection("appnizi.eu.auth0.com")

    payload = "{\"client_id\":\"dVYtmSw5m819mX2nS2raMZwo5lXcwDg6\",\"client_secret\":\"vN6N5HNG25MP-gjBPsVhf01dzuIPqAixFFImtGUU4vy4RuJwFEYcPnJg4r6EOOdr\",\"audience\":\"appnizi.nl/api\",\"grant_type\":\"client_credentials\"}"
        
    headers = { 'content-type': "application/json" }

    conn.request("POST", "/oauth/token", payload, headers)

    res = conn.getresponse()
    data = res.read()
    datadecoded = data.decode("utf-8")
    splitdata = datadecoded.split('"')

    return(splitdata[3])
print(accestoken())

def accestokendoctor():
    conn = http.client.HTTPSConnection("appnizi.eu.auth0.com")

    payload = "{\"client_id\":\"lyvNV89UXHNVDC7D8XFdv35HIpPNzFum\",\"client_secret\":\"9sieaPoIz42CxkelXM8jk_izfyyoAzpOkPyXs_ceRW5KS5slO0phSS_CShFXcaGu\",\"audience\":\"appnizi.nl/api\",\"grant_type\":\"client_credentials\"}"

    headers = { 'content-type': "application/json" }

    conn.request("POST", "/oauth/token", payload, headers)

    res = conn.getresponse()
    data = res.read()

    datadecoded = data.decode("utf-8")
    splitdata = datadecoded.split('"')

    return(splitdata[3])
header = {
        "Authorization": "Bearer "+accestoken(),
        'content-type' : "application/json"
        }
headerdoctor ={
        "Authorization": "Bearer "+accestokendoctor(),
        'content-type' : "application/json"
        }


#problematisch
def getwaterconsumptionbydate():
    r= requests.get(urlLocal+waterconsumptiondaily+"/17&date=2019-12-07 " ,headers=header)
    j= r.json()
    return j
def test_getwaterconsumptionbydate():
    v = Validator(waterconsumptionschema)
    j = getwaterconsumptionbydate()
    assert v.validate(j['waterConsumptions'][0]) == True
def getwaterconsumptionbydates():
    r= requests.get(urlLocal+waterconsumptionperiod+"/17?beginDate=2019-01-01&endDate=2019-12-30",headers=header)
    j= r.json()
    return j
#print(getwaterconsumptionbydates()[0])
def test_getwaterconsumptionbydates():
    v = Validator(waterconsumptionperiodschema)
    j = getwaterconsumptionbydates()
    assert v.validate(j[0]) == True


#Consumption
# Methods

def post_consumption(consumption_date):
    consumptionitem["Date"] = consumption_date
    r = requests.post(urlLocal+consumptions,data = json.dumps(consumptionitem) ,headers=header)
    return r.status_code

def get_consumptions_by_date(start_date):
    end_date = str(datetime.datetime.now().strftime('%Y-%m-%d'))  
    r= requests.get(urlLocal+consumptions+"?patientId=17&startDate=" + start_date + "&endDate=" + end_date ,headers = header)
    print(r)
    j= r.json()
    return j

def get_consumption_by_id(consumption_id):
    r= requests.get(urlLocal+consumption+"/"+str(consumption_id), headers = header)
    j= r.json()
    return j

def put_consumption(consumption_id):
    consumptionitem['FoodName'] = "EDITED Pittige pytest"
    r= requests.put(urlLocal+consumption+ "/" + str(consumption_id) ,data= json.dumps(consumptionitem),headers = header)
    return r.status_code

def delete_consumption(consumptionId):
    r= requests.delete(urlLocal+consumption+"/"+str(consumptionId),headers = header)
    return r.status_code


# Tests
def test_consumption():
    current_time = str(datetime.datetime.now().strftime('%Y-%m-%d'))  
    # Create new consumption
    r = post_consumption(current_time)
    assert r == 200

    # Get Consumptions (from new consumption date to now)
    v = Validator(patientconsumptionschema)
    j = get_consumptions_by_date(current_time)
    assert len(j['Consumptions']) > 0
    
    # Get the new consumption
    latest_consumption = j['Consumptions'][-1]
    new_consumption_id = latest_consumption['ConsumptionId']
    assert v.validate(latest_consumption) == True

    # Use the consumptionId of the new consumption to get the consumption with get_consumption_by_id
    v = Validator(consumptionschema)
    j = get_consumption_by_id(new_consumption_id)
    assert v.validate(j) == True
    assert j['ConsumptionId'] == new_consumption_id

    # Change the consumption name with put_consumption
    r = put_consumption(new_consumption_id)
    assert r == 200

    # Delete the new consumption
    r = delete_consumption(new_consumption_id)
    assert r == 200

    #wwater 
def postwaterconsumption():
    r= requests.post(urlLocal+waterconsumption,data = json.dumps(waterconsumptionitem) ,headers=header)
    j = r.json()
    return j

def deletewaterconsumption(id):
    r= requests.delete(urlLocal+waterconsumption+"/"+str(id),headers = header)
    return r.status_code
def test_postanddeletewaterconsumption():
    j = postwaterconsumption()
    assert j['amount']==100
    r = deletewaterconsumption(j['id'])
    assert r == 200
def getwaterconsumption():
    r= requests.get(urlLocal+waterconsumption+"/23" ,headers=header)
    j= r.json()
    return j
def test_getwaterconsumption():
    v = Validator(waterconsumptionschema)
    j = getwaterconsumption()
    assert v.validate(j) == True
    assert j['id'] ==23

def putwaterconsumption():
    r= requests.put(urlLocal+waterconsumption+"/23",data= json.dumps(waterconsumptionitem),headers = header)
    j = r.json()
    return j
def test_putwaterconsumption():
    v = Validator(waterconsumptionschema)
    j = putwaterconsumption()
    assert v.validate(j) == True


#patient
def getpatients():
    r= requests.get(urlLocal+patients ,headers=header)
    j= r.json()
    return j[0]
def test_getpatients():
    v = Validator(patientschema)
    j = getpatients()
    assert v.validate(j) == True
def getpatientbyid():
    r= requests.get(urlLocal+patient+"/17" ,headers=header)
    j= r.json()
    return j
def test_getpatientbyid():
    v = Validator(patientschema)
    j = getpatients()
    assert v.validate(j) == True
    

    
#def registerpatient():
#    r= requests.post(urlLocal+patient,data = json.dumps(patientregisteritem) ,headers=header)
#    #j = r.json()
#    return r.status_code
#def test_registerpatient():
#    r = registerpatient()
#    assert r == 200
    
def getpatientme():
    r= requests.get(urlLocal+patientMe ,headers=header)
    j= r.json()
    return j
def test_getpatientme():
    v = Validator(patientmeschema)
    j = getpatientme()
    validatethis = j['patient']
    assert v.validate(validatethis) == True

#doctor
def getdoctors():
    r= requests.get(urlLocal+doctor ,headers=header)
    j= r.json()
    return j[0]
def test_getdoctors():
    v = Validator(doctorschema)
    j = getdoctors()
    assert v.validate(j) == True
def getdoctorbyid():
    r= requests.get(urlLocal+doctor+"/1" ,headers=header)
    j= r.json()
    return j
def test_getdoctorbyid():
    v = Validator(doctorschema)
    j = getdoctorbyid()
    assert v.validate(j) == True
    
def getdoctorpatients():
    r= requests.get(urlLocal+doctor+"/4/patients" ,headers=headerdoctor)
    return r
def test_getdoctorpatients():
    r = getdoctorpatients()
    #dit lijkt een beetje een rare maar er zijn geen patients gekoppeld aan deze jonge
    #dus als ie leeg is dan is het 204
    assert r.status_code == 204
    
def getdoctorme():
    r= requests.get(urlLocal+doctorMe ,headers=headerdoctor)
    j= r.json()
    return j
def test_getdoctorme():
    v = Validator(doctorschema)
    j = getdoctorme()
    assert v.validate(j['doctor'])== True

#dietarymanagement
def test_putdietarymanagement():
    r = requests.put(urlLocal+dietarymanagement+"/8", data= json.dumps(dietarymanagementitem), headers = header)
    assert r.status_code == 200

def test_deletedietarymanagement():
    r = requests.get(urlLocal+dietarymanagement+patientid ,headers=header)
    j = r.json()
    managements = j["Dietarymanagement"]
    management = managements[len(managements) -1]
    r = requests.delete(urlLocal+dietarymanagement+"/"+str(management["id"]), headers = header)
    assert r.status_code == 200

def test_postdietarymanagement():
    r = requests.post(urlLocal+dietarymanagement, data = json.dumps(dietarymanagementitem), headers=header)
    assert r.status_code == 200

def test_getdietarymanagement():
    r = requests.get(urlLocal+dietarymanagement+patientid ,headers=header)
    assert r.status_code == 200

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
    r= requests.get(urlLocal+foodFavorites+"/17",headers = header)
    j= r.json()
    return j
def postfoodfavorite():
    r= requests.post(urlLocal+foodFavorites+"?patientId=17&foodId=3",headers = header)
    return r.status_code
def deletefoodfavorite():
    r= requests.delete(urlLocal+foodFavorites+"?patientId=17&foodId=3",headers = header)
    return r.status_code
def getmeal():
    r= requests.get(urlLocal+meal+"/17",headers=header)
    j= r.json()
    return j
def postmeal():
    r= requests.post(urlLocal+meal+"/17",data = json.dumps(mealitem) ,headers=header)
    j= r.json()
    return j
def deletemeal(mealId):
    r= requests.delete(urlLocal+meal+"?patientId=17&mealId="+str(mealId),headers=header)
    return r.status_code
def putmeal():
    r= requests.put(urlLocal+meal+"/17/9",data = json.dumps(mealitem),headers=header)
    return r.status_code
#tests
def test_putmeal():
    r= putmeal()
    assert r ==200
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
    assert j[0]['PatientId'] == 17
def test_postanddeletemeal():
    v = Validator(mealschema)
    j = postmeal()
    assert v.validate(j) == True
    assert j['PatientId'] == 17
    mealId = j['MealId']
    r = deletemeal(mealId)
    assert r == 200


