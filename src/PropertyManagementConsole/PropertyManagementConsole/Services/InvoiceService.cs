using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;
using PropertyManagementConsole.Data.Repositories;
using PropertyManagementConsole.Models;

namespace PropertyManagementConsole.Services;

public class InvoiceService
{
    private readonly FlatRepository _flatRepo = new FlatRepository();
    private readonly MaintenanceRepository _maintRepo = new MaintenanceRepository();
    private readonly InvoiceRepository _invoiceRepo = new InvoiceRepository();

    public int GenerateInvoice(int tenantId, int flatId, int month, int year)
    {
        var baseRent = _flatRepo.GetBaseRentByFlatId(flatId);
        if (baseRent == null)
            throw new Exception("Flat not found or BaseRent missing.");

        var jobs = _maintRepo.GetJobsByTenantMonth(tenantId, month, year);

        decimal extras = 0;
        foreach (var j in jobs) extras += j.Cost;

        var invoice = new Invoice
        {
            TenantId = tenantId,
            PeriodMonth = month,
            PeriodYear = year,
            BaseRent = baseRent.Value,
            ExtrasTotal = extras
        };

        int invoiceId;
        try
        {
            invoiceId = _invoiceRepo.CreateInvoice(invoice);
        }
        catch (SqlException)
        {
            // Likely duplicate invoice for same period (unique index)
            throw new Exception("Invoice already exists for this tenant and month/year.");
        }

        // Add rent line
        _invoiceRepo.AddInvoiceLine(new InvoiceLine
        {
            InvoiceId = invoiceId,
            Description = $"Monthly Rent ({month:D2}/{year})",
            Amount = baseRent.Value,
            Category = "Rent"
        });

        // Add maintenance lines
        foreach (var j in jobs)
        {
            _invoiceRepo.AddInvoiceLine(new InvoiceLine
            {
                InvoiceId = invoiceId,
                Description = $"{j.JobType} on {j.JobDate:yyyy-MM-dd}" + (string.IsNullOrWhiteSpace(j.Notes) ? "" : $" ({j.Notes})"),
                Amount = j.Cost,
                Category = "Maintenance"
            });
        }

        return invoiceId;
    }
}