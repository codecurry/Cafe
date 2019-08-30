using System.Collections.Generic;
using System.Linq;

namespace Ddd.Api
{
    public static class TableList
    {
        private static readonly object lockObject = new object();
        static TableList()
        {
            AvailableTables = new List<string>(){ "T1", "T2", "T3", "T4", "T5" };
        }
        public static List<string> AvailableTables { get; set; }
        public static List<string> OpenTables { get; set; }
        public static void BookTable(string tableNo)
        {
            lock(lockObject)
            {
                var item = AvailableTables.FirstOrDefault(x => x == tableNo);
                if (item != null)
                {
                    AvailableTables.Remove(item);
                    OpenTables.Add(item);
                }
            }
        }
        public static void CloseTable(string tableNo)
        {
            lock(lockObject)
            {
                var item = OpenTables.FirstOrDefault(x => x == tableNo);
                if (item != null)
                {
                    OpenTables.Remove(item);
                    AvailableTables.Add(item);
                }
            }
        }
    }
}