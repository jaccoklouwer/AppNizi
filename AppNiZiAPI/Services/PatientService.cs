using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Enum;
using AppNiZiAPI.Models.Handler;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using AppNiZiAPI.Services.Helpers;
using AppNiZiAPI.Services.Serializer;
using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services
{
    public interface IPatientService
    {
        IMessageHandler FeedbackHandler { get; }

        Task<Dictionary<ServiceDictionaryKey, object>> TryGetPatientById(HttpRequest request, string idText);
        Task<Dictionary<ServiceDictionaryKey, object>> TryListPatients(HttpRequest request);
        Task<Dictionary<ServiceDictionaryKey, object>> TryDeletePatient(HttpRequest request, string idText);

        Task<Dictionary<ServiceDictionaryKey, object>> TryRegisterPatient(HttpRequest request);
        Task<Dictionary<ServiceDictionaryKey, object>> TryUpdatePatient(HttpRequest request);
    }

    public class PatientService : IPatientService
    {
        public IMessageHandler FeedbackHandler { get; }

        private readonly IPatientRepository _patientRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDietaryManagementRepository _dietaryManagementRepository;

        private readonly IMessageSerializer _messageSerializer;
        private readonly IQueryHelper _queryHelper;
        private readonly IAuthorization _authorizationService;

        public PatientService(IPatientRepository patientRepository, IFoodRepository foodRepository,
            IAccountRepository accountRepository, IDietaryManagementRepository dietaryManagementRepository,
            IMessageHandler feedbackHandler, IMessageSerializer messageSerializer,
            IQueryHelper queryHelper, IAuthorization authorizationService)
        {
            _patientRepository = patientRepository;
            this._foodRepository = foodRepository;
            this._accountRepository = accountRepository;
            this._dietaryManagementRepository = dietaryManagementRepository;
            this.FeedbackHandler = feedbackHandler;
            _messageSerializer = messageSerializer;
            _queryHelper = queryHelper;
            this._authorizationService = authorizationService;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryGetPatientById(HttpRequest req, string idText)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            if (!_queryHelper.IsValidId(dictionary, idText))
                return dictionary;

            try
            {
                int id = Int32.Parse(idText);

                // Auth
                try
                {
                    if (!await IsAuthorized(dictionary, req, id))
                        return dictionary;
                }
                catch
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "Auth threw an error. Has the token lifetime expired?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.Unauthorized);
                    return dictionary;
                }

                Patient patient = await _patientRepository.Select(id);

                if (patient.PatientId <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"No patient found for given ID: {id}.");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                PatientReturnModel returnModel = new PatientReturnModel()
                {
                    Id = patient.PatientId,
                    DateOfBirth = patient.DateOfBirth,
                    WeightInKilograms = patient.WeightInKilograms,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName
                };

                dynamic data = _messageSerializer.Serialize(returnModel);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryListPatients(HttpRequest request)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                int amountRequested = _queryHelper.ExtractIntegerFromRequestQuery("count", request);

                List<Patient> patients = await _patientRepository.List(amountRequested);
                PatientReturnModel[] returnModels = new PatientReturnModel[patients.Count];

                for (int i = 0; i < patients.Count; i++)
                {
                    returnModels[i] = new PatientReturnModel(patients[i]);
                }

                dynamic data = _messageSerializer.Serialize(returnModels);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryUpdatePatient(HttpRequest request)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                PatientUpdateModel patient = await _messageSerializer.Deserialize<PatientUpdateModel>(request.Body);

                if (patient.PatientId <= 0)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "Please pass a valid patient ID to update.");
                    return dictionary;
                }

                try
                {
                    if (!await IsAuthorized(dictionary, request, patient.PatientId))
                        return dictionary;
                }
                catch
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "Auth threw an error. Has the token lifetime expired?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.Unauthorized);
                    return dictionary;
                }

                bool success = await _patientRepository.Update(patient);

                if (!success)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"Update failed for given ID: {patient.PatientId}. Does patient exist?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dictionary.Add(ServiceDictionaryKey.VALUE, $"Deleted patient with ID: {patient.PatientId}");
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryRegisterPatient(HttpRequest request)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                AuthLogin authLogin = null;
                // Auth
                try
                {
                    authLogin = await _authorizationService.LoginAuthAsync(request);
                    if (authLogin == null)
                    {
                        dictionary.Add(ServiceDictionaryKey.ERROR, Messages.AuthNoAcces);
                        dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.Unauthorized);
                        return dictionary;
                    }
                }
                catch
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "Auth threw an error. Has the token lifetime expired?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.Unauthorized);
                    return dictionary;
                }

                // Creation
                PatientLogin newPatient = await _messageSerializer.Deserialize<PatientLogin>(request.Body);
                newPatient.Account = _accountRepository.RegisterAccount(newPatient.Patient, Role.Patient);

                newPatient = await _patientRepository.RegisterPatient(newPatient);
                newPatient.Patient = await _patientRepository.Select(newPatient.Patient.PatientId);

                newPatient.AuthLogin = authLogin;

                // Serialization
                dynamic data = _messageSerializer.Serialize(newPatient);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryDeletePatient(HttpRequest req, string idText)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            if (!_queryHelper.IsValidId(dictionary, idText))
                return dictionary;

            try
            {
                int patientId = Int32.Parse(idText);

                // Auth
                try
                {
                    if (!await IsAuthorized(dictionary, req, patientId))
                        return dictionary;
                }
                catch
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, "Auth threw an error. Has the token lifetime expired?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.Unauthorized);
                    return dictionary;
                }

                // Deleting
                Patient patient = await _patientRepository.Select(patientId);
                int accountId = patient.AccountId;

                if (!await TryPerformDelete(patientId, accountId))
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"Deletion failed for given ID: {patientId}. Does patient exist?");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dictionary.Add(ServiceDictionaryKey.VALUE, $"Deleted patient with ID: {patientId}");
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        private async Task<bool> TryPerformDelete(int patientId, int accountId)
        {
            await _dietaryManagementRepository.DeleteByPatientId(patientId);
            await _foodRepository.Delete(patientId);
            bool success = await _patientRepository.Delete(patientId);

            if (accountId > 0)
                _accountRepository.Delete(accountId);

            return success;
        }

        private async Task<bool> IsAuthorized(Dictionary<ServiceDictionaryKey, object> dict, HttpRequest req, int id)
        {
            AuthResultModel authResult = await _authorizationService.AuthForDoctorOrPatient(req, id);

            if (!authResult.Result)
            {
                dict.Add(ServiceDictionaryKey.ERROR, $"Authorization check wasn't passed.");
                HttpStatusCode httpStatusCode = (HttpStatusCode)(int)authResult.StatusCode;
                dict.Add(ServiceDictionaryKey.HTTPSTATUSCODE, httpStatusCode);
            }

            return authResult.Result;
        }

        private void AddErrorMessage(Dictionary<ServiceDictionaryKey, object> dict, Exception ex)
        {
            string callbackMessage = FeedbackHandler.BuildErrorMessage(ex);
            dict.Add(ServiceDictionaryKey.ERROR, callbackMessage);
        }
    }

    public enum ServiceDictionaryKey
    {
        VALUE,
        ERROR,
        HTTPSTATUSCODE
    }
}


// REGISTER PATIENT
//{
//    "Patient": {
//        "FirstName": "Jim",
//        "LastName": "Pickem",
//        "DateOfBirth": "2019-11-03T19:28:10.5222753+01:00",
//        "WeightInKilograms": 88.3,
//        "DoctorId": 2
//    }
//}

/*/
 {
    "Account": null,
    "Patient": {
        "PatientId": 0,
        "AccountId": 0,
        "DoctorId": 0,
        "FirstName": "Jim",
        "LastName": "Pickem",
        "DateOfBirth": "2019-11-03T19:28:10.5222753+01:00",
        "WeightInKilograms": 88.3,
        "Guid": "dVYtmSw5m819mX2nS2raMZwo5lXcwDg6"
    },
    "Doctor": {
        "DoctorId": 2,
        "FirstName": null,
        "LastName": null,
        "Location": null
    },
    "AuthLogin": null
}
 * 
 * 
 */


//private void ReadBody(Stream stream)
//{


//    // Parse Patient Info
//    JObject jsonParsed = JObject.Parse(content);

//    newPatient = new PatientLogin
//    {
//        Patient = new Patient
//        {
//            FirstName = jsonParsed["firstName"].ToString(),
//            LastName = jsonParsed["lastName"].ToString(),
//            DateOfBirth = (DateTime)jsonParsed["dateOfBirth"],
//            WeightInKilograms = (float)jsonParsed["weight"],
//            Guid = authLogin.Guid
//        },
//        Doctor = new DoctorModel
//        {
//            DoctorId = (int)jsonParsed["doctorId"]
//        }
//    };
//}