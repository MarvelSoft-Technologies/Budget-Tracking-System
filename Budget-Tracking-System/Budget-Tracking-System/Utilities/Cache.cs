using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Budget_Tracking_System
{
    public class CacheStructure
    {
        public object Data { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
    public class Cache
    {
        static Dictionary<string, CacheStructure> Items = new Dictionary<string, CacheStructure>();

        public static bool Add(string Key, object Data, DateTime ExpiryDate)
        {
            try
            {
                if (ExpiryDate < DateTime.Now) return false;
                var newCacheData = new CacheStructure()
                {
                    Data = Data,
                    ExpiryDate = ExpiryDate
                };

                if (Items.ContainsKey(Key.ToLower()))
                {
                    Items[Key.ToLower()] = newCacheData;
                }
                else
                {
                    Items.Add(Key.ToLower(), newCacheData);
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        public static object Get(string Key)
        {
            try
            {
                if (Items.ContainsKey(Key.ToLower()))
                {
                    var dataInCache = Items[Key.ToLower()];
                    if(dataInCache.ExpiryDate > DateTime.Now)
                    {
                        return dataInCache.Data;
                    }
                    else
                    {
                        Items.Remove(Key.ToLower());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

    }
}
