using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyManagementConsole.Models;
using PropertyManagementConsole.Utils;

namespace PropertyManagementConsole.Tests;

[TestClass]
public class InvoiceCalculatorTests
{
    [TestMethod]
    public void CalculateExtrasTotal_NoJobs_ReturnsZero()
    {
        var jobs = new List<MaintenanceJob>();

        var extras = InvoiceCalculator.CalculateExtrasTotal(jobs);

        Assert.AreEqual(0m, extras);
    }

    [TestMethod]
    public void CalculateExtrasTotal_MultipleJobs_SumsCorrectly()
    {
        var jobs = new List<MaintenanceJob>
        {
            new MaintenanceJob { Cost = 80m },
            new MaintenanceJob { Cost = 120m },
            new MaintenanceJob { Cost = 45.50m }
        };

        var extras = InvoiceCalculator.CalculateExtrasTotal(jobs);

        Assert.AreEqual(245.50m, extras);
    }

    [TestMethod]
    public void CalculateGrandTotal_AddsBaseAndExtras()
    {
        decimal baseRent = 950m;
        decimal extras = 120m;

        decimal total = InvoiceCalculator.CalculateGrandTotal(baseRent, extras);

        Assert.AreEqual(1070m, total);
    }
}