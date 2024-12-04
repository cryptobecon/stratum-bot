using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stratumbot.Core
{
    static class TID
    {
        private static Dictionary<object, int> TIDs = new Dictionary<object, int>();

        public static int CurrentID
        {
            get
            {
                if(TIDs.ContainsKey(new { ThreadId = Thread.CurrentThread.ManagedThreadId, TaskId = Task.CurrentId }))
                {
                    return TIDs[new { ThreadId = Thread.CurrentThread.ManagedThreadId, TaskId = Task.CurrentId }];
                } else
                {
                    return 0;
                }
            }
            set
            {
                TIDs[new { ThreadId = Thread.CurrentThread.ManagedThreadId, TaskId = Task.CurrentId }] = value;
            }
        }

        public static void Remove(int _tid)
        {
            var item = TIDs.First(kvp => kvp.Value == _tid);
            TIDs.Remove(item.Key);
        }
    }
}
