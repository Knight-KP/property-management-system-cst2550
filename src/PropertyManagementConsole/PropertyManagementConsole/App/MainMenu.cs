using System;
using System.Collections.Generic;
using System.Text;

using PropertyManagementConsole.DataStructures;
using PropertyManagementConsole.Models;

namespace PropertyManagementConsole.App;

public static class MainMenu
{
    public static void Show(BinarySearchTree<Tenant> tenantTree)
    {
        while (true)
        {
            Console.WriteLine("\n=== Property Management System ===");
            Console.WriteLine("1) Manager");
            Console.WriteLine("2) Tenant");
            Console.WriteLine("0) Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            if (choice == "0") break;

            if (choice == "1")
            {
                ManagerMenu.Show();
            }
            else if (choice == "2")
            {
                TenantMenu.LoginAndShow(tenantTree);
            }
            else
            {
                Console.WriteLine("Invalid option. Try again.");
            }
        }
    }
}
