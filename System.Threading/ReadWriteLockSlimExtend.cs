using System;
using System.Threading;

/*
ThreadSafeCollections

Copyright Philip Pierce © 2010 - 2014

I’ve been doing a lot of mult-threading work, recently, using the standard Thead 
classes, the Worker Queue, PLINQ (Parallel LINQ), and new TPL (Task Parallel Lirary). 
The problem with most of the built-in generic collections (Queue<>, List<>, Dictionary<>, etc), 
is that they are not thread safe. There is a thread-safe collection provided by .NET, but it's
klunky and doesn't support everything I need.

I created a library of thread safe collections which allow me to use the standard 
generic collection actions (foreach, LINQ, etc), while at the same time being thread safe.

The classes in this library inherit from the appropriate collection interface 
(IEnumerable, ICollection, etc). Each class also has all the functions and properties 
that it’s original non-thread safe class has, including new functions (such as
AddIfNotExistElseUpdate for TDictionary).

I've also included an extension to the ReaderWriterLockSlim, which allows me to wrap
Read / Write locks in a more user-friendly manner. In addition, the extensions handle
the issue of always having try/finally blocks on the locks.

I use the ReaderWriterLockSlim a lot, because of its versatility in locking, it's small
use of resources, and its speed. 
*/

namespace System.Threading
{
    /// <summary>
    /// Provides LINQ extensions for the ReaderWriterLockSlim
    /// to make it easier to ensure the lock is released
    /// </summary>
    public static class ReadWriteLockSlimExtend
    {
        /// <summary>
        /// Performs a Read Lock
        /// </summary>
        /// <param name="readerWriterLockSlim">the slim lock</param>
        /// <param name="action">action to perform during the lock</param>
        public static void PerformUsingReadLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            try
            {
                readerWriterLockSlim.EnterReadLock();
                action();
            }
            finally
            {
                readerWriterLockSlim.ExitReadLock();
            }
        }

        /// <summary>
        /// Performs a Read Lock
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="readerWriterLockSlim">the slim lock</param>
        /// <param name="action">action to perform during the lock</param>
        public static T PerformUsingReadLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            try
            {
                readerWriterLockSlim.EnterReadLock();
                return action();
            }
            finally
            {
                readerWriterLockSlim.ExitReadLock();
            }
        }

        /// <summary>
        /// Performs a Write Lock
        /// </summary>
        /// <param name="readerWriterLockSlim">the slim lock</param>
        /// <param name="action">action to perform during the lock</param>
        public static void PerformUsingWriteLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            try
            {
                readerWriterLockSlim.EnterWriteLock();
                action();
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Performs a Write Lock
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="readerWriterLockSlim">the slim lock</param>
        /// <param name="action">action to perform during the lock</param>
        public static T PerformUsingWriteLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            try
            {
                readerWriterLockSlim.EnterWriteLock();
                return action();
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Performs an upgradeable Read / Write Lock
        /// </summary>
        /// <param name="readerWriterLockSlim">the slim lock</param>
        /// <param name="action">action to perform during the lock</param>
        public static void PerformUsingUpgradeableReadLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            try
            {
                readerWriterLockSlim.EnterUpgradeableReadLock();
                action();
            }
            finally
            {
                readerWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Performs an upgradeable Read / Write Lock
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="readerWriterLockSlim">the slim lock</param>
        /// <param name="action">action to perform during the lock</param>
        public static T PerformUsingUpgradeableReadLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            try
            {
                readerWriterLockSlim.EnterUpgradeableReadLock();
                return action();
            }
            finally
            {
                readerWriterLockSlim.ExitUpgradeableReadLock();
            }
        }
    }
}
