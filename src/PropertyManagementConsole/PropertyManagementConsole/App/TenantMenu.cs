using System;
using System.Collections.Generic;
using System.Text;

using PropertyManagementConsole.Data.Repositories;
using PropertyManagementConsole.DataStructures;
using PropertyManagementConsole.Models;

namespace PropertyManagementConsole.App;

public static class TenantMenu
{
    public static void LoginAndShow(BinarySearchTree<Tenant> tenantTree)
    {
        Console.Write("\nEnter your Tenant ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tenantId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var tenant = tenantTree.Search(tenantId);
        if (tenant == null)
        {
            Console.WriteLine("Tenant not found.");
            return;
        }

        Console.WriteLine($"\nWelcome {tenant.FullName}!");

        var complaintRepo = new ComplaintRepository();
        var invoiceRepo = new InvoiceRepository();

        while (true)
        {
            Console.WriteLine("\n--- Tenant Menu ---");
            Console.WriteLine("1) View my invoices");
            Console.WriteLine("2) Raise a complaint");
            Console.WriteLine("3) View my complaints");
            Console.WriteLine("0) Logout");
            Console.Write("Choose: ");

            string? choice = Console.ReadLine();

            if (choice == "0") break;

            if (choice == "1")
            {
                ViewMyInvoices(invoiceRepo, tenant.TenantId);
            }
            else if (choice == "2")
            {
                RaiseComplaint(complaintRepo, tenant);
            }
            else if (choice == "3")
            {
                ViewMyComplaints(complaintRepo, tenant.TenantId);
            }
            else
            {
                Console.WriteLine("Invalid option.");
            }
        }
    }

    private static void RaiseComplaint(ComplaintRepository repo, Tenant tenant)
    {
        Console.WriteLine("\nComplaint categories: Electric / Water / Other");
        Console.Write("Category: ");
        string category = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("Category cannot be empty.");
            return;
        }

        Console.Write("Describe the issue: ");
        string desc = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(desc))
        {
            Console.WriteLine("Description cannot be empty.");
            return;
        }

        var complaint = new Complaint
        {
            TenantId = tenant.TenantId,
            FlatId = tenant.FlatId,
            Category = category,
            Description = desc,
            Status = "Open"
        };

        repo.AddComplaint(complaint);
        Console.WriteLine("Complaint submitted ✅");
    }

    private static void ViewMyComplaints(ComplaintRepository repo, int tenantId)
    {
        var list = repo.GetComplaintsByTenant(tenantId);

        if (list.Count == 0)
        {
            Console.WriteLine("No complaints found.");
            return;
        }

        Console.WriteLine("\n--- My Complaints ---");
        foreach (var c in list)
        {
            Console.WriteLine($"#{c.ComplaintId} | {c.Category} | {c.Status} | {c.CreatedAt:yyyy-MM-dd}");
            Console.WriteLine($"   {c.Description}");
        }
    }

    private static void ViewMyInvoices(InvoiceRepository repo, int tenantId)
    {
        var invoices = repo.GetInvoicesByTenant(tenantId);

        if (invoices.Count == 0)
        {
            Console.WriteLine("No invoices found.");
            return;
        }

        Console.WriteLine("\n--- My Invoices ---");
        foreach (var inv in invoices)
        {
            Console.WriteLine($"Invoice #{inv.InvoiceId} | {inv.PeriodMonth:D2}/{inv.PeriodYear} | Total £{inv.GrandTotal} | Created {inv.CreatedAt:yyyy-MM-dd}");
        }

        Console.Write("\nEnter Invoice ID to view details (or 0 to go back): ");
        if (!int.TryParse(Console.ReadLine(), out int invoiceId))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        if (invoiceId == 0) return;

        var lines = repo.GetInvoiceLines(invoiceId);
        if (lines.Count == 0)
        {
            Console.WriteLine("No invoice lines found.");
            return;
        }

        Console.WriteLine("\nInvoice Lines:");
        foreach (var line in lines)
        {
            Console.WriteLine($"- {line.Category}: {line.Description} = £{line.Amount}");
        }
    }
}

