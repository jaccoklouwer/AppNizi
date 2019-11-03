using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models;
using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models.Handler;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Services.Helpers;
using AppNiZiAPI.Services.Serializer;
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

        Task<Dictionary<ServiceDictionaryKey, object>> TryGetPatientById(string idText);
        Task<Dictionary<ServiceDictionaryKey, object>> TryListPatients(HttpRequest request);
        Task<Dictionary<ServiceDictionaryKey, object>> TryRegisterPatient(HttpRequest request, AuthLogin authLogin);
        Task<Dictionary<ServiceDictionaryKey, object>> TryDeletePatient(string idText);
    }

    public class PatientService : IPatientService
    {
        public IMessageHandler FeedbackHandler { get; }

        private readonly IPatientRepository _patientRepository;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IQueryHelper _queryHelper;

        public PatientService(IPatientRepository patientRepository,
            IMessageHandler feedbackHandler, IMessageSerializer messageSerializer,
            IQueryHelper queryHelper)
        {
            _patientRepository = patientRepository;
            this.FeedbackHandler = feedbackHandler;
            _messageSerializer = messageSerializer;
            _queryHelper = queryHelper;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryGetPatientById(string idText)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            if (!_queryHelper.IsValidId(dictionary, idText))
                return dictionary;

            try
            {
                int id = Int32.Parse(idText);
                Patient patient = _patientRepository.Select(id);

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

                List<Patient> patients = _patientRepository.List(amountRequested);
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

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryRegisterPatient(HttpRequest request, AuthLogin authLogin)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                PatientLogin newPatient = await _messageSerializer.Deserialize<PatientLogin>(request.Body);
                newPatient = _patientRepository.RegisterPatient(newPatient);
                newPatient.AuthLogin = authLogin;

                dynamic data = _messageSerializer.Serialize(newPatient);
                dictionary.Add(ServiceDictionaryKey.VALUE, data);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryDeletePatient(string idText)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            if (!_queryHelper.IsValidId(dictionary, idText))
                return dictionary;

            try
            {
                int id = Int32.Parse(idText);
                bool success = _patientRepository.Delete(id);

                if (!success)
                {
                    dictionary.Add(ServiceDictionaryKey.ERROR, $"Deletion failed for given ID: {id}");
                    dictionary.Add(ServiceDictionaryKey.HTTPSTATUSCODE, HttpStatusCode.NotFound);
                    return dictionary;
                }

                dictionary.Add(ServiceDictionaryKey.VALUE, $"Deleted patient with ID: {id}");
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, FeedbackHandler);
            }

            return dictionary;
        }

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
