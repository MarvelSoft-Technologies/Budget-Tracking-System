using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NPoco;

namespace Budget_Tracking_System
{
    public class DataContext
    {
        private static string ConnectionString = Globals.SkillbaseConnectionString;

        public static IDatabase MainContext()
        {
            return new Database(ConnectionString, DatabaseType.SqlServer2012, System.Data.SqlClient.SqlClientFactory.Instance);
        }
        

        public static object Insert(object NewRecord)
        {
            try
            {
                using (var _dbContext = MainContext())
                {
                    return _dbContext.Insert(NewRecord);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static int Update(object Record)
        {
            try
            {
                using (var _dbContext = MainContext())
                {
                    return _dbContext.Update(Record);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static int Delete(object Record)
        {
            try
            {
                using (var _dbContext = MainContext())
                {
                    return _dbContext.Delete(Record);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            try
            {
                using (var _dbContext = MainContext())
                {
                    return _dbContext.Query<T>(sql, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static T FirstOrDefault<T>(string sql, params object[] args)
        {
            try
            {
                using (var _dbContext = MainContext())
                {
                    return _dbContext.FirstOrDefault<T>(sql, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static T ExecuteScalar<T>(string sql, params object[] args)
        {
            try
            {
                using (var _dbContext = MainContext())
                {
                    return _dbContext.ExecuteScalar<T>(sql, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static int Execute(string sql, params object[] args)
        {
            try
            {
                using (var _dbContext = MainContext())
                {
                    return _dbContext.Execute(sql, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

    }
}
