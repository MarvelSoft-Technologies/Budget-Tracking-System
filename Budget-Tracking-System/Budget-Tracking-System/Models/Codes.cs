using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPoco;

namespace Budget_Tracking_System
{
    [TableName("Codes")]
    [PrimaryKey("Id")]
    public class Codes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }

        public static string New(string Key, int Length, string Prefix="", string Suffix="")
        {
            try
            {
                var ThisCode = new Codes
                {
                    Code = Prefix+ Utilities.RandomCode(Length) + Suffix
                };
                while ((Get(ThisCode.Code) != null) || (ThisCode.Code == null))
                {
                    ThisCode.Code = Prefix+ Utilities.RandomCode(Length) + Suffix;
                }
                //ThisCode.Code = ThisCode.Code.ToUpper();
                ThisCode.Key = Key ?? "";
                
                DataContext.Insert(ThisCode);

                if (ThisCode.Id > 1)
                {
                    return ThisCode.Code;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static Codes Get(string Code)
        {
            try
            {
                return DataContext.FirstOrDefault<Codes>("; exec CodesGet @0", Code);
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }
    }
}
