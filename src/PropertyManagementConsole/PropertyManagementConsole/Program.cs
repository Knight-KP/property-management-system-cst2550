using PropertyManagementConsole.App;
using PropertyManagementConsole.Data.Repositories;
using PropertyManagementConsole.DataStructures;
using PropertyManagementConsole.Models;

var repo = new TenantRepository();
var tenants = repo.GetAllTenants();

// Load tenants into BST for fast search (coursework focus)
var tenantTree = new BinarySearchTree<Tenant>();
foreach (var t in tenants)
{
    tenantTree.Insert(t.TenantId, t);
}

MainMenu.Show(tenantTree);

Console.WriteLine("Goodbye!");
