using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SoapServer
{
    public class SoapService : ISoapService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public SoapService( DataContext dataContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = dataContext;
        }

        public string Test(string s)
        {
            Console.WriteLine("Test Method Executed!");
            return s;
        }
        public async Task<TrainerResponse> GetTrainers()
        {
            var trainers = await _context.Trainers.ToListAsync();
            return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = trainers };
        }

        public async Task<TrainerResponse> GetTrainer(int id)
        {
            var trainers = await _context.Trainers.ToListAsync();
            var trainer = trainers.Where(x => x.Id == id).FirstOrDefault();
            if (trainer == null)
            {
                return new TrainerResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"User with the id {id} does not exist in database.",
                    Trainers = new List<Trainer>()
                };
            }
            return new TrainerResponse()
            {
                ErrorId = null,
                ErrorMsg = "",
                Trainers = new List<Trainer>() { trainer }
            };
        }

        public async Task<TrainerResponse> RegisterNewTrainer(ConsoleApp1.Trainer trainer)
        {
            try
            {
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new TrainerResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"Error saving data to database.",
                    Trainers = new List<Trainer>()
                };
            }
            return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = new List<Trainer>() { trainer } };
        }

        public async Task<TrainerResponse> UpdateTrainer(Trainer trainer)
        {
            try
            {
                _context.Entry(await _context.Trainers.FirstOrDefaultAsync(x => x.Id == trainer.Id)).CurrentValues.SetValues(trainer);

                //_context.Trainers.Update(trainer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new TrainerResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"Error saving data to database.",
                    Trainers = new List<Trainer>()
                };
            }
            return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = new List<Trainer>() { trainer } };
        }

        public async Task<TrainerResponse> DeleteTrainer(int id)
        {
            var trainers = await _context.Trainers.ToListAsync();
            var trainer = trainers.Where(x => x.Id == id).FirstOrDefault();
            if (trainer == null)
            {
                return new TrainerResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"User with the id {id} does not exist in database.",
                    Trainers = new List<Trainer>()
                };
            }
            try
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new TrainerResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"Error saving data to database.",
                    Trainers = new List<Trainer>()
                };
            }
            return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = new List<Trainer>() { trainer } };
        }
    }
}
