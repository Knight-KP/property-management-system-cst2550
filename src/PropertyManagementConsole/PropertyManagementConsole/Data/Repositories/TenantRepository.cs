using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;
using PropertyManagementConsole.Models;

namespace PropertyManagementConsole.Data.Repositories;

public class TenantRepository
{
    public List<Tenant> GetAllTenants()
    {
        var tenants = new List<Tenant>();

        using var conn = new SqlConnection(DbConfig.ConnectionString);
        conn.Open();

        string sql = @"SELECT TenantId, FullName, FlatId, MoveInDate
                       FROM Tenants
                       ORDER BY TenantId;";

        using var cmd = new SqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            tenants.Add(new Tenant
            {
                TenantId = reader.GetInt32(0),
                FullName = reader.GetString(1),
                FlatId = reader.GetInt32(2),
                MoveInDate = reader.GetDateTime(3)
            });
        }

        return tenants;
    }

    public Tenant? GetTenantById(int tenantId)
    {
        using var conn = new SqlConnection(DbConfig.ConnectionString);
        conn.Open();

        string sql = @"SELECT TenantId, FullName, FlatId, MoveInDate
                       FROM Tenants
                       WHERE TenantId = @TenantId;";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@TenantId", tenantId);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new Tenant
        {
            TenantId = reader.GetInt32(0),
            FullName = reader.GetString(1),
            FlatId = reader.GetInt32(2),
            MoveInDate = reader.GetDateTime(3)
        };
    }
}
