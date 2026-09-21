using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PayRollApi.Application.Interfaces;
using PayRollApi.Domain.Common;
using PayRollApi.Infrastructure.Helper;
using PayRollApi.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace PayRollApi.Infrastructure.Persistence.Repositories
{
    public class CRUDRepository<T>(PayRollDbContext context) : ICRUDinterface<T> where T : class, IAuditable
    {
        public async Task<T> Create(T entity)
        {
           await context.Set<T>().AddAsync(entity);
            return entity;
        }
        public async Task<List<T>> CreateRange(List<T> entity)
        {
            await context.Set<T>().AddRangeAsync(entity);
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await GetById(id);
            context.Set<T>().Remove(entity);
            return true;
        }
        public async Task<bool> DeleteRange(List<T> values)
        {
            context.Set<T>().RemoveRange(values);
            return true;
        }
        public async Task<ICollection<T>> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public async Task<T> GetById(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task Update(T entity)
        {
            context.Set<T>().Update(entity);
        }

        public async Task UpdateRange(List<T> entity)
        {
            context.Set<T>().UpdateRange(entity);

        }
        public async Task<ICollection<T>> GetPages(int currentpage = 0, int pageSize = 100)
        {
            var page = await context.Set<T>().Pagination<T>(currentpage, pageSize);
            return page.Items.ToList();
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }


        #region       Helpers For Dynamic Get All With Conditions
        private static readonly string[] DangerousKeywords =
{
            "insert", "update", "delete", "drop", "alter", "truncate",
            "exec", "execute", "merge", "grant", "revoke", "create",
            "select", "union", "from",
            "xp_", "sp_", "--", "/*", "*/", ";"
        };

        private static readonly HashSet<string> AllowedSqlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "and", "or", "not", "null", "is", "like", "in", "between", "true", "false"
        };


        private async Task<HashSet<string>> GetTableColumnsAsync(string tableName)
        {
            var connection = context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose) await connection.OpenAsync();

            try
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                var p = cmd.CreateParameter();
                p.ParameterName = "@TableName";
                p.Value = tableName;
                cmd.Parameters.Add(p);

                var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    columns.Add(reader.GetString(0));

                if (columns.Count == 0)
                    throw new ArgumentException($"Table '{tableName}' does not exist.");

                return columns;
            }
            finally
            {
                if (shouldClose) await connection.CloseAsync();
            }
        }

        private void ValidateConditionSafety(string condition)
        {
            var lowered = condition.ToLowerInvariant();

            foreach (var keyword in DangerousKeywords)
            {
                if (lowered.Contains(keyword))
                    throw new ArgumentException($"Condition contains a forbidden keyword or pattern: '{keyword}'");
            }

            if (condition.Count(c => c == '\'') % 2 != 0)
                throw new ArgumentException("Condition has unbalanced quotes.");
        }

        private void ValidateConditionColumns(string condition, HashSet<string> allowedColumns)
        {
            var bracketMatches = Regex.Matches(condition, @"\[([^\]]+)\]");
            foreach (Match m in bracketMatches)
            {
                var colName = m.Groups[1].Value;
                if (!allowedColumns.Contains(colName))
                    throw new ArgumentException($"Column '{colName}' does not exist in table.");
            }

            var stripped = Regex.Replace(condition, @"\[[^\]]+\]", "");
            stripped = Regex.Replace(stripped, @"'[^']*'", "");
            stripped = Regex.Replace(stripped, @"\b\d+(\.\d+)?\b", "");

            var suspiciousTokens = Regex.Matches(stripped, @"[A-Za-z_][A-Za-z0-9_]*")
                .Select(x => x.Value)
                .Where(t => !AllowedSqlKeywords.Contains(t))
                .Distinct()
                .ToList();

            if (suspiciousTokens.Any())
                throw new ArgumentException(
                    $"Condition contains unexpected identifier(s): {string.Join(", ", suspiciousTokens)}. Fields must be written as [ColumnName].");
        }

        #endregion
        public async Task<List<T>> GetDynamic(string tableName, string? condition = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name is required.");

            if (string.IsNullOrWhiteSpace(condition))
            {
                return await context.Set<T>()
                    .FromSqlRaw($"SELECT * FROM [{tableName}]")
                    .ToListAsync();
            }

            var allowedColumns = await GetTableColumnsAsync(tableName);
            ValidateConditionSafety(condition);
            ValidateConditionColumns(condition, allowedColumns);

            var sql = $"SELECT * FROM [{tableName}] WHERE {condition}";
            return await context.Set<T>().FromSqlRaw(sql).ToListAsync();
        }

    }
}
