using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.SD;
using Quartz;
using System;

namespace Minotaur.Scheduler
{
    public class ConfirmationCodeHandler : IJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmationCodeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Random random = new();
            int confirmationСode = 0;

            var orders = await _unitOfWork.Orders.GetAllAsync(o => o.OrderStatus == StatusByOrder.DeliveredToPickUp);

            Parallel.ForEach(orders, order =>
            {
                confirmationСode = random.Next(100000, 999999);
                order.ConfirmationCode = random.Next(100000, 999999);
            });

            _unitOfWork.Orders.UpdateRange(orders.ToArray());
            await _unitOfWork.SaveAsync();
        }
    }
}
