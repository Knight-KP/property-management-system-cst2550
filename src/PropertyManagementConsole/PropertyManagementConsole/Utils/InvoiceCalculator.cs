using System;
using System.Collections.Generic;
using System.Text;

using PropertyManagementConsole.Models;

namespace PropertyManagementConsole.Utils;

public static class InvoiceCalculator
{
    public static decimal CalculateExtrasTotal(List<MaintenanceJob> jobs)
    {
        decimal total = 0;
        foreach (var j in jobs)
        {
            total += j.Cost;
        }
        return total;
    }

    public static decimal CalculateGrandTotal(decimal baseRent, decimal extrasTotal)
    {
        return baseRent + extrasTotal;
    }
}