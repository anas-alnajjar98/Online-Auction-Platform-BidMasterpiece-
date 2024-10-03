using MasterPiece.Data;
using MasterPiece.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CheckPendingPaymentsService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly EmailHelper _emailHelper;

    public CheckPendingPaymentsService(IServiceScopeFactory serviceScopeFactory, EmailHelper emailHelper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _emailHelper = emailHelper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckPendingPayments(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    public async Task CheckPendingPaymentsWithoutToken()
    {
        await CheckPendingPayments(CancellationToken.None);
    }

    public async Task CheckPendingPayments(CancellationToken stoppingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var _context = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

            var now = DateTime.Now;
            var pendingPayments = await _context.Payments
                .Where(p => p.PaymentStatus == "pending" && p.PaymentDueDate <= now)
                .Include(p => p.Auction)
                .ThenInclude(a => a.Product)
                .ToListAsync(stoppingToken);

            foreach (var payment in pendingPayments)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var user = await _context.Users.FindAsync(payment.UserId);
                if (user != null && !payment.IsNotificationSent)
                {
                    try
                    {
                        _emailHelper.SendMessage(
                            user.Username,
                            user.Email,
                            "Payment Time Expired",
                            $"Your payment for {payment.Auction.Product.ProductName} has expired. The auction will be rescheduled."
                        );

                        payment.IsNotificationSent = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                }

                var auction = payment.Auction;
                auction.CurrentHighestBidderId = null;
                auction.CurrentHighestBid = 0;
                auction.EndTime = DateTime.Now.AddHours(24);
                _context.Auctions.Update(auction);

                // Update the payment status to expired instead of removing it
                payment.PaymentStatus = "expired";
                _context.Payments.Update(payment);
            }

            await _context.SaveChangesAsync(stoppingToken);
        }
    }
}
