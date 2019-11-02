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
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services
{
    public interface IPatientService
    {
        Task<Dictionary<ServiceDictionaryKey, object>> TryGetPatientById(int id);
        Task<Dictionary<ServiceDictionaryKey, object>> TryListPatients(HttpRequest request);

        Task<Dictionary<ServiceDictionaryKey, object>> TryRegisterPatient(HttpRequest request, AuthLogin authLogin);
        Task<Dictionary<ServiceDictionaryKey, object>> TryDeletePatient(int id);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IFeedbackHandler _feedbackHandler;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IQueryHelper _queryHelper;

        public PatientService(IPatientRepository patientRepository,
            IFeedbackHandler feedbackHandler, IMessageSerializer messageSerializer,
            IQueryHelper queryHelper)
        {
            _patientRepository = patientRepository;
            _feedbackHandler = feedbackHandler;
            _messageSerializer = messageSerializer;
            _queryHelper = queryHelper;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryGetPatientById(int id)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            if (id <= 0)
            {
                dictionary.Add(ServiceDictionaryKey.ERROR, "No ID passed.");
                return dictionary;
            }

            try
            {
                Patient patient = _patientRepository.Select(id);

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
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, _feedbackHandler);
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
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, _feedbackHandler);
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
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, _feedbackHandler);
            }

            return dictionary;
        }

        public async Task<Dictionary<ServiceDictionaryKey, object>> TryDeletePatient(int id)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            if(id <= 0)
            {
                dictionary.Add(ServiceDictionaryKey.ERROR, "No ID passed.");
                return dictionary;
            }

            try
            {
                bool success = _patientRepository.Delete(id);
                dictionary.Add(ServiceDictionaryKey.VALUE, success);
            }
            catch (Exception ex)
            {
                dictionary.AddErrorMessage(ServiceDictionaryKey.ERROR, ex, _feedbackHandler);
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
            string callbackMessage = _feedbackHandler.BuildErrorMessage(ex);
            dict.Add(ServiceDictionaryKey.ERROR, callbackMessage);
        }
    }

    public enum ServiceDictionaryKey
    {
        VALUE,
        ERROR
    }
}
