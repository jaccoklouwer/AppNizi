﻿using AppNiZiAPI.Models;
using AppNiZiAPI.Models.Repositories;
using AppNiZiAPI.Security;
using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services
{
    class ConsumptionService : IConsumptionService
    {
        private readonly IConsumptionRepository consumptionRepository;
        private readonly IAuthorization authorizationService;

        public ConsumptionService(IConsumptionRepository consumptionRepository, IAuthorization authorizationService)
        {
            this.consumptionRepository = consumptionRepository;
            this.authorizationService = authorizationService;
        }


        public async Task<ActionResult> AddConsumption(HttpRequest req)
        {
            Consumption newConsumption = new Consumption();
            string consumptionJson = await new StreamReader(req.Body).ReadToEndAsync();
            JsonConvert.PopulateObject(consumptionJson, newConsumption);

            if (!CorrectConsumption(newConsumption)) return new BadRequestObjectResult(Messages.ErrorInvalidConsumptionObject);

            // Auth check
            if (await Authorised(req, newConsumption.PatientId)) return new BadRequestObjectResult(Messages.AuthNoAcces);

            if (consumptionRepository.AddConsumption(newConsumption)) return new OkObjectResult(Messages.OKPost);
            return new BadRequestObjectResult(Messages.ErrorPost);
        }

        public async Task<ActionResult> GetConsumptionByConsumptionId(HttpRequest req, string consumptionId)
        {
            if (!int.TryParse(consumptionId, out int id)) return new BadRequestObjectResult(Messages.ErrorIncorrectId);

            ConsumptionView targetConsumption = consumptionRepository.GetConsumptionByConsumptionId(id);
            // Auth check
            if (await Authorised(req, targetConsumption.PatientId)) return new BadRequestObjectResult(Messages.AuthNoAcces);

            var consumptionJson = JsonConvert.SerializeObject(targetConsumption);
            return consumptionJson != null && targetConsumption.ConsumptionId != 0
                ? (ActionResult)new OkObjectResult(consumptionJson)
                : new BadRequestObjectResult(Messages.ErrorIncorrectId);
        }

        public async Task<ActionResult> GetConsumptionsForPatientBetweenDates(HttpRequest req)
        {
            DateTime startDate;
            DateTime endDate;
            string patientIdString;
            try
            {
                patientIdString = req.Query["patientId"];
                startDate = DateTime.ParseExact(req.Query["startDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(req.Query["endDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult(Messages.ErrorInvalidDateValues);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Messages.ErrorMissingValues);
            }

            if (!int.TryParse(patientIdString, out int patientId)) return new BadRequestObjectResult(Messages.ErrorIncorrectId);

            // Auth check
            if (await Authorised(req, patientId)) return new BadRequestObjectResult(Messages.AuthNoAcces);

            PatientConsumptionsView consumptions = new PatientConsumptionsView(consumptionRepository.GetConsumptionsForPatientBetweenDates(patientId, startDate, endDate));

            var consumptionJson = JsonConvert.SerializeObject(consumptions);
            return consumptionJson != null
                ? (ActionResult)new OkObjectResult(consumptionJson)
                : new BadRequestObjectResult(Messages.ErrorIncorrectId);
        }

        public async Task<ActionResult> RemoveConsumption(HttpRequest req, string consumptionId)
        {
            // TODO: REFACTOR TO SINGLE METHOD
            if (!int.TryParse(consumptionId, out int Id) || Id <= 0) return new BadRequestObjectResult(Messages.ErrorIncorrectId);
            int patientId = consumptionRepository.GetConsumptionByConsumptionId(Id).PatientId;

            // Auth check
            if (await Authorised(req, patientId)) return new BadRequestObjectResult(Messages.AuthNoAcces);

            if (consumptionRepository.DeleteConsumption(Id, patientId)) return new OkObjectResult(Messages.OKDelete);

            return new BadRequestObjectResult(Messages.ErrorDelete);
        }

        public async Task<ActionResult> UpdateConsumption(HttpRequest req, string consumptionId)
        {
            if (!int.TryParse(consumptionId, out int id)) return new BadRequestObjectResult(Messages.ErrorIncorrectId);
           
            int targetPatientId = consumptionRepository.GetConsumptionByConsumptionId(id).PatientId;

            Consumption updateConsumption = new Consumption();
            string consumptionJson = await new StreamReader(req.Body).ReadToEndAsync();
            JsonConvert.PopulateObject(consumptionJson, updateConsumption);

            if (!CorrectConsumption(updateConsumption)) return new BadRequestObjectResult(Messages.ErrorInvalidConsumptionObject);

            // Check if updated consumption patientId equals target patientId
            if (updateConsumption.PatientId != targetPatientId) return new BadRequestObjectResult(Messages.ErrorPut);

            // Auth check
            if (await Authorised(req, targetPatientId)) return new BadRequestObjectResult(Messages.AuthNoAcces);

            if (consumptionRepository.UpdateConsumption(id, updateConsumption)) return new OkObjectResult(Messages.OKUpdate);
            return new BadRequestObjectResult(Messages.ErrorPut);
        }

        private bool CorrectConsumption(Consumption consumption)
        {
            if (consumption.Amount <= 0) return false;
            if (consumption.FoodName.Trim().Length <= 1) return false;
            if (consumption.WeightUnitId == 0) return false;
            if (consumption.PatientId == 0) return false;
            return true;
        }

        private async Task <bool> Authorised(HttpRequest req, int patientId)
        {
            AuthResultModel authResult = await authorizationService.CheckAuthorization(req, patientId);
            if (authResult.Result) return true;
            return false;
        }
    }
}
