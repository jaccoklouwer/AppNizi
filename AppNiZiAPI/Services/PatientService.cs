using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Handlers;
using AppNiZiAPI.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static AppNiZiAPI.Services.PatientService;

namespace AppNiZiAPI.Services
{
    public interface IPatientService
    {
        Dictionary<ServiceDictionaryKey, object> TryGetPatientById(int id);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IFeedbackHandler _feedbackHandler;

        public PatientService(IPatientRepository patientRepository, IFeedbackHandler feedbackHandler)
        {
            _patientRepository = patientRepository;
            _feedbackHandler = feedbackHandler;
        }

        public Dictionary<ServiceDictionaryKey, object> TryGetPatientById(int id)
        {
            Dictionary<ServiceDictionaryKey, object> dictionary = new Dictionary<ServiceDictionaryKey, object>();

            try
            {
                Patient patient = _patientRepository.Select(id);

                PatientReturnModel returnModel = new PatientReturnModel()
                {
                    Guid = patient.Guid,
                    DateOfBirth = patient.DateOfBirth,
                    WeightInKilograms = patient.WeightInKilograms,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName
                };

                dictionary.Add(ServiceDictionaryKey.OBJECT, returnModel);
            }
            catch(Exception ex)
            {
                // Build error message and return it.
                string callbackMessage = _feedbackHandler.BuildErrorMessage(ex);
                dictionary.Add(ServiceDictionaryKey.ERROR, callbackMessage);
            }

            return dictionary;
        }

        public enum ServiceDictionaryKey
        {
            OBJECT,
            ERROR
        }
    }
}
