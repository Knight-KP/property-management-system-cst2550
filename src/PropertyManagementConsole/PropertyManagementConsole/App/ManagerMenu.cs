using System;
using System.Collections.Generic;
using System.Text;

using PropertyManagementConsole.Data.Repositories;
using PropertyManagementConsole.Models;
using PropertyManagementConsole.Services;

namespace PropertyManagementConsole.App;

public static class ManagerMenu
{
    public static void Show()
    {
        Console.WriteLine("\nManager Access");

        var tenantRepo = new TenantRepository();
        var complaintRepo = new ComplaintRepository();
        var maintRepo = new MaintenanceRepository();

        while (true)
        {
            Console.WriteLine("\n--- Manager Menu ---");
            Console.WriteLine("1) View tenants");
            Console.WriteLine("2) Add maintenance job");
            Console.WriteLine("3) View maintenance jobs for tenant");
            Console.WriteLine("4) Generate invoice");
            Console.WriteLine("5) View open complaints");
            Console.WriteLine("6) Update complaint status");
            Console.WriteLine("0) Logout");
            Console.Write("Choose: ");

            string? choice = Console.ReadLine();

            if (choice == "0") break;

            if (choice == "1")
            {
                ViewTenants(tenantRepo);
            }
            else if (choice == "2")
            {
                AddMaintenanceJob(maintRepo);
            }
            else if (choice == "3")
            {
                ViewMaintenanceJobs(maintRepo);
            }
            else if (choice == "4")
            {
                GenerateInvoice();
            }
            else if (choice == "5")
            {
                ViewOpenComplaints(complaintRepo);
            }
            else if (choice == "6")
            {
                UpdateComplaintStatus(complaintRepo);
            }
            else
            {
                Console.WriteLine("Invalid option.");
            }
        }
    }

    private static void ViewTenants(TenantRepository repo)
    {
        var tenants = repo.GetAllTenants();

        if (tenants.Count == 0)
        {
            Console.WriteLine("No tenants found.");
            return;
        }

        Console.WriteLine("\n--- Tenants ---");
        foreach (var t in tenants)
        {
            Console.WriteLine($"{t.TenantId}: {t.FullName} | FlatId: {t.FlatId} | Move-in: {t.MoveInDate:yyyy-MM-dd}");
        }
    }

    private static void AddMaintenanceJob(MaintenanceRepository repo)
    {
        Console.Write("\nTenant ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tenantId))
        {
            Console.WriteLine("Invalid Tenant ID.");
            return;
        }

        Console.Write("Flat ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flatId))
        {
            Console.WriteLine("Invalid Flat ID.");
            return;
        }

        Console.WriteLine("Job types: Plumber / Electrician / Other");
        Console.Write("Job type: ");
        string jobType = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(jobType))
        {
            Console.WriteLine("Job type cannot be empty.");
            return;
        }

        Console.Write("Job date (yyyy-mm-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime jobDate))
        {
            Console.WriteLine("Invalid date.");
            return;
        }

        Console.Write("Cost: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal cost))
        {
            Console.WriteLine("Invalid cost.");
            return;
        }

        Console.Write("Notes (optional): ");
        string? notes = Console.ReadLine();

        var job = new MaintenanceJob
        {
            TenantId = tenantId,
            FlatId = flatId,
            JobType = jobType,
            JobDate = jobDate,
            Cost = cost,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        repo.AddJob(job);
        Console.WriteLine("Maintenance job added ✅");
    }

    private static void ViewMaintenanceJobs(MaintenanceRepository repo)
    {
        Console.Write("\nTenant ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tenantId))
        {
            Console.WriteLine("Invalid Tenant ID.");
            return;
        }

        Console.Write("Filter by month/year? (y/n): ");
        string? filter = Console.ReadLine();

        List<MaintenanceJob> jobs;

        if (filter != null && filter.Trim().ToLower() == "y")
        {
            Console.Write("Month (1-12): ");
            if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
            {
                Console.WriteLine("Invalid month.");
                return;
            }

            Console.Write("Year (e.g. 2026): ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Invalid year.");
                return;
            }

            jobs = repo.GetJobsByTenantMonth(tenantId, month, year);
        }
        else
        {
            jobs = repo.GetJobsByTenant(tenantId);
        }

        if (jobs.Count == 0)
        {
            Console.WriteLine("No maintenance jobs found.");
            return;
        }

        Console.WriteLine("\n--- Maintenance Jobs ---");
        foreach (var j in jobs)
        {
            Console.WriteLine($"#{j.JobId} | {j.JobType} | {j.JobDate:yyyy-MM-dd} | £{j.Cost}");
            if (!string.IsNullOrWhiteSpace(j.Notes))
                Console.WriteLine($"   Notes: {j.Notes}");
        }
    }

    private static void GenerateInvoice()
    {
        var service = new InvoiceService();

        Console.Write("\nTenant ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tenantId))
        {
            Console.WriteLine("Invalid Tenant ID.");
            return;
        }

        Console.Write("Flat ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flatId))
        {
            Console.WriteLine("Invalid Flat ID.");
            return;
        }

        Console.Write("Month (1-12): ");
        if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
        {
            Console.WriteLine("Invalid month.");
            return;
        }

        Console.Write("Year (e.g. 2026): ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year.");
            return;
        }

        try
        {
            int invoiceId = service.GenerateInvoice(tenantId, flatId, month, year);
            Console.WriteLine($"Invoice generated ✅ (InvoiceId: {invoiceId})");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not generate invoice: " + ex.Message);
        }
    }

    private static void ViewOpenComplaints(ComplaintRepository repo)
    {
        var list = repo.GetOpenComplaints();

        if (list.Count == 0)
        {
            Console.WriteLine("No open complaints ✅");
            return;
        }

        Console.WriteLine("\n--- Open Complaints ---");
        foreach (var c in list)
        {
            Console.WriteLine($"#{c.ComplaintId} | Tenant {c.TenantId} | Flat {c.FlatId} | {c.Category} | {c.Status} | {c.CreatedAt:yyyy-MM-dd}");
            Console.WriteLine($"   {c.Description}");
        }
    }

    private static void UpdateComplaintStatus(ComplaintRepository repo)
    {
        Console.Write("Enter Complaint ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.WriteLine("New status options: Open / InProgress / Resolved");
        Console.Write("New status: ");
        string status = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(status))
        {
            Console.WriteLine("Status cannot be empty.");
            return;
        }

        bool updated = repo.UpdateComplaintStatus(id, status);
        Console.WriteLine(updated ? "Status updated ✅" : "Complaint ID not found ❌");
    }
}
